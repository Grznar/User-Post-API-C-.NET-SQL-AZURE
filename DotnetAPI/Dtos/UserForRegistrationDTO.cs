using System.ComponentModel.DataAnnotations;

namespace DotnetAPI.Dtos
{
    public partial class UserForRegistration
    {
        public string Email{get;set;}
        public string Password{get;set;}
        public string PasswordConfirm{get;set;}

        public UserForRegistration()
        {
        if(Email==null) Email="";
        if(Password==null) Password="";
        if(PasswordConfirm==null) PasswordConfirm="";
        }
    }
}