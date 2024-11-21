using WebAPIProject.Helper;
using WebAPIProject.Model;
using WebAPIProject.Repos.Models;

namespace WebAPIProject.Service
{
    public interface IUserRoleService
    {
        Task<APIResponse> AssignRolePermission(List<Rolepermission> _data);
        Task<List<Role>> GetAllRoles();
        Task<List<Menu>> GetAllMenus();
        Task<List<Appmenus>> GetAllMenubyrole(string userrole);
        Task<Menupermission> GetAllMenupermissionbyrole(string userrole, string menucode);
    }
}
