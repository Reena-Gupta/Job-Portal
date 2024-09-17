using CPP_Dapper_WebAPI.Models;

namespace CPP_Dapper_WebAPI.Admin_Repository
{
    public interface IAdmin
    {
        Task<Admin> GetAdminAsync(int adminId);
        Task<bool> InsertAdminAsync(Admin admin);
        Task<Admin> GetAdmin(string Username);
    }
}
