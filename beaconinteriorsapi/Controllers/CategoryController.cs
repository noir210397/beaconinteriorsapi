using AutoMapper;
using beaconinteriorsapi.Controllers.Base;
using beaconinteriorsapi.DTOS;
using beaconinteriorsapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace beaconinteriorsapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : MyBaseController
    {
        private readonly CategoryService _categoryService;
        private readonly ILogger _logger;
        protected override string ResourceName => "category";

        public CategoryController(CategoryService categoryService,IMapper mapper,ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }
        // GET: api/<CategoryController>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCategories()
        {
            _logger.LogInformation("received request to get all categories at {Date}",DateTime.UtcNow);
            var categories = await _categoryService.GetCategoriesAsync();
           return Ok(categories);
        }

        // GET api/<CategoryController>/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSingleCategory(int id)
        {
            _logger.LogInformation("received request to get category with Id:{ProductId} at {Date}",id, DateTime.UtcNow);
            return Ok(await _categoryService.GetSingleCategoryAsync(id));
        }

        // POST api/<CategoryController>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> AddCategory([FromBody] CategoryDTO categoryDto)
        {
            _logger.LogInformation("received request to add category with name:{ProducName} at {Date}", categoryDto.Name, DateTime.UtcNow);
            ValidateString(categoryDto.Name);
            var category = await _categoryService.AddCategoryAsync(categoryDto.Name);
            return CreatedAtAction(nameof(GetSingleCategory), new {id=category.Id},category);
        }

        // PUT api/<CategoryController>/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDTO category)
        {

            _logger.LogInformation("received request to update product with Id:{ProductId} to new Value:{CategoryName} at {Date}", id,category, DateTime.UtcNow);
            ValidateString(category.Name);
            return Ok(await _categoryService.UpdateCategoryAsync(id, category.Name));
        }

        // DELETE api/<CategoryController>/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            _logger.LogInformation("received request to delete product with Id:{ProductId} at {Date}", id, DateTime.UtcNow);
            await _categoryService.DeleteCategoryAsync(id);
            return NoContent();
        }
    }
}
