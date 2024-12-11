
using System.ComponentModel.DataAnnotations;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    DataContextDapper _dapper;
    public UserController(IConfiguration config)
    {
        Console.WriteLine(config.GetConnectionString("DefaultConnection"));
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }
    [HttpGet("GetUsers")]
    // public IActionResult Test()  - API RESPONSE
    public IEnumerable<User> GetUsers()
    {

        string sql = @"
        
        SELECT  [UserId]
        , [FirstName]
        , [LastName]
        , [Email]
        , [Gender]
        , [Active]
  FROM  TutorialAppSchema.Users";
        IEnumerable<User> users = _dapper.LoadData<User>(sql);
        return users;

    }
    [HttpGet("GetUserJobInfo")]
    // public IActionResult Test()  - API RESPONSE
    public IEnumerable<UserJobInfo> GetUserJobInfo()
    {

        string sql = @"
        
        SELECT  [UserId]
        , [JobTitle]
        , [Department]
  FROM  TutorialAppSchema.UserJobInfo";
        IEnumerable<UserJobInfo> users = _dapper.LoadData<UserJobInfo>(sql);
        return users;

    }
    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {

        string sql = @"
        
        SELECT  [UserId]
        , [FirstName]
        , [LastName]
        , [Email]
        , [Gender]
        , [Active]
  FROM  TutorialAppSchema.Users
  WHERE UserId=" + userId.ToString();
        User users = _dapper.LoadDataSingle<User>(sql);
        return users;
    }

    [HttpGet("GetSingleJobInfo/{userId}")]
    public UserJobInfo GetSingleUserJobInfo(int userId)
    {
        string sql = @"
        
        SELECT  [UserId]
        , [JobTitle]
        , [Department]
  FROM  TutorialAppSchema.UserJobInfo
  WHERE UserId=" + userId.ToString();

        UserJobInfo users = _dapper.LoadDataSingle<UserJobInfo>(sql);
        return users;

    }
    [HttpPut("EditUSer")]
    public IActionResult EditUser(User user)
    {
        string sql = @"
    UPDATE TutorialAppSchema.Users
    SET[FirstName] = '" + user.FirstName +
        "',[LastName] = '" + user.LastName +
        "',[Email] = '" + user.Email +
        "',[Gender] = '" + user.Gender +
        "',[Active]= '" + user.Active +
        "' WHERE UserId=" + user.UserID;
        Console.WriteLine(sql);
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed to Update User");

    }
    [HttpPut("EditUserJobInfo")]
    public IActionResult EditUserJobInfo(UserJobInfo user)
    {
        string sql = @"
    UPDATE TutorialAppSchema.UserJobInfo
    SET  [JobTitle]= '" + user.JobTitle +
        "',[Department]='" + user.Department +

        "' WHERE UserId=" + user.UserID;
        Console.WriteLine(sql);
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed to edit User");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDTO user)
    {
        string sql = @"
         INSERT INTO TutorialAppSchema.Users(
    [FirstName]
    ,[LastName]
    ,[Email]
    ,[Gender]
    ,[Active]
)
VALUES(" +
   "'" + user.FirstName +
   "','" + user.LastName +
   "','" + user.Email +
   "','" + user.Gender +
   "','" + user.Active +
   "')";


        Console.WriteLine(sql);
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed to Update User");

    }
    [HttpPost("AddUserJobInfo")]
    public IActionResult AddUserJobInfo(UserJobInfoDTO user)
    {

        string sql = @"
        INSERT INTO TutorialAppSchema.UserJobInfo
        (
        [JobTitle],
        [Department]
        )
        VALUES(" +
    "'" + user.JobTitle +
    "','" + user.Department +
    "')";
        Console.WriteLine(sql);
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed to Add User Job Info");
    }
    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"DELETE FROM TutorialAppSchema.Users
     WHERE UserId = " + userId.ToString();
        Console.WriteLine(sql);
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed to delete User");

    }
    [HttpDelete("DeleteJobInfo/{userId}")]
    public IActionResult DeleteJobInfo(int userId)
    {
        string sql = @"
    DELETE FROM TutorialAppSchema.UserJobInfo
    WHERE UserId = "+userId.ToString();
    Console.WriteLine(sql);
    if(_dapper.ExecuteSql(sql))
    {
    return Ok();
    }
    throw new Exception("Failed to delete User Job Info ");
    }


}


