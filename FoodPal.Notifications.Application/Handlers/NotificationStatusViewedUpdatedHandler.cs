using AutoMapper;
using FluentValidation;
using FoodPal.Notifications.Service;
using FoodPal.Notifications.Application.Extensions;
using FoodPal.Notifications.Data.Abstractions;
using FoodPal.Notifications.Domain;
using FoodPal.Notifications.Dto.Exceptions;
using FoodPal.Notifications.Dto.Intern;
using FoodPal.Notifications.Processor.Commands;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FoodPal.Notifications.Application.Handlers
{
    public class NotificationStatusViewedUpdatedHandler : IRequestHandler<NotificationStatusViewedUpdatedCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<NotificationStatusViewedUpdatedCommand> _validator;

        public NotificationStatusViewedUpdatedHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<NotificationStatusViewedUpdatedCommand> validator)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._validator = validator;
        }

        public async Task<bool> Handle(NotificationStatusViewedUpdatedCommand request, CancellationToken cancellationToken)
        {
            this._validator.ValidateAndThrowEx(request);

            var notoficationModel = await this._unitOfWork.GetRepository<Notification>().FindByIdAsync(request.Id);

            // change the notification status
            notoficationModel.Status = Common.Enums.NotificationStatusEnum.Viewed;
            return await this._unitOfWork.SaveChangesAsnyc();
        }
    }
}
