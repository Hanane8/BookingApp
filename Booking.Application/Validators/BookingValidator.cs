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

            
        }
    }
}
