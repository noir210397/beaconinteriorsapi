using beaconinteriorsapi.DTOS;
using FluentValidation;

namespace beaconinteriorsapi.Validators
{
    public abstract class BaseProductValidator<T> : AbstractValidator<T> where T : BaseCreateOrUpate
    {
        public BaseProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.");

            RuleFor(x => x.ShortDescription)
                .NotEmpty().WithMessage("Short description is required.")
                .MaximumLength(200).WithMessage("Short description cannot exceed 200 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Full description is required.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity cannot be negative.");

            RuleFor(x => x.Dimensions)
                .NotEmpty().WithMessage("Dimensions are required.");

            RuleFor(x => x.Summary)
                .NotEmpty().WithMessage("Summary is required.");

            RuleFor(x => x.Categories)
                .NotNull().WithMessage("Categories are required.")
                .Must(cats => cats.Count > 0).WithMessage("At least one category is required.");
        }
    }
    public class CreateProductValidator : BaseProductValidator<CreateProductDTO>
    {
        private readonly List<string> _allowedExtensions = new() { ".jpg", ".jpeg", ".png" };
        private const long _maxFileSize = 5 * 1024 * 1024;

        public CreateProductValidator()
        {
            RuleFor(x => x.Images)
                .NotNull().WithMessage("Product images are required.")
                .Must(files => files.Count > 0).WithMessage("Please upload at least one image.")
                .Custom((files, context) =>
                {
                    var tooLarge = files.Where(f => f.Length > _maxFileSize).Select(f => f.FileName).ToList();
                    if (tooLarge.Any())
                    {
                        context.AddFailure("Images",$"The following files are larger than 5MB: {string.Join(", ", tooLarge)}");
                    }

                    var invalidTypes = files.Where(f =>
                        !_allowedExtensions.Contains(System.IO.Path.GetExtension(f.FileName).ToLower())
                    ).Select(f => f.FileName).ToList();

                    if (invalidTypes.Any())
                    {
                        context.AddFailure("Images", $"Only .png, .jpg, .jpeg files are allowed. Invalid files: {string.Join(", ", invalidTypes)}");
                    }
                });
        }
    }
    public class UpdateProductValidator : BaseProductValidator<UpdateProductDTO>
    {
        private readonly List<string> _allowedExtensions = new() { ".jpg", ".jpeg", ".png" };
        private const long _maxFileSize = 5 * 1024 * 1024;

        public UpdateProductValidator()
        {

            RuleFor(x => x)
                .Custom((model, context) =>
                {
                    var hasCurrent = model.CurrentFiles != null && model.CurrentFiles.Count > 0;
                    var hasNew = model.NewFiles != null && model.NewFiles.Count > 0;

                    // Ensure at least one image remains (either kept or newly added)
                    if (!hasCurrent && !hasNew)
                    {
                        context.AddFailure("At least one image is required. Either retain an existing image or upload a new one.");
                    }

                    // Validate new file sizes
                    if (hasNew)
                    {
                        var tooLarge = model.NewFiles
                            .Where(f => f.Length > _maxFileSize)
                            .Select(f => f.FileName)
                            .ToList();

                        if (tooLarge.Any())
                        {
                            context.AddFailure($"The following files are larger than 5MB: {string.Join(", ", tooLarge)}");
                        }

                        var invalidExtensions = model.NewFiles
                            .Where(f => !_allowedExtensions.Contains(Path.GetExtension(f.FileName).ToLower()))
                            .Select(f => f.FileName)
                            .ToList();

                        if (invalidExtensions.Any())
                        {
                            context.AddFailure($"Only .png, .jpg, .jpeg files are allowed. Invalid files: {string.Join(", ", invalidExtensions)}");
                        }
                    }
                });
        }
    }
}
