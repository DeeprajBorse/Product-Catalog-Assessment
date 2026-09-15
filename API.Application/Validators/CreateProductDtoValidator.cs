using API.Application.DTO;
using API.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Application.Validators
{
    public class CreateProductDTOValidator : AbstractValidator<CreateProductDTO>
    {
        public CreateProductDTOValidator()
        {
            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(255).WithMessage("Product name must not exceed 255 characters.");

            RuleFor(x => x.CreatedBy)
                .NotEmpty().WithMessage("CreatedBy is required.")
                .MaximumLength(100).WithMessage("CreatedBy must not exceed 100 characters.");

            RuleFor(x => x.ProductDescription)
                .MaximumLength(1000).WithMessage("Product description must not exceed 1000 characters.");

            RuleFor(x => x.ProductPrice)
               .GreaterThan(0).WithMessage("Product price must be greater than zero.");

            RuleForEach(x => x.Items)
                .SetValidator(new CreateItemDTOValidator());
        }
    }

    public class CreateItemDTOValidator  : AbstractValidator<CreateItemDTO>
    {
        public CreateItemDTOValidator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be a positive number.");
        }
    }
}
