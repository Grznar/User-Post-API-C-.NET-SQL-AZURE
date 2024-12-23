using System.Data;
using System.Security.Cryptography.X509Certificates;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.VisualBasic;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public PostController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("Posts/{postId}/{userId}/{searchParam}")]
        public IEnumerable<Post> GetPosts(int postId = 0, int userId = 0, string searchParam = "None")
        {
            DynamicParameters sqlParameters = new DynamicParameters();
            string sql = @"EXEC TutorialAppSchema.spPost_Get";
            string parameters = "";



            if (userId != 0)
            {
                parameters += ", @UserId=@UserIdParameter";
                sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
            }
            if (postId != 0)
            {
                parameters += ", @PostId=@PostIdParametr";
                sqlParameters.Add("@PostIdParametr", postId, DbType.Int32);
            }
            if (searchParam.ToLower() != "None")
            {
                parameters += ", @SearchValue=@SearchParam";
                sqlParameters.Add("@SearchParam", searchParam, DbType.String);
            }

            if (parameters.Length > 0)
            {
                sql += parameters.Substring(1);//, parameters.Length);
            }

            IEnumerable<Post> posts = _dapper.LoadDataWithParametres<Post>(sql, sqlParameters);
            return posts;
        }
        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPots()
        {
             DynamicParameters sqlParameters = new DynamicParameters();
            
            string sql = @"EXEC TutorialAppSchema.spPost_Get
             @UserId = @ThisUserParam";

             sqlParameters.Add("@ThisUserParam", this.User.FindFirst("userId")?.Value, DbType.Int32);
            return _dapper.LoadData<Post>(sql);
        }
        [HttpPut("UpsertPost")]
        public IActionResult UpsertPost(Post postToUpsert)
        {

             string sql = @"EXEC TutorialAppSchema.spPost_Upsert
            @UserId = @UserIdParam,
            @PostTitle = @PostTitleParam,
            @PostContent = @PostContentParam,
            ";

        DynamicParameters sqlParameters = new DynamicParameters();

        sqlParameters.Add("@UserIdParam", postToUpsert.UserId, DbType.Int32);
        sqlParameters.Add("@PostTitleParam", postToUpsert.PostTitle, DbType.String);
        sqlParameters.Add("@PostContentParam", postToUpsert.PostContent, DbType.String);

        if(postToUpsert.PostId>0)
        {
            sql+= ",@PostId = @PostIdParam";
        sqlParameters.Add("@PostIdParam", postToUpsert.PostId, DbType.Int32);
        }
        

        if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
        {
            return Ok();
        } 

        throw new Exception("Failed to Update Post");
        }

        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql = @" EXEC TutorialAppSchema.spPost_Delete
                @PostId = @PostIdParam "+
            ", @UserId = @ThisUserParam" ;

            DynamicParameters sqlParameters = new DynamicParameters();
        sqlParameters.Add("@PostIdParam", postId, DbType.Int32);
        sqlParameters.Add("@ThisUserParam", this.User.FindFirst("userId")?.Value, DbType.Int32);
        if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
        {
            return Ok();
        } 

        throw new Exception("Failed to Delete User");

        }
    }
}
