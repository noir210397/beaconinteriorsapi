using beaconinteriorsapi.DTOS;
using beaconinteriorsapi.Utils;
using FluentValidation;

namespace beaconinteriorsapi.Validators
{


    public class CheckoutDTOValidator : AbstractValidator<CheckoutDTO>
    {
        public CheckoutDTOValidator()
        {
            RuleFor(x => x.UserId)
                .Custom((userId, context) =>
                {
                    if (userId != null && !GuidUtils.ToGuid(userId))
                    {
                        context.AddFailure("Invalid userId provided.");
                    }
                });

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("At least one item is required.")
                .Custom((items, context) =>
                {
                    if (items.Any(item =>
                            string.IsNullOrWhiteSpace(item.Id) || !GuidUtils.ToGuid(item.Id) || item.Quantity <= 0))
                    {
                        context.AddFailure("All items must have a valid ID and quantity greater than 0.");
                    }
                });
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number is required");
            RuleFor(x => x.EmailAddress).NotEmpty().WithMessage("Email address is required");

            RuleFor(x=>x.BillingAddress).SetValidator(new AddressValidator());
            RuleFor(x=>x.ShippingAddress!).
                SetValidator(new AddressValidator())
                .When(x => x.ShippingAddress != null); ;
        }
    }
    public class AddressValidator:AbstractValidator<AddressDTO>
    {
        public AddressValidator()
        {
            RuleFor(x=>x.StreetAddress)
                .NotEmpty()
                .WithMessage("please provide a valid street address");
            RuleFor(x => x.PostCode)
            .NotEmpty()
            .WithMessage("please provide a valid post code");
           RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("please provide a valid city");
           RuleFor(x => x.StreetAddress)
            .NotEmpty()
            .WithMessage("please provide a valid country");
        }

    }


}

