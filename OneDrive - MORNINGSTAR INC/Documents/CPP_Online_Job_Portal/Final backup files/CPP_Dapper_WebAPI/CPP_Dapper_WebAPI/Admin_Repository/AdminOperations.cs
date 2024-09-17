using CPP_Dapper_WebAPI.Models;
using Dapper;
using Npgsql;
using System.Data;

namespace CPP_Dapper_WebAPI.Admin_Repository
{
    public class AdminOperations : IAdmin
    {
        private readonly IConfiguration _config;
        NpgsqlConnection _conn;

        public AdminOperations(IConfiguration config)
        {
            _config = config;
            string connStr = _config.GetConnectionString("Pconnstr");
            _conn = new NpgsqlConnection(connStr);
        }
        public async Task<Admin> GetAdminAsync(int Admin_Id)
        {
            try
            {
                await _conn.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("p_admin_id", Admin_Id, DbType.Int32);

                // Ensure you use CommandType.Text to call a function
                var result = await _conn.QueryFirstOrDefaultAsync<Admin>(
                    "SELECT * FROM get_username_by_admin_id(@p_admin_id)",
                    parameters,
                    commandType: CommandType.Text
                );

                return result;
            }
            catch (Exception ex)
            {
                // Log or handle exception
                throw new Exception("Error fetching Admin Details", ex);
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }
        public async Task<bool> InsertAdminAsync(Admin owner)
        {
            try
            {
                await _conn.OpenAsync();
                await _conn.ExecuteAsync("insert_admin", new { p_username = owner.Username, p_password = owner.Password }, null, null, CommandType.StoredProcedure);
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

        public async Task<Admin> GetAdmin(string Username)
        {
            try
            {
                await _conn.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("p_Username", Username, DbType.String);

                // Ensure you use CommandType.Text to call a function
                var result = await _conn.QueryFirstOrDefaultAsync<Admin>(
                    "SELECT * FROM Admin where Username = @p_Username",
                    parameters,
                    commandType: CommandType.Text
                );

                return result;
            }
            catch (Exception ex)
            {
                // Log or handle exception
                throw new Exception("Error fetching Admin Details", ex);
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }
    }
}
