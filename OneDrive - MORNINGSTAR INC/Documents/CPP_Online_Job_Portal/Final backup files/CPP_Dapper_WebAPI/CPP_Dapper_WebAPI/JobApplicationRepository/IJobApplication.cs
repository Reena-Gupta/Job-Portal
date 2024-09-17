using JobPortalWebAPI.Models;

namespace JobPortalWebAPI.JobApplicationRepository
{
    public interface IJobApplication
    {

        public Task<IEnumerable<JobApplication>> GetJobApplicationByEmailAsync(string email);
        //public Task<JobApplication> GetJobApplicationByIdAsync(int applicationId);
        // public Task<int> AddApplicationAsync(int jobseekerId, int jobId, DateTime applicationDate, string status, int recruiterId);
        //public Task<int> AddApplicationAsync(JobApplication jobApp);

        // public Task<JobApplication> GetJobApplicationByIdAsync(int jobId);
        // public Task<int> AddApplicationAsync(int jobseekerId, int jobId, DateTime applicationDate, string status, int recruiterId);
        //public  Task<int> AddApplicationAsync(JobApplication jobApp);

        public Task<IEnumerable<JobApplication>> GetJobApplicationAsync(string js_email);
        Task<bool> UpdateJobapplicationAsync(string recruitemail, string job_title, string status);

    }
}
