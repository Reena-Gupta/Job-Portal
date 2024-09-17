using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace JobPortalWebAPI.Models
{
    public class JobApplication
    {      
       // public int application_id { get; set; } 
        public string js_email { get; set; } 
        public int job_id { get; set; } 
        public DateTime application_date { get; set; } = DateTime.Now;
        public string status { get; set; } 
        public string remail { get; set; }
        public string js_resumelink { get; set; }
        public string jp_title { get; set; }
        public string jp_description { get; set; }
        public string r_compname { get; set; }

    }
}
