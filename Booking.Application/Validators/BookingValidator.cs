using Booking.App.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.App.Validators
{
    public class BookingValidator : AbstractValidator<BookingDto>
    {
        public BookingValidator()
        {
            RuleFor(x => x.PerformanceId)
                .NotEmpty().WithMessage("PerformanceId is required.");

            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("CustomerName is required.")
                .MaximumLength(100).WithMessage("CustomerName cannot exceed 100 characters.");

            RuleFor(x => x.CustomerEmail)
                .NotEmpty().WithMessage("CustomerEmail is required.")
                .EmailAddress().WithMessage("CustomerEmail must be a valid email address.");
        }
    }
}
