using AutoMapper;
using FoodPal.Notifications.Common.Enums;
using FoodPal.Notifications.Common.Exceptions;
using FoodPal.Notifications.Data.Abstractions;
using FoodPal.Notifications.Processor.Commands;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FoodPal.Notifications.Processor.BackgroundWorker
{
    public class NotificationErrorWorker : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private Timer _timer;

        public NotificationErrorWorker(IUnitOfWork unitOfWork, IMediator mediator, IMapper mapper, ILogger<NotificationErrorWorker> logger)
        {
            this._unitOfWork = unitOfWork;
            this._mediator = mediator;
            this._mapper = mapper;
            this._logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(5));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            var notificationModelQueryable = _unitOfWork.GetRepository<Domain.Notification>()
                 .Find(notification => NotificationStatusEnum.Error.Equals(notification.Status)).AsEnumerable().ToList();

            foreach(Domain.Notification notification in notificationModelQueryable)
           {
               try
               {
                   var command = this._mapper.Map<ErrorNotificationCommand>(notification);
                    await this._mediator.Send(command);

               }
               catch (ValidationsException e)
               {
                   var errors = e.Errors.Aggregate((curr, next) => $"{curr}; {next}");
                   this._logger.LogError(e, errors);
               }
               catch (Exception e)
               {
                   this._logger.LogError(e, $"Something went wrong in {nameof(NotificationErrorWorker)}");
               }
           }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);
            
            return Task.CompletedTask;
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
