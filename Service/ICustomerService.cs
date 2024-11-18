using WebAPIProject.Helper;
using WebAPIProject.Model;
using WebAPIProject.Repos.Models;

namespace WebAPIProject.Service
{
    public interface ICustomerService
    {
        Task<List<Customermodel>> Getall();
        Task<Customermodel> Getbycode(int code);
        Task<APIResponse> Remove(int code);
        Task<APIResponse> Create(Customermodel data);
        Task<APIResponse> Update(Customermodel data, int code);
    }
}
