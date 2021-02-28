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
    public class ErrorNotificationHandler : IRequestHandler<ErrorNotificationCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public ErrorNotificationHandler(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._notificationService = notificationService; 
        }

        public async Task<bool> Handle(ErrorNotificationCommand request, CancellationToken cancellationToken)
        {
            var errorNotificationModel = this._mapper.Map<Domain.Notification>(request);

            var userModel = await this._unitOfWork.GetRepository<User>().FindByIdAsync(errorNotificationModel.UserId);
            var notificationServiceDto = new NotificationServiceDto
            {
                Body = errorNotificationModel.Message,
                Email = userModel.Email,
                Subject = errorNotificationModel.Title,
                PhoneNo = userModel.PhoneNo
            };
            var sent = await this._notificationService.Send(errorNotificationModel.Type, notificationServiceDto);

            // change the notification status
            var notoficationModel = await this._unitOfWork.GetRepository<Notification>().FindByIdAsync(request.Id);
            notoficationModel.Status = sent ? Common.Enums.NotificationStatusEnum.Viewed : Common.Enums.NotificationStatusEnum.Error;
            await this._unitOfWork.SaveChangesAsnyc();

            return sent;
        }
    }
}
