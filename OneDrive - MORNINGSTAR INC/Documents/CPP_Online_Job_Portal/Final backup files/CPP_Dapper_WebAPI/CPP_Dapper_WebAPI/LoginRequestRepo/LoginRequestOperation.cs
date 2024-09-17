using CPP_Dapper_WebAPI.Models;
using Dapper;
using JobPortalWebAPI.Models;
using Npgsql;
using System.Data;

namespace JobPortalWebAPI.LoginRequest
{
    public class LoginRequestOperation : ILoginRequest
    {
        private readonly IConfiguration _config;
        string constr;
        NpgsqlConnection _conn;
        NpgsqlCommand cmd;
        NpgsqlDataAdapter da;

        public LoginRequestOperation(IConfiguration _config)
        {
            _config = _config;
            constr = _config.GetConnectionString("Pconnstr");
            _conn = new NpgsqlConnection(constr);
        }
        public async Task<Recruiters> GetRecruitersAsync(string email)
        {
            try
            {
                await _conn.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("p_email", email, DbType.String);

                var result = await _conn.QueryFirstOrDefaultAsync<Recruiters>(
                    "SELECT * FROM Recruiters WHERE email = @p_email",
                    parameters,
                    commandType: CommandType.Text
                );

                return result;
            }
            catch (Exception ex)
            {
                // Handle or log exception
                throw new Exception("An error occurred while fetching the recruiters", ex);
            }
            finally
            {
                if (_conn.State == ConnectionState.Open)
                {
                    await _conn.CloseAsync();
                }
            }
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

        //public async Task<Recruiters> GetAdminAsync(string Username)
        //{
        //    try
        //    {
        //        await _conn.OpenAsync();
        //        var parameters = new DynamicParameters();
        //        parameters.Add("p_username", Username, DbType.String);

        //        var result = await _conn.QueryFirstOrDefaultAsync<Recruiters>(
        //            "SELECT * FROM admin WHERE Username = @p_em",
        //            parameters,
        //            commandType: CommandType.Text
        //        );

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle or log exception
        //        throw new Exception("An error occurred while fetching the recruiters", ex);
        //    }
        //    finally
        //    {
        //        if (_conn.State == ConnectionState.Open)
        //        {
        //            await _conn.CloseAsync();
        //        }
        //    }
        //}
    }
}
