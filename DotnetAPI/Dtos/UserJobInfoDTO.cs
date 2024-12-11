namespace DotnetAPI.Dtos
{
    public partial class UserJobInfoDTO
    {

        

        
        public string JobTitle{ get; set; }
        public string Department { get; set; }
   

        public UserJobInfoDTO()
        {
            if (JobTitle == null)
            {
                JobTitle = "";
            }
            if (Department == null)
            {
                Department = "";
            }
            
        }




    }
}