using ECommerce.Application.DTOs.Products;
using FluentValidation;

namespace ECommerce.Application.Validators;

/// <summary>
/// FluentValidation validator for CreateProductDto.
/// </summary>
public class CreateProductValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("A valid CategoryId is required.");
    }
}