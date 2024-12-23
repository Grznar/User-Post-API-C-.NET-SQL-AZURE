using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using DotnetAPI.Data;
using Dapper;
namespace Dotnet.API.Helpers
{
    public class AuthHelper
    {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;
        public AuthHelper(IConfiguration config)
        {   _dapper = new DataContextDapper(config);
            _config = config;
        }

        public byte[] GetPassowordHash(string password, byte[] passwordSalt)
        {
            string passwordSaltPlusString =
                       _config.GetSection("AppSettings:PasswordKey").Value
                        + Convert.ToBase64String(passwordSalt);

            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000,
                numBytesRequested: 256 / 8

            );

        }

        public string CreateToken(int userId)
        {
            Claim[] claims = new Claim[] {
                new Claim("userId",userId.ToString())
            };

            string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;

            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        tokenKeyString != null ? tokenKeyString : ""
                    )
                );

            SigningCredentials credentials = new SigningCredentials
            (tokenKey, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddDays(1)
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);
        }
        public bool SetPassword(UserForLogin userForSetPassword)
        {
            byte[] passwordSalt = new byte[128 / 8];
            using (RandomNumberGenerator rnd = RandomNumberGenerator.Create())
            {
                rnd.GetNonZeroBytes(passwordSalt);
            }

            string passwordSaltPlusString =
            _config.GetSection("AppSettings:PasswordKey").Value
             + Convert.ToBase64String(passwordSalt);

            byte[] passwordHash = GetPassowordHash(userForSetPassword.Password, passwordSalt);


            string sqlAddAuth = @"EXEC TutorialAppSchema.spRegistration_Upsert
                        @Email = @EmailParam , @PasswordHash = @PasswordHashParam, @PasswordSalt = @PasswordSaltParam";


            DynamicParameters sqlParameters = new DynamicParameters();
            
            sqlParameters.Add("@EmailParam",userForSetPassword.Email,DbType.String);
            sqlParameters.Add("@PasswordHashParam",userForSetPassword.Email,DbType.Binary);
            sqlParameters.Add("@PasswordSaltParam",userForSetPassword.Email,DbType.Binary);
            return (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters));
            
        }
    }
}
