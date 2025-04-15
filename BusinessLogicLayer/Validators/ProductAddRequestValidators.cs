using eCommerce.BusinessLogicLayer.DTO;
using FluentValidation;

namespace eCommerce.BusinessLogicLayer.Validators
{
    public class ProductAddRequestValidators : AbstractValidator<ProductAddRequest>
    {
        public ProductAddRequestValidators()
        {
            RuleFor(temp=>temp.ProductName).NotEmpty().WithMessage("Product Name cann't be blank");
            RuleFor(temp=>temp.Category).IsInEnum().WithMessage("Please provide valid category");
            RuleFor(temp=>temp.UnitPrice).InclusiveBetween(0,double.MaxValue)
                .WithMessage($"Unit price should be between 0 to {double.MaxValue}");
            RuleFor(temp => temp.QuantityInStock).InclusiveBetween(0, int.MaxValue)
              .WithMessage($"Quantity In Stock should be between 0 to {int.MaxValue}");
        }
    }
}
