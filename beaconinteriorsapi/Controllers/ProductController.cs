using beaconinteriorsapi.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace beaconinteriorsapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        // GET: api/<ProductController>
        [HttpGet]
        [Route("/all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IEnumerable<Product> GetProducts()
        {
            var products = new List<Product> { };
            return products;
        }

        // GET api/<ProductController>/5
        [HttpGet("{id:alpha}")]
        public ActionResult<Product> GetSingleProduct(string id)
        {
            var product = new Product("west", "shaken up", "the best of both worlds", 21, 4, new List<string>());
            return Ok(product) ;
        }

        // POST api/<ProductController>
        [HttpPost]
        public void Post([FromBody] Product value)
        {
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id:alpha}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
