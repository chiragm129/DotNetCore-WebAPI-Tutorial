using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPIProject.Model;
using WebAPIProject.Service;

namespace WebAPIProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        public UserController(IUserService service)
        {
            this.userService = service;
        }

        [HttpPost("UserRegistration")]
        public async Task<IActionResult> UserRegistration(UserRegister userRegister)
        {
            var data = await this.userService.UserRegistration(userRegister);
            return Ok(data);
        }

        [HttpPost("ConfirmRegistration")]
        public async Task<IActionResult> ConfirmRegistration(int userid, string username, string otptext)
        {
            var data = await this.userService.ConfirmRegister(userid, username, otptext);
            return Ok(data);  
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string username, string oldpassword, string newpassword)
        {
            var data = await this.userService.ResetPassword(username, oldpassword, newpassword);
            return Ok(data);
        }
        
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string username)
        {
            var data = await this.userService.ForgetPassword(username);
            return Ok(data);
        } 
        
        [HttpPost("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword(string username, string password, string otptext)
        {
            var data = await this.userService.UpdatePassword(username, password, otptext);
            return Ok(data);
        }
        
        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(string username, bool status)
        {
            var data = await this.userService.UpdateStatus(username, status);
            return Ok(data);
        }
        
        [HttpPost("UpdateRole")]
        public async Task<IActionResult> UpdateRole(string username,string role)
        {
            var data = await this.userService.UpdateRole(username, role);
            return Ok(data);
        }
    }
}
