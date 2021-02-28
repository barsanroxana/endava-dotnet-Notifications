using FluentValidation;
using FoodPal.Notifications.Processor.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodPal.Notifications.Validations
{
    public class NotificationStatusViewedUpdatedCommandValidator : InternalValidator<NotificationStatusViewedUpdatedCommand>
    {
        public NotificationStatusViewedUpdatedCommandValidator()
        {
            this.RuleFor(x => x.Id).NotEmpty();
        }
    }
}
