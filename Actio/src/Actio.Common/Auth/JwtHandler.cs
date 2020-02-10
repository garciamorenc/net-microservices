using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Actio.Common.Auth
{
    public class JwtHandler : IJwtHandler
    {
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        private readonly JwtOptions _options;
        private readonly SecurityKey _issuerSigninKey;
        private readonly SigningCredentials _signingCredentials;
        private readonly JwtHeader _jwtHeader;
        private readonly TokenValidationParameters _tokenValidationParameters;


        public JwtHandler(IOptions<JwtOptions> options)
        {
            this._options = options.Value;
            this._issuerSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._options.SecretKey));
            this._signingCredentials = new SigningCredentials(this._issuerSigninKey, SecurityAlgorithms.HmacSha256);
            this._jwtHeader = new JwtHeader(this._signingCredentials);
            this._tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidIssuer = this._options.Issuer,
                IssuerSigningKey = this._issuerSigninKey
            };
        }

        public JsonWebToken Create(Guid userId)
        {
            var nowUtc = DateTime.UtcNow;
            var expires = nowUtc.AddMinutes(this._options.ExpiryMinutes);
            var centuryBegin = new DateTime(1970, 1, 1).ToUniversalTime();
            var exp = (long)(new TimeSpan(expires.Ticks - centuryBegin.Ticks).TotalSeconds);
            var now = (long)(new TimeSpan(nowUtc.Ticks - centuryBegin.Ticks).TotalSeconds);
            var payload = new JwtPayload
            {
                {"sub", userId},
                {"issuer", this._options.Issuer},
                {"iat", now},
                {"exp", exp},
                {"unique_name", userId}
            };
            var jwt = new JwtSecurityToken(this._jwtHeader, payload);
            var token = this._jwtSecurityTokenHandler.WriteToken(jwt);

            return new JsonWebToken
            {
                Token = token,
                Expires = exp
            };
        }
    }
}