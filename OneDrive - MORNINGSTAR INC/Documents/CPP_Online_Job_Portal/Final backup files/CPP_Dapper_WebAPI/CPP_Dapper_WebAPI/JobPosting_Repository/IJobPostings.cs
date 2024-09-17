using CPP_Dapper_WebAPI.Models;

namespace CPP_Dapper_WebAPI.JobPosting_Repository
{
    public interface IJobPostings
    {
        Task<IEnumerable<JobPostings>> GetJobPostingsAsync(string email);
        Task<bool> InsertJobPostingAsync(JobPostings jobPosting);
        Task<bool> UpdateJobPostingAsync(string recruitemail, string job_title, JobPostings jobPosting);
        Task<bool> DeleteJobPostingAsync(string job_title, string recruitemail);
        Task<IEnumerable<JobPostings>> getalljobpostings();
        public Task<IEnumerable<JobPostings>> GetAllJobPostingsAsync();
    }
}

    