using AutoMapper;
using FoodPal.Contracts;
using FoodPal.Notifications.Dto.Exceptions;
using FoodPal.Notifications.Processor.Commands;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FoodPal.Notifications.Processor.Messages.Consumers
{
    public class UserUpdatedConsumer : IConsumer<IUserUpdated>
    { 
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<UserUpdatedCommand> _logger;

        public UserUpdatedConsumer(IMediator mediator, IMapper mapper, ILogger<UserUpdatedCommand> logger)
        { 
            this._mediator = mediator;
            this._mapper = mapper;
            this._logger = logger;
        }

        public async Task Consume(ConsumeContext<IUserUpdated> context)
        {
            try
            {
                var message = context.Message;

                var command = this._mapper.Map<UserUpdatedCommand>(message);

                // TODO: refactor this 
                await this._mediator.Send(command);
            }
            catch (ValidationsException e)
            {
                // TODO: offer validation to end user by persisting it to an audit/log 

                var errors = e.Errors.Aggregate((curr, next) => $"{curr}; {next}");
                this._logger.LogError(e, errors);
            }
            catch (Exception e)
            { 
                this._logger.LogError(e, $"Something went wrong in {nameof(UserUpdatedConsumer)}");
            }
        }
    }
}
