using System.ComponentModel.DataAnnotations;

namespace DotnetAPI.Dtos
{
    public partial class UserForLogin
    {
        public string Email{get;set;}
        public string Password{get;set;}
        

        public UserForLogin()
        {
        if(Email==null) Email="";
        if(Password==null) Password="";
        
        }
    }
}