using AutoMapper;
using beaconinteriorsapi.Commands;
using beaconinteriorsapi.Data;
using beaconinteriorsapi.DTOS;
using beaconinteriorsapi.Models;
using beaconinteriorsapi.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using StripeSDK=Stripe;
using static beaconinteriorsapi.Exceptions.ExceptionHelpers;

namespace beaconinteriorsapi.Services
{
    public class CheckoutService
    {
        private readonly BeaconInteriorsDBContext _dbContext;
        private readonly IMapper _mapper;
       
        public CheckoutService(BeaconInteriorsDBContext dbContext,IMapper mapper) { 
        _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<string> CheckoutAsync(CheckoutCommand command)
        {
            var (isValid, errors, orderItems, userId) = await ValidateOrderAsync(command);
            if (!isValid)
            {
                ThrowBadRequest("unable to place order due to bad request",errors);
            }
            var orderId = await CreateOrderAsync(userId, orderItems, command);
            var total = CalculateTotal(orderItems);
            var paymentIntentId = await CreatePaymentIntentAsync(total,orderId);
            return paymentIntentId;

        }
        private async Task<(bool IsValid, IDictionary<string, string[]> errors, List<OrderItems> orderItems, Guid? userId)> ValidateOrderAsync(CheckoutCommand command)
        {
            Dictionary<string, string[]> errors = new ();
            Guid? userGuid = null;
            var orderItems = new List<OrderItems>();
            //ensure atomicity
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            if (command.UserId != null) {
                //find user here if not good add error to dictionary
            }
            foreach (var item in command.Items)
            {
                var itemErrors = new List<string>();
                    var product = await _dbContext.Products
                        .Where(p => p.Id == item.Id)
                        .FirstOrDefaultAsync();

                    if (product == null)
                    {
                        itemErrors.Add("Product not found.");
                    }
                    else if (product.Quantity < item.Quantity)
                    {
                        itemErrors.Add($"Insufficient stock. Requested: {item.Quantity}, Available: {product.Quantity}.");
                    }
                    else
                    {
                        // Optional: reserve stock here by decrementing temporarily
                        product.Quantity -= item.Quantity;
                        var orderItem = _mapper.Map<OrderItems>(product);
                        orderItem.Quantity = item.Quantity;
                        orderItems.Add(orderItem);
                    }

                if (itemErrors.Any())
                {
                    errors[item.Id.ToString()] = itemErrors.ToArray();
                }
            }

            if (errors.Count > 0)
            {
                await transaction.RollbackAsync();
                return (false, errors, [], userGuid);
            }

             await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            return (true, errors, orderItems, userGuid);

        }
        private async Task<string> CreateOrderAsync(Guid? userId, List<OrderItems> orderItems, CheckoutCommand checkoutDTO)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                List<Address> addresses = new List<Address>();
                //return user from validating order and use here then add to order
                if (userId != null)
                {
                    // Optional: validate user exists and retrieve if needed
                }

                // Add shipping address if it exists
                if (checkoutDTO.ShippingAddress != null)
                {
                    var shippingAddress = _mapper.Map<Address>(checkoutDTO.ShippingAddress);
                    shippingAddress.AddressType = AddressType.Shipping;
                    addresses.Add(shippingAddress);
                }

                // Add billing address (required)
                var billingAddress = _mapper.Map<Address>(checkoutDTO.BillingAddress);
                billingAddress.AddressType = AddressType.Billing;
                addresses.Add(billingAddress);

                // Get and update tracking safely inside transaction
                var tracking = await _dbContext.Tracking.FirstOrDefaultAsync();
                if (tracking == null)
                {
                    throw new InvalidOperationException("Tracking record not found.");
                }
                tracking.CurrentTrackingCount++;
                var trackingId = $"BI{tracking.DateTime.ToString("ddMMyyyy")}-{tracking.CurrentTrackingCount.ToTrackingFormat()}";
                var newOrder = new Order
                {
                    PhoneNumber = checkoutDTO.PhoneNumber!,
                    EmailAddress = checkoutDTO.EmailAddress!,
                    Addresses = addresses,
                    Status = OrderStatusType.Pending,
                    PaymentStatus = PaymentStatusType.Pending,
                    TrackingID = trackingId,
                    Items = orderItems,
                    //UserId = userId
                };

                await _dbContext.Orders.AddAsync(newOrder);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return trackingId;
            }
            catch
            {
                await transaction.RollbackAsync();
                ThrowServerError("unable to create order due to internal server error");
                throw;
            }
        }
        private async Task<string> CreatePaymentIntentAsync(long amountInCents, string trackingId, string currency = "gbp")
        {
            var stripeKey = Environment.GetEnvironmentVariable("STRIPE_API_KEY") ?? throw new Exception("stripe key not found");
            try
            {
                var stripeClient = new StripeSDK.StripeClient(stripeKey);

                var paymentIntentService = new StripeSDK.PaymentIntentService(stripeClient);

                var options = new StripeSDK.PaymentIntentCreateOptions
                {
                    Amount = amountInCents,
                    Currency = currency,
                    PaymentMethodTypes = new List<string> { "card" },
                    Metadata = new Dictionary<string, string>() { { "tracking_id", trackingId } }
                };

                var paymentIntent = await paymentIntentService.CreateAsync(options);

                return paymentIntent.Id;
            }
            catch (Exception e)
            {
                ThrowServerError($"unable to create payment intent due to server error and stripe error:{e.Message}");
                throw;
            }
            
        }
        private long CalculateTotal(List<OrderItems> orderItems)
        {
            var total = orderItems.Sum(item => item.Quantity * item.Price);
            return total.ToPence(); 
        }

    }
}
