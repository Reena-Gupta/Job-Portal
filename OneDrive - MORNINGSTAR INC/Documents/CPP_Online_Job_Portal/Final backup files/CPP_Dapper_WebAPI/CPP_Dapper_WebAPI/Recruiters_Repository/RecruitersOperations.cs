using CPP_Dapper_WebAPI.Models;
using Dapper;
using Npgsql;
using System.Data;

namespace CPP_Dapper_WebAPI.Recruiters_Repository
{
    public class RecruitersOperations : IRecruiters
    {
        private readonly IConfiguration _config;
        NpgsqlConnection _conn;

        public RecruitersOperations(IConfiguration config)
        {
            _config = config;
            string connStr = _config.GetConnectionString("Pconnstr");
            _conn = new NpgsqlConnection(connStr);
        }
        public async Task<Recruiters> GetRecruitersAsync(string recruitemail)
        {
            try
            {
                await _conn.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("p_email", recruitemail, DbType.String);

                // Ensure you use CommandType.Text to call a function
                var result = await _conn.QueryFirstOrDefaultAsync<Recruiters>(
                    "SELECT * FROM get_recruiter_by_email(@p_email)",
                    parameters,
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
        public async Task<bool> InsertRecruitersAsync(Recruiters recruit)
        {
            try
            {
                await _conn.OpenAsync();
                await _conn.ExecuteAsync("insert_recruiter", new { p_company_name = recruit.company_name, p_location = recruit.location, p_contact_number = recruit.contact_number, p_email = recruit.email, p_password = recruit.password}, null, null, CommandType.StoredProcedure);
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
        public async Task<bool> UpdateRecruitersAsync(string recruitemail, Recruiters recruit)
        {
            {
                try
                {
                    await _conn.OpenAsync();
                    await _conn.ExecuteAsync("update_recruiters", new { p_email = recruitemail, p_company_name = recruit.company_name, p_location = recruit.location, p_contact_number = recruit.contact_number, p_password = recruit.password}, null, null, CommandType.StoredProcedure);
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
        public async Task<bool> DeleteRecruitersAsync(string recruitemail)
        {
            try
            {
                await _conn.OpenAsync();
                await _conn.ExecuteAsync("delete_recruiter_by_email", new { p_email = recruitemail }, null, null, CommandType.StoredProcedure);
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

        public async Task<bool> RegisterRecruiters(Recruiters recruit)
        {
            try
            {
                await _conn.OpenAsync();

                await _conn.ExecuteAsync(
                    "Registerrecruiter",
                    new
                    {
                        p_company_name = recruit.company_name,
                        p_password = recruit.password,
                        p_email = recruit.email,
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
        public async Task<IEnumerable<Recruiters>> getallrecruiters()
        {
            try
            {
                await _conn.OpenAsync();

                // Ensure you use CommandType.Text to call a function
                var result = await _conn.QueryAsync<Recruiters>(
                    "SELECT * FROM getallrecruiter()",
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
