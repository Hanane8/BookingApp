using Booking.App.DTOs;
using FluentValidation;

namespace Booking.App.Validators
{
    public class BookPerformanceValidator : AbstractValidator<BookPerformanceDto>
    {
        public BookPerformanceValidator()
        {
            RuleFor(x => x.PerformanceId)
                .NotEmpty().WithMessage("PerformanceId is required.")
                .GreaterThan(0).WithMessage("PerformanceId must be greater than 0.");
        }
    }
} 