using System.Data;
using System.Data.Common;
using System.Net;
using System.Numerics;
using CPP_Dapper_WebAPI.Models;
using Dapper;
using Npgsql;

namespace CPP_Dapper_WebAPI.JobPosting_Repository
{
    public class JobPostingsOperations : IJobPostings
    {
        private readonly IConfiguration _config;
        NpgsqlConnection _conn;
        NpgsqlCommand cmd;

        public JobPostingsOperations(IConfiguration config)
        {
            _config = config;
            string connStr = _config.GetConnectionString("Pconnstr");
            _conn = new NpgsqlConnection(connStr);
        }
        public async Task<IEnumerable<JobPostings>> GetJobPostingsAsync(string email)
        {
            try
            {
                await _conn.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("p_remail", email, DbType.String);

                // Ensure you use CommandType.Text to call a function
                var result = await _conn.QueryAsync<JobPostings>(
                    "SELECT * FROM get_job_posting_by_remail(@p_remail)",
                    parameters,
                    commandType: CommandType.Text
                );

                return result;
            }
            catch (Exception ex)
            {
                // Log or handle exception
                throw new Exception("Error fetching job postings", ex);
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }
        public async Task<bool> InsertJobPostingAsync(JobPostings jobPosting)
        {
            try
            {
                await _conn.OpenAsync();
                await _conn.ExecuteAsync("insert_job_posting", new { p_job_title = jobPosting.job_title, p_job_location = jobPosting.job_location, p_contact_person = jobPosting.contact_person, p_contact_number = jobPosting.contact_number, p_functional_skills = jobPosting.functional_skills, p_technical_skills = jobPosting.technical_skills, p_job_description = jobPosting.job_description, p_job_type = jobPosting.job_type, p_last_date = jobPosting.last_date, p_salary = jobPosting.salary, p_remail = jobPosting.remail, p_jp_companyname = jobPosting.jp_companyname }, null, null, CommandType.StoredProcedure);
                return true;

            }
            catch (Exception ex)
            {
                // Log exception details
                throw new Exception($"Error executing stored procedure: {ex.Message}", ex);
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }
        public async Task<bool> UpdateJobPostingAsync(string recruitemail, string job_title, JobPostings jobPosting)
        {
            {
                try
                {
                    await _conn.OpenAsync();
                    await _conn.ExecuteAsync("update_job_posting", new { p_remail = recruitemail, p_job_title = job_title, p_job_location = jobPosting.job_location, p_contact_person = jobPosting.contact_person, p_contact_number = jobPosting.contact_number, p_functional_skills = jobPosting.functional_skills, p_technical_skills = jobPosting.technical_skills, p_job_description = jobPosting.job_description, p_job_type = jobPosting.job_type, p_last_date = jobPosting.last_date, p_salary = jobPosting.salary }, null, null, CommandType.StoredProcedure);
                    return true;
                }
                catch (NpgsqlException ex)
                {
                    throw new ApplicationException("Database operation failed.", ex);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("An unexpected error occurred.", ex);
                }
                finally
                {
                    await _conn.CloseAsync();
                }
            }
        }
        public async Task<bool> DeleteJobPostingAsync(string job_title, string recruitemail)
        {
            try
            {
                await _conn.OpenAsync();
                await _conn.ExecuteAsync("delete_job_posting", new { p_job_title = job_title, p_remail = recruitemail }, null, null, CommandType.StoredProcedure);
                return true;
            }
            catch (Exception ex)
            {
                // Handle exceptions such as non-existent records or database issues
                throw new Exception("Error deleting job posting", ex);
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }
        public async Task<IEnumerable<JobPostings>> getalljobpostings()
        {
            try
            {
                await _conn.OpenAsync();

                // Ensure you use CommandType.Text to call a function
                var result = await _conn.QueryAsync<JobPostings>(
                    "SELECT * FROM getalljobposting()",
                    commandType: CommandType.Text
                );

                return result;
            }
            catch (Exception ex)
            {
                // Log or handle exception
                throw new Exception("Error fetching job postings", ex);
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }

        public async Task<IEnumerable<JobPostings>> GetAllJobPostingsAsync()
        {
            try
            {
                await _conn.OpenAsync();

                var query = @"
                                SELECT 
                                job_title, 
                                job_location,  
                                functional_skills,
                                technical_skills,
                                job_description,
                                job_type,
                                post_date,
                                last_date,
                                salary,  
                                jp_companyname
                            FROM 
                                JobPostings";

                var result = await _conn.QueryAsync<JobPostings>(query);

                return result;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("Error fetching all job postings", ex);
            }
            finally
            {
                await _conn.CloseAsync(); // Ensure the connection is closed
            }
        }
        //public async Task<bool> AInsertJobPosting(JobPostings jobPosting)
        //{
        //    try
        //    {
        //        await _conn.OpenAsync();
        //        await _conn.ExecuteAsync("insert_job_posting", new { p_job_title = jobPosting.job_title, p_job_location = jobPosting.job_location, p_contact_person = jobPosting.contact_person, p_contact_number = jobPosting.contact_number, p_functional_skills = jobPosting.functional_skills, p_technical_skills = jobPosting.technical_skills, p_job_description = jobPosting.job_description, p_job_type = jobPosting.job_type, p_last_date = jobPosting.last_date, p_salary = jobPosting.salary, p_remail = jobPosting.remail, p_jp_companyname = jobPosting.jp_companyname }, null, null, CommandType.StoredProcedure);
        //        return true;

        //    }
        //    catch (Exception ex)
        //    {
        //        // Log exception details
        //        throw new Exception($"Error executing stored procedure: {ex.Message}", ex);
        //    }
        //    finally
        //    {
        //        await _conn.CloseAsync();
        //    }
        //}
    }
}

