using beaconinteriorsapi.Controllers.Base;
using beaconinteriorsapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace beaconinteriorsapi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : MyBaseController
    {
        private readonly OrderService _orderService;
        private readonly ClaimsPrincipal _principal;
        protected override string ResourceName => "Order";

        public OrderController(OrderService orderService,ClaimsPrincipal principal) {
            _orderService = orderService;
            _principal=principal;
        }
        // GET: api/<OrderController>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            return Ok(await _orderService.GetAllOrdersAsync());
        }

        // GET api/<OrderController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingleOrder(string id)
        {
            var orderId = ToGuidOrThrowBadRequestError(id);
            return Ok(await _orderService.GetSingleOrderAsync(orderId));

        }
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserOrders(string userId)
        {
            ValidateString(userId);
            return Ok(await _orderService.GetUserOrdersAsync(userId));
        }
        [HttpGet("tracking/{trackingID}")]
        public async Task<IActionResult> TrackOrder(string trackingId)
        {
            ValidateString(trackingId);
            return Ok(await _orderService.TrackOrderAsync(trackingId));
        }
        [HttpGet("check/{orderId}")]
        public async Task<IActionResult> CheckOrder(string orderId)
        {
            ValidateString(orderId);
            var expired = await _orderService.CheckIfOrderExpiredAsync(orderId);
            if(expired)
                return StatusCode(410, "This order has expired.");
            else
                return NoContent();
        }
        // DELETE api/<OrderController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var roles=_principal.FindAll(ClaimTypes.Role).Select(c=>c.Value);
            var userId = _principal.FindFirst(JwtRegisteredClaimNames.Sub)!.Value;
            var orderId = ToGuidOrThrowBadRequestError(id);
            await _orderService.DeleteOrderAsync(orderId,roles,userId);
            return NoContent();
        }

    }
}
