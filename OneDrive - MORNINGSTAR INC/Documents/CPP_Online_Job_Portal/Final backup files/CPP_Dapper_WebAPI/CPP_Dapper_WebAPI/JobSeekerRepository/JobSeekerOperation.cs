using JobPortalWebAPI.Models;
using System.Data;
using Npgsql;
using System.Data.Common;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using CPP_Dapper_WebAPI.Models;

namespace JobPortalWebAPI.JobSeekerRepo
{
    public class JobSeekerOperation : IJobSeeker
    {
        private readonly IConfiguration _config;
        string constr;
        NpgsqlConnection _conn;
        NpgsqlCommand cmd;
        NpgsqlDataAdapter da;

        public JobSeekerOperation(IConfiguration _config)
        {
            _config = _config;
            constr = _config.GetConnectionString("Pconnstr");
            _conn = new NpgsqlConnection(constr);
        }


        public async Task<Jobseeker> GetJobSeekerAsync(string email)
        {
            try
            {
                await _conn.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("p_email", email, DbType.String);

                var result = await _conn.QueryFirstOrDefaultAsync<Jobseeker>(
                    "SELECT * FROM Jobseeker WHERE email = @p_email",
                    parameters,
                    commandType: CommandType.Text
                );

                return result;
            }
            catch (Exception ex)
            {
                // Handle or log exception
                throw new Exception("An error occurred while fetching the jobseeker", ex);
            }
            finally
            {
                if (_conn.State == ConnectionState.Open)
                {
                    await _conn.CloseAsync();
                }
            }
        }


        public async Task<Jobseeker> GetJobSeekerByEmailAsync(string email)
        {
            try
            {
                await _conn.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("p_email", email);

                var result = await _conn.QueryFirstOrDefaultAsync<Jobseeker>(
                    "SELECT * FROM getJobseekerByEmailId(@p_email)",
                    parameters,
                    commandType: CommandType.Text
                );

                return result;
            }
            catch (Exception ex)
            {
                // Log or handle exception
                throw new Exception("Error fetching job seeker by Email Id", ex);
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }



        public async Task<bool> RegisterJobseeker(Jobseeker jobseeker)
        {
            try
            {
                await _conn.OpenAsync();

                await _conn.ExecuteAsync(
                    "registerJobseeker",
                    new
                    {
                        p_name = jobseeker.name,
                        p_pass = jobseeker.password,
                        p_email = jobseeker.email,                   
                        p_desig = "undefined",
                       
                    },
                    commandType: CommandType.StoredProcedure
                );

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing stored procedure: {ex.Message}");
                throw; // Re-throw the exception to be handled by the calling code
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }


        public async Task<bool> InsertJobseekerAsync(Jobseeker jobseeker)
        {
            try
            {
                await _conn.OpenAsync();

                // Execute the stored procedure with the parameters from the Jobseeker object
                await _conn.ExecuteAsync(
                    "insertJobseeker",
                    new
                    {
                        p_name = jobseeker.name,
                        p_email = jobseeker.email,
                        p_phone = jobseeker.phone_number,
                        p_addr = jobseeker.address,
                        p_desig = jobseeker.designation,
                        p_edu = jobseeker.education_bg,
                        p_work = jobseeker.work_experience,
                        p_skills = jobseeker.skills,
                        p_resume = jobseeker.resume_link
                    },
                    commandType: CommandType.StoredProcedure
                );

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing stored procedure: {ex.Message}");
                throw; // Re-throw the exception to be handled by the calling code
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }

       public async Task<bool> UpdateJobseekerByEmail(Jobseeker jobseeker) 
        { 
            try
            {
                await _conn.OpenAsync();

                var result = await _conn.ExecuteAsync(
                    "updateJobseekerByEmail",
                    new
                    {
                        p_name = jobseeker.name,
                        p_email = jobseeker.email,
                        p_phone = jobseeker.phone_number,
                        p_addr = jobseeker.address,
                        p_desig = jobseeker.designation,
                        p_edu = jobseeker.education_bg,
                        p_work = jobseeker.work_experience,
                        p_skills = jobseeker.skills,
                        p_resume = jobseeker.resume_link
                    },
                    commandType: CommandType.StoredProcedure
                );

                return true;
            }
            catch (NpgsqlException ex)
            {
                // Handle database-specific exceptions
                throw new ApplicationException("Database operation failed.", ex);
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }

        public async Task<bool> DeleteJobseekerByEmailAsync(string email)
        {
            try
            {
                await _conn.OpenAsync();

                // Execute the stored procedure to delete a jobseeker
                await _conn.ExecuteAsync(
                    "deleteJobseekerByEmail",
                    new
                    {
                        p_email = email
                    },
                    commandType: CommandType.StoredProcedure
                );

                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error deleting jobseeker", ex);
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }
        public async Task<IEnumerable<Jobseeker>> getalljs()
        {
            try
            {
                await _conn.OpenAsync();

                // Ensure you use CommandType.Text to call a function
                var result = await _conn.QueryAsync<Jobseeker>(
                    "SELECT * FROM getallJobseekerById()",
                    commandType: CommandType.Text
                );

                return result;
            }
            catch (Exception ex)
            {
                // Log or handle exception
                throw new Exception("Error fetching recruiters", ex);
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }
    }
}
