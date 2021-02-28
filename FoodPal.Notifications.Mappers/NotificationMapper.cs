﻿using FoodPal.Contracts;
using FoodPal.Notifications.Domain;
using FoodPal.Notifications.Processor.Commands;

namespace FoodPal.Notifications.Mappers
{
    public class NotificationMapper : InternalProfile
    {
        public NotificationMapper()
        {
            this.CreateMap<INewNotificationAdded, NewNotificationAddedCommand>();
            this.CreateMap<NewNotificationAddedCommand, Domain.Notification>();
            this.CreateMap<INotificationStatusViewedUpdated, NotificationStatusViewedUpdatedCommand>();
            this.CreateMap<NotificationStatusViewedUpdatedCommand, Domain.Notification>();
        }
    }
}
