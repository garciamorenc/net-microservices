using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Actio.Common.Commands;
using Actio.Common.Eceptions;
using Actio.Common.Events;
using Actio.Services.Activities.Services;
using Microsoft.Extensions.Logging;
using RawRabbit;

namespace Actio.Services.Activities.Handlers
{
    public class CreateActivityHandler : ICommandHandler<CreateActivity>
    {
        private readonly IBusClient _busClient;
        private readonly IActivityService _activityService;
        private ILogger _logger;

        public CreateActivityHandler(IBusClient busClient,
            IActivityService activityService,
            ILogger<CreateActivityHandler> logger)
        {
            this._activityService = activityService;
            this._busClient = busClient;
            this._logger = logger;
        }

        public async Task HandleAsync(CreateActivity command)
        {
            this._logger.LogInformation($"Creating activity: {command.Name}");
            try
            {
                await this._activityService.AddAsync(command.Id, command.UserId,
                    command.Category, command.Name, command.Description, command.CreatedAt);
                await this._busClient.PublishAsync(new ActivityCreated(command.Id,
                  command.UserId, command.Category, command.Name));
                return;
            }
            catch (ActioException ex)
            {
                await this._busClient.PublishAsync(new CreateActivityRejected(command.Id, 
                    ex.Code, ex.Message));
                this._logger.LogError(ex.Message);
            }
            catch (Exception ex)
            {
                await this._busClient.PublishAsync(new CreateActivityRejected(command.Id, 
                    "error", ex.Message));
                this._logger.LogError(ex.Message);
            }
        }
    }
}