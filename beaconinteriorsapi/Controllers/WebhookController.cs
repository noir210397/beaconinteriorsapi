using beaconinteriorsapi.Data;
using beaconinteriorsapi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace beaconinteriorsapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly BeaconInteriorsDBContext _context;
        private readonly ILogger  _logger;

        public WebhookController(BeaconInteriorsDBContext context,ILogger<WebhookController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpPost("Stripe")]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            string endpointSecret = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET") ?? throw new Exception("stripe key not found"); ;
            try
            {
                var stripeEvent = EventUtility.ParseEvent(json);
                var signatureHeader = Request.Headers["Stripe-Signature"];

                stripeEvent = EventUtility.ConstructEvent(json,
                        signatureHeader, endpointSecret);

                // If on SDK version < 46, use class Events instead of EventTypes
                if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    if (paymentIntent != null)
                    {
                        var orderId = Guid.Parse(paymentIntent.Metadata["order_id"]);

                        var order = await _context.Orders.FindAsync(orderId);
                        if (order != null)
                        {
                            order.PaymentStatus = PaymentStatusType.Paid; // or appropriate status enum
                            await _context.SaveChangesAsync();
                            //send email confirmation here
                        }
                    }
                    _logger.LogInformation("A successful payment for {0} was made.", paymentIntent.Amount);
                    // Then define and call a method to handle the successful payment intent.
                    // handlePaymentIntentSucceeded(paymentIntent);
                }
                else if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    if (paymentIntent != null) { 
                    var orderId = Guid.Parse(paymentIntent.Metadata["order_id"]);

                    var order = await _context.Orders.FindAsync(orderId);
                    if (order != null)
                    {
                        order.PaymentStatus = PaymentStatusType.Failed; // or appropriate status enum
                        await _context.SaveChangesAsync();
                    }
                    }
                    // Then define and call a method to handle the successful attachment of a PaymentMethod.
                    // handlePaymentMethodAttached(paymentMethod);
                }
                else if (stripeEvent.Type == EventTypes.PaymentIntentCanceled)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    if (paymentIntent != null)
                    {
                        var orderId = Guid.Parse(paymentIntent.Metadata["order_id"]);

                        var order = await _context.Orders.FindAsync(orderId);
                        if (order != null)
                        {
                            order.PaymentStatus = PaymentStatusType.Canceled; // or appropriate status enum
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                else
                {
                    _logger.LogInformation("Unhandled event type: {0}", stripeEvent.Type);
                }
                return Ok();
            }
            catch (StripeException e)
            {
                _logger.LogError("Error: {0}", e.Message);
                return BadRequest();
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("Tracking")]
        public async Task<IActionResult> TrackUpdate()
        {
            var tracking = await _context.Tracking.FirstOrDefaultAsync();

            if (tracking == null)
            {
                // If no tracking record, create one
                tracking = new Tracking
                {
                    CurrentTrackingCount = 0,
                    DateTime = DateTime.UtcNow.Date
                };

                _context.Tracking.Add(tracking);
                await _context.SaveChangesAsync();
                return Ok("Tracking record created.");
            }

            var today = DateTime.UtcNow.Date;

            if (tracking.DateTime < today)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Update date and increment count
                    tracking.DateTime = today;
                    tracking.CurrentTrackingCount += 1;

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Ok($"Tracking updated. Count: {tracking.CurrentTrackingCount}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"Error updating tracking: {ex.Message}");
                }
            }
            else
            {
                return Ok("Tracking already up-to-date.");
            }
        }


        [HttpGet("Orders")]
        public async Task AuditAndExpireOrdersAsync()
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var orders = await _context.Orders.Where(o=>o.PaymentStatus==PaymentStatusType.Processing|| o.PaymentStatus == PaymentStatusType.Pending).ToListAsync();

                foreach (var order in orders)
                {
                    var timeSinceCreation = DateTime.UtcNow - order.CreatedDate;

                    if (order.PaymentStatus == PaymentStatusType.Pending && timeSinceCreation.TotalMinutes > 15)
                    {
                        order.HasExpired = true;
                    }
                    else if (order.PaymentStatus == PaymentStatusType.Processing && timeSinceCreation.TotalHours > 2)
                    {
                        order.PaymentStatus = PaymentStatusType.Pending;
                        order.HasExpired = true;
                        //release products back to store
                    }

                    // You can also optionally log or collect affected orders here
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}
