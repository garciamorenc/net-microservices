using System.Threading.Tasks;
using Actio.Common.Auth;
using Actio.Common.Eceptions;
using Actio.Services.Identity.Domain.Models;
using Actio.Services.Identity.Domain.Repositories;
using Actio.Services.Identity.Domain.Services;

namespace Actio.Services.Identity.Services
{
    public class UserService : IUserService
    {
        public readonly IUserRepository _userRepository;
        public readonly IEncrypter _encrypter;
        public readonly IJwtHandler _jwtHandler;

        public UserService(IUserRepository userRepository,
            IEncrypter encrypter,
            IJwtHandler jwtHandler)
        {
            this._userRepository = userRepository;
            this._encrypter = encrypter;
            this._jwtHandler = jwtHandler;
        }

        public async Task RegisterAsync(string email, string password, string name)
        {
            var user = await this._userRepository.GetAsync(email);
            if (user != null)
                throw new ActioException("email_in_use", 
                    $"Email: '{email}' is already in use.");
            
            user = new User(email, name);
            user.SetPassword(password, this._encrypter);
            await this._userRepository.AddAsync(user);
        }

        public async Task<JsonWebToken> LoginAsync(string email, string password)
        {
            var user = await this._userRepository.GetAsync(email);
            if (user == null)
                throw new ActioException("invalid_credentials", 
                    $"Invalid credentials.");
            if (!user.ValidatePassword(password, this._encrypter))
                throw new ActioException("invalid_credentials", 
                    $"Invalid credentials.");

            return this._jwtHandler.Create(user.Id);
        }
    }
}