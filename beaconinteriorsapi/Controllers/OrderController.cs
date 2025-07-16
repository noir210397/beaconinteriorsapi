using beaconinteriorsapi.Controllers.Base;
using beaconinteriorsapi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace beaconinteriorsapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : MyBaseController
    {
        private readonly OrderService _orderService;
        protected override string ResourceName => "order";

        public OrderController(OrderService orderService) {
            _orderService = orderService;
        }
        // GET: api/<OrderController>
        [HttpGet]
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


        // DELETE api/<OrderController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var orderId = ToGuidOrThrowBadRequestError(id);
            await _orderService.DeleteOrderAsync(orderId);
            return NoContent();
        }
    }
}
