using AutoMapper;
using beaconinteriorsapi.Commands;
using beaconinteriorsapi.DTOS;
using beaconinteriorsapi.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using static beaconinteriorsapi.Exceptions.ExceptionHelpers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace beaconinteriorsapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly IValidator<CheckoutDTO> _validator;
        private readonly IMapper _mapper;
        private readonly CheckoutService _checkoutService;
        public CheckoutController(IValidator<CheckoutDTO> validator,IMapper mapper, CheckoutService checkoutService)
        {
           _validator = validator; 
            _mapper = mapper;
            _checkoutService = checkoutService;
        }

        // POST api/<CheckoutController>
        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDTO checkoutDetails)
        {
            ValidationResult result = _validator.Validate(checkoutDetails);
            if (!result.IsValid)
            {
                ThrowBadRequest("unable to valiate checkout details due to bad request", result.ToDictionary());
            }
            var command = _mapper.Map<CheckoutCommand>(checkoutDetails);
            var paymentIntentId = await _checkoutService.CheckoutAsync(command);
            return Ok(paymentIntentId);
        }

    }
}
