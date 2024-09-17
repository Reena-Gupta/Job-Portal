using JobPortalWebAPI.Models;
using System.Data;
using Npgsql;
using Dapper;
using CPP_Dapper_WebAPI.Models;

namespace JobPortalWebAPI.JobApplicationRepository
{
    public class JobApplicationOperation : IJobApplication
    {
        private readonly IConfiguration _config;
        string constr;
        NpgsqlConnection _conn;
        NpgsqlCommand cmd;
        NpgsqlDataAdapter da;

        public JobApplicationOperation(IConfiguration _config)
        {
            _config = _config;
            constr = _config.GetConnectionString("Pconnstr");
            _conn = new NpgsqlConnection(constr);
        }



        public async Task<IEnumerable<JobApplication>> GetJobApplicationByEmailAsync(string email)
        {
            try
            {
                await _conn.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("seeker_email", email);

                // Execute the procedure
                await _conn.ExecuteAsync(
                    "CALL getJobApplicationsByEmail(@seeker_email)",
                    parameters,
                    commandType: CommandType.Text
                );

                // Query the temporary table created by the procedure
                var jobApplications = await _conn.QueryAsync<JobApplication>(
                    "SELECT * FROM temp_job_applications",
                    commandType: CommandType.Text
                );

                return jobApplications;
            }
            catch (Exception ex)
            {
                // Log or handle exception
                throw new Exception("Error fetching job applications by email", ex);
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }


        //public async Task<JobApplication> GetJobApplicationByIdAsync(int applicationId)
        //{
        //    JobApplication jobApplication = null;

        //    const string sql = "SELECT * FROM getJobApplicationById(@p_application_id)";

        //    try
        //    {
        //        await using (var cmd = new NpgsqlCommand(sql, conn))
        //        {
        //            cmd.CommandType = CommandType.Text;
        //            cmd.Parameters.AddWithValue("p_application_id", applicationId);

        //            await conn.OpenAsync();

        //            await using (var reader = await cmd.ExecuteReaderAsync())
        //            {
        //                if (await reader.ReadAsync())
        //                {
        //                    jobApplication = new JobApplication
        //                    {
        //                        application_id = reader.GetInt32(reader.GetOrdinal("application_id")),
        //                        jobseeker_id = reader.GetInt32(reader.GetOrdinal("jobseeker_id")),
        //                        job_id = reader.GetInt32(reader.GetOrdinal("job_id")),
        //                        application_date = reader.GetDateTime(reader.GetOrdinal("application_date")),
        //                        status = reader.GetString(reader.GetOrdinal("status")),
        //                        recruiter_id = reader.GetInt32(reader.GetOrdinal("recruiter_id"))
        //                    };
        //                }
        //            }
        //        }
        //    }
        //    catch (NpgsqlException ex)
        //    {

        //        Console.WriteLine($"Database error: {ex.Message}");

        //    }
        //    catch (Exception ex)
        //    { 
        //        Console.WriteLine($"An unexpected error occurred: {ex.Message}");

        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            await conn.CloseAsync();
        //        }
        //    }

        //    return jobApplication;
        //}

        //public async Task<int> AddApplicationAsync(JobApplication jobApp)
        //{
        //    int result = -1;

        //    try
        //    {
        //        await _conn.OpenAsync();

        //        cmd = new NpgsqlCommand("insertApplication", _conn);

        //        cmd.CommandType = CommandType.StoredProcedure;


        //        cmd.Parameters.AddWithValue("p_jsid", jobApp.jobseeker_id);
        //        cmd.Parameters.AddWithValue("p_jid", jobApp.job_id);
        //        cmd.Parameters.AddWithValue("p_appdate", jobApp.application_date);
        //        cmd.Parameters.AddWithValue("p_stat", jobApp.status);
        //        cmd.Parameters.AddWithValue("p_recrid", jobApp.recruiter_id);


        //        result = await cmd.ExecuteNonQueryAsync();
        //    }
        //    catch (NpgsqlException ex)
        //    {

        //        throw new ApplicationException("Database operation failed.", ex);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException("An unexpected error occurred.", ex);
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            await conn.CloseAsync();
        //        }
        //    }

        //    return result;
        //}

        public async Task<IEnumerable<JobApplication>> GetJobApplicationAsync(string js_email)
        {
            try
            {
                await _conn.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("p_js_email", js_email, DbType.String);

                // Ensure you use CommandType.Text to call a function
                var result = await _conn.QueryAsync<JobApplication>(
                    "SELECT * FROM getJobApplicationByremail(@p_js_email)",
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

        public async Task<bool> UpdateJobapplicationAsync(string recruitemail, string job_title, string status)
        {
            {
                try
                {
                    await _conn.OpenAsync();
                    await _conn.ExecuteAsync("UpdateJobApplicationStatus", new { p_remail = recruitemail, p_jp_title = job_title, p_status = status}, null, null, CommandType.StoredProcedure);
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
    }
}
