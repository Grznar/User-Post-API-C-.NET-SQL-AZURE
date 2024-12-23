
using System.ComponentModel.DataAnnotations;
using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
    DataContextDapper _dapper;
    public UserCompleteController(IConfiguration config)
    {
        Console.WriteLine(config.GetConnectionString("DefaultConnection"));
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }
    [HttpGet("GetUsers/{userId}/{isActive}")]
    public IEnumerable<UserComplete> GetUsers(int userId, bool isActive )
    {
        
        string sql = @"
        EXEC TutorialAppSchema.spUsers_Get";
        string parametres = "";
        DynamicParameters sqlParametres = new DynamicParameters();

        if(userId!=0)
        {
        parametres+=", @UserId = @UserIdParameter";
        sqlParametres.Add("@UserIdParameter",userId,DbType.Int32);
        } 
        if(isActive)
        {
        parametres+=", @Active = @ActiveParameter";
        sqlParametres.Add("@ActiveParameter",isActive,DbType.Boolean);
        }
        if(parametres.Length>0)sql += parametres.Substring(1);

        IEnumerable<UserComplete> users = _dapper.LoadDataWithParametres<UserComplete>(sql,sqlParametres);
        return users;

    }
    [HttpPut("EditUser")]
    public IActionResult UpsertUser(UserComplete user)
    {

        string sql = @"EXEC TutorialAppSchema.spUser_Upsert
             @FirstName = @FirstNameParam 
            ,@LastName= @LastNameParam
            ,@Email =   @EmailParam 
            ,@Gender =  @GenderParam
            ,@Active=   @ActiveParam
            ,@JobTitle=   @ActiveParam
            ,@Department=   @DepartmentParam
            ,@Salary=   @SalaryParam
             @UserId=   @UserIdParam";

        DynamicParameters sqlParameters = new DynamicParameters();
        sqlParameters.Add("@FirstNameParam",user.FirstName,DbType.String);
        sqlParameters.Add("@LastNameParam",user.LastName,DbType.String);
        sqlParameters.Add("@EmailParam",user.Email,DbType.String);
        sqlParameters.Add("@GenderParam",user.Gender,DbType.String);
        sqlParameters.Add("@ActiveParam",user.Active,DbType.Boolean);
        sqlParameters.Add("@ActiveParam",user.JobTitle,DbType.String);
        sqlParameters.Add("@DepartmentParam",user.Department,DbType.String);
        sqlParameters.Add("@SalaryParam",user.Salary,DbType.Decimal);
        sqlParameters.Add("@UserIdParam",user.UserID,DbType.Int32);
        Console.WriteLine(sql);
        if (_dapper.ExecuteSqlWithParameters(sql,sqlParameters))
        {
            return Ok();
        }
        throw new Exception("Failed to Update User");

    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {DynamicParameters sqlParameters = new DynamicParameters();
        string sql = @"EXEC TutorialAppSchema.spUser_Delete
        @UserId = @UserIdParam";
         sqlParameters.Add("@UserIdParam",userId,DbType.Int32);
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed to delete User");

    }
}


