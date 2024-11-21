using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPIProject.Repos.Models;
using WebAPIProject.Service;

namespace WebAPIProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleService userRole;

        public UserRoleController(IUserRoleService roleService)
        {
            this.userRole = roleService;
        }

        [HttpPost("AssignRolePermission")]
        public async Task<IActionResult> AssignRolePermission(List<Rolepermission> rolepermissions)
        {
            var data = await this.userRole.AssignRolePermission(rolepermissions);
            return Ok(data);
        }
        
        [HttpGet("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var data = await this.userRole.GetAllRoles();
            if(data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet("GetAllMenus")]
        public async Task<IActionResult> GetAllMenus()
        {
            var data = await this.userRole.GetAllMenus();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet("GetAllMenusbyrole")]
        public async Task<IActionResult> GetAllMenusbyrole(string userrole)
        {
            var data = await this.userRole.GetAllMenubyrole(userrole);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet("GetAllMenusPermissionbyrole")]
        public async Task<IActionResult> GetAllMenusPermissionbyrole(string userrole, string menucode)
        {
            var data = await this.userRole.GetAllMenupermissionbyrole(userrole, menucode);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
    }
}
