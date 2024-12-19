namespace DotnetAPI.Models
{
    public partial class UserSalary
    {

        public int UserID { get; set; }

       
        public decimal Salary { get; set; }
      
        public decimal AvgSalary{get;set;}

        public UserSalary()
        {
            
        }




    }
}