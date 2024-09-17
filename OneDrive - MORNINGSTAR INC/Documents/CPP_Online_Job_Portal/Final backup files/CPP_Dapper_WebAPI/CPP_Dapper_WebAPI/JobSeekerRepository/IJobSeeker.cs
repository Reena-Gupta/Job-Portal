using JobPortalWebAPI.Models;

namespace JobPortalWebAPI.JobSeekerRepo
{
    public interface IJobSeeker
    {

        public Task<Jobseeker> GetJobSeekerAsync(string email);
        public Task<bool> InsertJobseekerAsync(Jobseeker jobseeker);
        //public Task<bool> UpdateJobseekerAsync(int jobseekerId, string email, string phoneNumber, string address, string designation, string educationBg, string workExperience, string skills, string resumeLink);

        public Task<bool> RegisterJobseeker(Jobseeker jobseeker);
        //public Task<bool> DeleteJobseekerAsync(int jobseekerId);
        public Task<Jobseeker> GetJobSeekerByEmailAsync(string email);
        public Task<bool> UpdateJobseekerByEmail(Jobseeker jobseeker);
        public Task<bool> DeleteJobseekerByEmailAsync(string email);
        public Task<IEnumerable<Jobseeker>> getalljs();
    }
}
