using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Dapper;
using Dotnet.API.Helpers;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;
        private readonly AuthHelper _authHelper;
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
            _config = config;
        }
        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistration userForRegistration)
        {
            if (userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                string sqlCheckUserExists = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" + userForRegistration.Email + "'";
                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
                if (existingUsers.Count() == 0)
                {

                    UserForLogin userForSetPassword = new UserForLogin()
                    {
                        Email = userForRegistration.Email,
                        Password = userForRegistration.Password

                    };

                    if (_authHelper.SetPassword(userForSetPassword))
                    {
                        string sqlAddUser = @"EXEC TutorialAppSchema.spUser_Upsert
                        @FirstName = '" + userForRegistration.FirstName +
                        "',@LastName= '" + userForRegistration.LastName +
                        "',@Email = '" + userForRegistration.Email +
                        "',@Gender = '" + userForRegistration.Gender +
                        "',@Active= 1" +
                        ",@JobTitle= '" + userForRegistration.JobTitle +
                        "',@Department= '" + userForRegistration.Department +
                        "',@Salary= '" + userForRegistration.Salary + "'";
                        // string sqlAddUser = @"
                        //             INSERT INTO TutorialAppSchema.Users(
                        //             [FirstName]
                        //             ,[LastName]
                        //             ,[Email]
                        //             ,[Gender]
                        //             ,[Active]   
                        //         )
                        //         VALUES(" +
                        //         "'" + userForRegistration.FirstName +
                        //         "','" + userForRegistration.LastName +
                        //         "','" + userForRegistration.Email +
                        //         "','" + userForRegistration.Gender +
                        //         "', 1)";

                        if (_dapper.ExecuteSql(sqlAddUser))
                        {
                            return Ok();
                        }
                        throw new Exception("Failed to add user");
                    }
                    throw new Exception("Failed to Register user");
                }
                throw new Exception("User already exists");


            }
            throw new Exception("Password do not match");
        }
        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword(UserForLogin userForSetPassword)
        {
            if (_authHelper.SetPassword(userForSetPassword))
            {
                return Ok();
            }
            throw new Exception("Failed to update password ");
        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLogin userForLogin)
        {
            string sqlForHasAndSalt = @"EXEC TutorialAppSchema.spLoginConfirmation_Get
            @Email = @EmailParam";

            DynamicParameters sqlParameters = new DynamicParameters();
            //  SqlParameter emailParametr = new SqlParameter("@EmailParam", SqlDbType.VarChar);
            // emailParametr.Value = userForLogin.Email;
            // sqlParameters.Add(emailParametr);
            sqlParameters.Add("@EmailParam",userForLogin.Email,DbType.String);
            

            UserForLoginConfirmationDto
            userForLoginConfirmationDto =
            _dapper.LoadDataSingleWithParametres
            <UserForLoginConfirmationDto>
            (sqlForHasAndSalt,sqlParameters);

            byte[] passwordHash = _authHelper.GetPassowordHash(userForLogin.Password, userForLoginConfirmationDto.PasswordSalt);

            for (int index = 0; index < passwordHash.Length; index++)
            {
                if (passwordHash[index] != userForLoginConfirmationDto.PasswordHash[index])
                {
                    return StatusCode(401, "Incorect password!");
                }
            }
            string userIdSql = @"SELECT UserId FROM TutorialAppSchema.Users WHERE Email = '" +
            userForLogin.Email + "'";
            int userId = _dapper.LoadDataSingle<int>(userIdSql);
            return Ok(new Dictionary<string, string>
            {
            {"token",_authHelper.CreateToken(userId)}
            });
        }
        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
            string userId = User.FindFirst("userId")?.Value + "";

            string userIdSql = @"SELECT UserId FROM TutorialAppSchema.Users
         WHERE UserId = " + userId;
            int userIdFromDB = _dapper.LoadDataSingle<int>(userIdSql);



            return Ok(new Dictionary<string, string>
            {
            {"token",_authHelper.CreateToken(userIdFromDB)}
            });
        }

    }






}