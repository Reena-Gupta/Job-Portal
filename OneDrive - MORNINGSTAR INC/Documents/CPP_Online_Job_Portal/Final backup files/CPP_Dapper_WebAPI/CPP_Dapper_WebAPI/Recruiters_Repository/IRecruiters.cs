using CPP_Dapper_WebAPI.Models;

namespace CPP_Dapper_WebAPI.Recruiters_Repository
{
    public interface IRecruiters
    {
        Task<Recruiters> GetRecruitersAsync(string recruitemail);
        Task<bool> InsertRecruitersAsync(Recruiters recruit);
        Task<bool> UpdateRecruitersAsync(string recruitemail, Recruiters recruit);
        Task<bool> DeleteRecruitersAsync(string recruitemail);
        Task<bool> RegisterRecruiters(Recruiters recruit);
        Task<IEnumerable<Recruiters>> getallrecruiters();
    }
}
