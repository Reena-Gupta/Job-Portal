using CPP_Dapper_WebAPI.Models;
using JobPortalWebAPI.Models;

namespace JobPortalWebAPI.LoginRequest
{
    public interface ILoginRequest
    {
        public Task<Recruiters> GetRecruitersAsync(string email);
        //public Task<Recruiters> GetAdminAsync(string Username);
        public Task<Jobseeker> GetJobSeekerAsync(string email);
    }
}
