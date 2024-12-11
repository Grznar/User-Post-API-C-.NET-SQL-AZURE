
using System.Collections;
using AutoMapper;
using Dotnet.API.Data;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{
    
    IUserRepository _userRepository;
    IMapper _mapper;
    IMapper _mapperJobInfo;
    public UserEFController(IConfiguration config,IUserRepository userRepository)
    {
        _userRepository= userRepository;
        
        
        
        _mapper = new Mapper(new MapperConfiguration(cfg => {
            cfg.CreateMap<UserToAddDTO,User>();
        }));
        _mapperJobInfo = new Mapper(new MapperConfiguration(cfg => {
            cfg.CreateMap<UserJobInfoDTO,UserJobInfo>();
    }));
    }

    
    [HttpGet("GetUsers")]
    // public IActionResult Test()  - API RESPONSE
    public IEnumerable<User> GetUsers()
    {

        IEnumerable<User> users=_userRepository.GetUsers();
        return users;

    }
    // [HttpGet("GetUserJobInfo")]
    // public IEnumerable<UserJobInfo> GetUserJobInfo()
    // {
    //     IEnumerable<UserJobInfo> users=_entityFramework.UserJobInfo.ToList<UserJobInfo>();
    //     return users;
    // }
    [HttpGet("GetSingleUser/{userId}")]
    public User  GetSingleUser(int userId)
    {
        return  _userRepository.GetSingleUser(userId);
    }
    [HttpGet("GetSingleUserSalary/{userId}")]
    public UserSalary  GetSingleUserSalary(int userId)
    {
        return  _userRepository.GetSingleUserSalary(userId);
    }
    // [HttpGet("GetSingleUserJobInfo/{userId}")]
    // public UserJobInfo GetSingleJobInfo(int userId)
    // {
    // UserJobInfo? userJobInfo = _entityFramework.UserJobInfo
    // .Where(u=>u.UserID==userId)
    // .FirstOrDefault<UserJobInfo>();
    // if(userJobInfo!=null)
    // {
    // return userJobInfo;
    // }
    // throw new Exception("Failed to get single user job info ");
    // }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        User? userDb = _userRepository.GetSingleUser(user.UserID);
        

        if(userDb!=null)
        {
        userDb.Active=user.Active;
        userDb.FirstName=user.FirstName;
        userDb.LastName=user.LastName;
        userDb.Email=user.Email;
        userDb.Gender=user.Gender;
        
        if (_userRepository.SaveChanges())
        {
            return Ok();
        }
        }
        throw new Exception("Failed to Edit User");
    }
    [HttpPut("EditUserSalary")]
    public IActionResult EditUserSalary(UserSalary user)
    {
        UserSalary? userDb = _userRepository.GetSingleUserSalary(user.UserID);
        

        if(userDb!=null)
        {
        _mapper.Map(userDb,user);
        
        
        
        if (_userRepository.SaveChanges())
        {
            return Ok();
        }
        }
        throw new Exception("Failed to Edit User");
    }
    [HttpPut("EditUserJobInfo")]
    public IActionResult EditUserSJobInfo(UserJobInfo user)
    {
        UserJobInfo? userDb = _userRepository.GetSingleUserJobInfo(user.UserID);
        

        if(userDb!=null)
        {
        _mapper.Map(userDb,user);
        
        
        
        if (_userRepository.SaveChanges())
        {
            return Ok();
        }
        }
        throw new Exception("Failed to Edit User");
    }
    //  [HttpPut("EditUserJobInfo")]
    //  public IActionResult EditUserJobInfo(UserJobInfo userJobInfo)
    //  {
    //      UserJobInfo? userJobInfoU=_entityFramework.UserJobInfo
    //      .Where(u=>u.UserID==userJobInfo.UserID)
    //      .FirstOrDefault();
    //     if(userJobInfoU!=null)
    //     {
    //     userJobInfoU.JobTitle=userJobInfo.JobTitle;
    //     userJobInfoU.Department=userJobInfo.Department;
        
        
    //     if (_userRepository.SaveChanges())
    //     {
    //         return Ok();
    //     }
    //     }
    //     throw new Exception("Failed to Edit User Job Info");
         
    //  }
    
     




    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDTO user)
    {
        User userDb = _mapper.Map<User>(user);
        
        _userRepository.AddEntity<User>(userDb);
        
        if (_userRepository.SaveChanges())
        {
            return Ok();
        }
        
        throw new Exception("Failed to Add User");

    }
    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        User? userDb = _userRepository.GetSingleUser(userId);
        

        if(userDb!=null)
        {
        _userRepository.RemoveEntity<User>(userDb);
        
        
        if (_userRepository.SaveChanges())
        {
            return Ok();
        }
        }
        throw new Exception("Failed to Edit User");

    }
    [HttpDelete("DeleteUserJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfo(int userId)
    {
        UserJobInfo? userDb = _userRepository.GetSingleUserJobInfo(userId);
        

        if(userDb!=null)
        {
        _userRepository.RemoveEntity<UserJobInfo>(userDb);
        
        
        if (_userRepository.SaveChanges())
        {
            return Ok();
        }
        }
        throw new Exception("Failed to Edit User");

    }
    [HttpDelete("DeleteUserSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {
        UserSalary? userDb = _userRepository.GetSingleUserSalary(userId);
        

        if(userDb!=null)
        {
        _userRepository.RemoveEntity<UserSalary>(userDb);
        
        
        if (_userRepository.SaveChanges())
        {
            return Ok();
        }
        }
        throw new Exception("Failed to Edit User");

    }


}


