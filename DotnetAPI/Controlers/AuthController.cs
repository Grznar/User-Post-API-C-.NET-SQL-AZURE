using System.Data;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _config = config;
        }
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

                    byte[] passwordHash = GetPassowordHash(userForRegistration.Password,passwordSalt);


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
                        return Ok();
                    }
                    throw new Exception("Failed to Register user");
                }
                throw new Exception("User already exists");


            }
            throw new Exception("Password do not match");
        }
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

            byte[] passwordHash=GetPassowordHash(userForLogin.Password,userForLoginConfirmationDto.PasswordSalt);
            
            for(int index=0;index<passwordHash.Length;index++)
            {
                if(passwordHash[index]!=userForLoginConfirmationDto.PasswordHash[index])
                {
                return StatusCode(401,"Incorect password!");
                }
            }
                        return Ok();
        }
        private byte[] GetPassowordHash(string password,byte[]passwordSalt)
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
    }






}