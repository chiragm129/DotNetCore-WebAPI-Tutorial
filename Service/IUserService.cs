using WebAPIProject.Helper;
using WebAPIProject.Model;

namespace WebAPIProject.Service
{
    public interface IUserService
    {
        Task<APIResponse> UserRegistration(UserRegister userRegister);
        Task<APIResponse> ConfirmRegister(int userid, string username, string otptext);
        Task<APIResponse> ResetPassword(string username, string oldpassword, string newpassword);
        Task<APIResponse> ForgetPassword(string username);
        Task<APIResponse> UpdatePassword(string username, string password, string Otptext);
        Task<APIResponse> UpdateStatus(string username, bool userstatus);
        Task<APIResponse> UpdateRole(string username, string userrole);


    }
}
