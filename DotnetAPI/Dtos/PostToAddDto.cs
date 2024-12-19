namespace DotnetAPI.Dtos
{
    public partial class PostToaddDto
    {

        
        
        public string PostTitle{get;set;}
        public string PostContent{get;set;}
        
        
        public PostToaddDto()
        {
            if(PostTitle==null) PostTitle="";
            if(PostContent==null) PostContent="";
        }
    }
}