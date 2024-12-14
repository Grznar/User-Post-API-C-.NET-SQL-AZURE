using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
            _authHelper=new AuthHelper(config);
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

                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rnd = RandomNumberGenerator.Create())
                    {
                        rnd.GetNonZeroBytes(passwordSalt);
                    }

                    string passwordSaltPlusString =
                    _config.GetSection("AppSettings:PasswordKey").Value
                     + Convert.ToBase64String(passwordSalt);

                    byte[] passwordHash = _authHelper.GetPassowordHash(userForRegistration.Password, passwordSalt);


                    string sqlAddAuth = @"INSERT INTO  TutorialAppSchema.Auth
                            (
                            [Email],
                        [PasswordHash],
                        [PasswordSalt])
                        VALUES(
                        '" + userForRegistration.Email +
                        "' ,@PasswordHash,@PasswordSalt)";

                    List<SqlParameter> sqlParameters = new List<SqlParameter>();
                    SqlParameter passwordSaltParametr = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                    passwordSaltParametr.Value = passwordSalt;
                    SqlParameter passwordHashParametr = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                    passwordHashParametr.Value = passwordHash;

                    sqlParameters.Add(passwordSaltParametr);
                    sqlParameters.Add(passwordHashParametr);
                    if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
                    {
                        string sqlAddUser = @"
         INSERT INTO TutorialAppSchema.Users(
    [FirstName]
    ,[LastName]
    ,[Email]
    ,[Gender]
    ,[Active]   
)
VALUES(" +
"'" + userForRegistration.FirstName +
"','" + userForRegistration.LastName +
"','" + userForRegistration.Email +
"','" + userForRegistration.Gender +
"', 1)";

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
        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLogin userForLogin)
        {
            string sqlForHasAndSalt = @"SELECT
            [PasswordHash],
            [PasswordSalt]
            FROM TutorialAppSchema.Auth
            WHERE Email='" + userForLogin.Email + "'";
            UserForLoginConfirmationDto
            userForLoginConfirmationDto =
            _dapper.LoadDataSingle
            <UserForLoginConfirmationDto>
            (sqlForHasAndSalt);

            byte[] passwordHash = _authHelper.GetPassowordHash(userForLogin.Password, userForLoginConfirmationDto.PasswordSalt);

            for (int index = 0; index < passwordHash.Length; index++)
            {
                if (passwordHash[index] != userForLoginConfirmationDto.PasswordHash[index])
                {
                    return StatusCode(401, "Incorect password!");
                }
            }
            string userIdSql = @"SELECT UserId FROM TutorialAppSchema.Users WHERE Email = '"+
            userForLogin.Email + "'";
            int userId = _dapper.LoadDataSingle<int>(userIdSql);
            return Ok(new Dictionary<string ,string>
            {
            {"token",_authHelper.   CreateToken(userId)}
            });
        }
        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
        string userId = User.FindFirst("userId")?.Value + "";

        string userIdSql= @"SELECT UserId FROM TutorialAppSchema.Users
         WHERE UserId = " + userId;
         int userIdFromDB = _dapper.LoadDataSingle<int>(userIdSql);



        return Ok(new Dictionary<string ,string>
            {
            {"token",_authHelper.CreateToken(userIdFromDB)}
            });
        }
        
    }






}