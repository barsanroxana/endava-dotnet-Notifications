using MediatR;

namespace FoodPal.Notifications.Processor.Commands
{
    public class NotificationStatusViewedUpdatedCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
