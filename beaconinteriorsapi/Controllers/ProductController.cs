using beaconinteriorsapi.Controllers.Base;
using beaconinteriorsapi.DTOS;
using beaconinteriorsapi.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using static beaconinteriorsapi.Exceptions.ExceptionHelpers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace beaconinteriorsapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : MyBaseController
    {
        private readonly ProductService _service;
        private readonly IValidator<CreateProductDTO> _createProductValidator;
        private readonly IValidator<UpdateProductDTO> _updateProductValidator;
        private readonly ILogger<ProductController> _logger;
        protected override string ResourceName => "product";

        public ProductController(ProductService service,IValidator<CreateProductDTO> createProductValidator,IValidator<UpdateProductDTO> updateProductValidator,ILogger<ProductController> logger)
        {
            _service=service;
            _createProductValidator=createProductValidator;
            _updateProductValidator=updateProductValidator;
            _logger=logger;
        }
        // GET: api/<ProductController>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async  Task<IActionResult> GetProducts()
        {      
            _logger.LogInformation("received request to get all products at {Date}",DateTime.UtcNow);
            return Ok(await _service.GetProductsAsync());
        }

        // GET api/<ProductController>/5
        [HttpGet("{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetSingleProduct(string productId)
        {
            _logger.LogInformation("Received request to get product with ID:{ProductID} at {Date}", productId, DateTime.UtcNow);
            var id=ToGuidOrThrowBadRequestError(productId);
            var product =await _service.GetSingleProductAsync(id);
            return Ok(product);
        }

        // POST api/<ProductController>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDTO productDto)
        {
            _logger.LogInformation("Received request to create product:{ProductName} at {Date}", productDto.Name, DateTime.UtcNow);
            ValidationResult result = _createProductValidator.Validate(productDto);
                if (!result.IsValid)
                {
                 ThrowBadRequest("unable to create product due to bad request validation error",result.ToDictionary());
                }
                var product = await _service.AddProductAsync(productDto);
                return CreatedAtAction(nameof(GetSingleProduct),new {productId=product.Id },product);
        }

        // PUT api/<ProductController>/5
        [HttpPut("{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProduct(string productId, [FromForm] UpdateProductDTO productToUpdate)
        {
            _logger.LogInformation("Received request to update product with Id:{ProductId}, Name:{ProductName} at {Date}",productId, productToUpdate.Name, DateTime.UtcNow);
            var id=ToGuidOrThrowBadRequestError(productId);
            ValidationResult result = _updateProductValidator.Validate(productToUpdate);
            if (!result.IsValid)
            {
                 ThrowBadRequest($"unable to update product due to bad request validation error",result.ToDictionary());
            }
            return Ok(await _service.UpdateProductAsync(id, productToUpdate));
            
            
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{productId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteProduct(string productId )
        {
            _logger.LogInformation("received request to delete product with Id:{ProductId} at {Date}",productId, DateTime.UtcNow);
            await _service.RemoveProductByIdAsync(ToGuidOrThrowBadRequestError(productId));
            return NoContent();
        }
        [HttpGet("Category/{category}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchProductsByCategory(string category)
        {
            _logger.LogInformation("received request to search for products in category:{CategoryName} product at {Date}", category, DateTime.UtcNow);
            ValidateString(category, "no category provided to search by");
            return Ok(await _service.GetProductsByCategoryAsync(category));    
        }
        [HttpGet("Name/{category}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchProductsByName(string name)
        {
            _logger.LogInformation("received request to search for products with Name:{ProductName} at {Date}",name, DateTime.UtcNow);
            ValidateString(name,"no name provided to search by");
            return Ok(await _service.GetProductsByNameAsync(name));
        }
    }
}
