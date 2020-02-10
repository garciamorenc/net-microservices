using System;
using System.Threading.Tasks;
using Actio.Common.Commands;
using Actio.Common.Eceptions;
using Actio.Common.Events;
using Actio.Services.Identity.Services;
using Microsoft.Extensions.Logging;
using RawRabbit;

namespace Actio.Services.Identity.Handlers
{
    public class CreateUserHandler : ICommandHandler<CreateUser>
    {
        private readonly IBusClient _busClient;
        private readonly IUserService _userService;
        private readonly ILogger _logger;

        public CreateUserHandler(IBusClient busClient,
            IUserService userService,
            ILogger<CreateUserHandler> logger)
        {
            this._busClient = busClient;
            this._userService = userService; 
            this._logger = logger;           
        }

        public async Task HandleAsync(CreateUser command)
        {
            this._logger.LogInformation($"Creating user: {command.Email} {command.Name}");
            try
            {
                await this._userService.RegisterAsync(command.Email, command.Password, 
                    command.Name);
                await this._busClient.PublishAsync(new UserCreated(command.Email, 
                    command.Name));
                return;
            }
            catch (ActioException ex)
            {
                await this._busClient.PublishAsync(new CreateUserRejected(command.Email, 
                    ex.Code, ex.Message));
                this._logger.LogError(ex.Message);
            }
            catch (Exception ex)
            {
                await this._busClient.PublishAsync(new CreateUserRejected(command.Email, 
                    "error", ex.Message));
                this._logger.LogError(ex.Message);
            }
        }
    }
}