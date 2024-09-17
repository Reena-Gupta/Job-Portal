using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobPortalWebAPI.Models
{
    public class Jobseeker
    {
     //   public int jobseeker_id { get; set; } 
     //   public int user_id { get; set; } 
        public string name { get; set; } 
        public string email { get; set; } 
        public string phone_number { get; set; } 
        public string address { get; set; } 
        public string designation { get; set; } 
        public string education_bg { get; set; } 
        public string work_experience { get; set; } 
        public string skills { get; set; } 
        public string resume_link { get; set; } 
        public DateTime created_at { get; set; } = DateTime.Now;

        public string password { get; set; }
    }


   

}
