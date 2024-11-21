using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.EntityFrameworkCore;
using WebAPIProject.Helper;
using WebAPIProject.Model;
using WebAPIProject.Repos;
using WebAPIProject.Repos.Models;
using WebAPIProject.Service;

namespace WebAPIProject.Container
{
    public class UserService : IUserService
    {
        private readonly LearndataContext context;
        public UserService(LearndataContext context)
        {
            this.context = context;

        }
        public async Task<APIResponse> ConfirmRegister(int userid, string username, string otptext)
        {
            APIResponse response = new APIResponse();
            bool otpresponse = await ValidateOTP(username, otptext);
            if (!otpresponse)
            {
                response.Result = "Fail";
                response.Message = "Invalid OTP or Expired";
            }
            else
            {
                var _tempdata = await this.context.Tempusers.FirstOrDefaultAsync(item => item.Id == userid);
                var _user = new TblUser()
                {
                    Username = username,
                    Name = _tempdata.Name,
                    Password = _tempdata.Password,
                    Email = _tempdata.Email,
                    Phone = _tempdata.Phone,
                    Failattempt = 0,
                    Isactive = true,
                    Islocked = false,
                    Role = "user"
                };
                await this.context.TblUsers.AddAsync(_user);
                await this.context.SaveChangesAsync();
                await UpdatePWDManager(username, _tempdata.Password);
                response.Result = "Pass";
                response.Message = "Registered Successfully";
            }
            return response;
        }

        public async Task<APIResponse> UserRegistration(UserRegister userRegister)
        {
            APIResponse response = new APIResponse();
            int userid = 0;
            bool isvalid = true;

            try
            {
                //duplicate user
                var _user = await this.context.TblUsers.Where(Item => Item.Username == userRegister.UserName).ToListAsync();
                if (_user.Count > 0)
                {
                    isvalid = false;
                    response.Result = "Fail";
                    response.Message = "Duplicate Username";
                }

                //duplicate email
                var _useremail = await this.context.TblUsers.Where(Item => Item.Email == userRegister.Email).ToListAsync();
                if (_useremail.Count > 0)
                {
                    isvalid = false;
                    response.Result = "Fail";
                    response.Message = "Duplicate Email";
                }

                if (userRegister != null && isvalid)
                {
                    var _tempuser = new Tempuser()
                    {
                        Code = userRegister.UserName,
                        Name = userRegister.Name,
                        Email = userRegister.Email,
                        Phone = userRegister.Phone,
                        Password = userRegister.Password
                    };
                    await this.context.Tempusers.AddAsync(_tempuser);
                    await this.context.SaveChangesAsync();
                    userid = _tempuser.Id;
                    string OTPText = Generaterandomnumber();
                    await UpdateOtp(userRegister.UserName, OTPText, "register");
                    await SendOtpMail(userRegister.Email, OTPText, userRegister.Name);
                    response.ResponseCode = 200;
                    response.Result = "Pass";
                    response.Message = userid.ToString();

                }
                else
                {
                    response.ResponseCode = 404;
                    response.Errormessage = "Not Found";
                }

            }
            catch (Exception)
            {
                response.Result = "Fail";
            }

            return response;
        }

        public async Task<APIResponse> ResetPassword(string username, string oldpassword, string newpassword)
        {
            APIResponse response = new APIResponse();
            var _user = await this.context.TblUsers.FirstOrDefaultAsync(item => item.Username == username && item.Password == oldpassword && item.Isactive == true);
            if (_user != null)
            {
                var _pwdhistory = await Validatetemphisory(username, newpassword);
                if (_pwdhistory)
                {
                    response.Result = "Fail";
                    response.Message = "dont use the same password that has used in last 3 transaction";
                }
                else
                {
                    _user.Password = newpassword;
                    await this.context.SaveChangesAsync();
                    await UpdatePWDManager(username, newpassword);
                    response.ResponseCode = 200;
                    response.Result = "pass";
                    response.Message = "Password Changed";
                }
            }
            else
            {
                response.Result = "Fail";
                response.Message = "Failed to validate old password";
            }
            return response;
        }

        public async Task<APIResponse> ForgetPassword(string username)
        {
            APIResponse response = new APIResponse();
            var _user = await this.context.TblUsers.FirstOrDefaultAsync(item => item.Username == username && item.Isactive == true);
            if (_user != null)
            {
                string otptext = Generaterandomnumber();
                await UpdateOtp(username, otptext, "forgetpassword");
                await SendOtpMail(_user.Email, otptext, _user.Name);
                response.ResponseCode = 200;
                response.Result = "Pass";
                response.Message = "OTP sent!";
            }
            else
            {
                response.ResponseCode = 404;
                response.Result = "fail";
                response.Message = "Invalid user..";
            }
            return response;
        }

        public async Task<APIResponse> UpdatePassword(string username, string password, string Otptext)
        {
            APIResponse response = new APIResponse();

            bool otpvalidation = await ValidateOTP(username, Otptext);
            if (otpvalidation)
            {
                bool pwdhistory = await Validatetemphisory(username, password);
                if (pwdhistory)
                {
                    response.Result = "Fail";
                    response.Message = "Dont use the same password that used in last 3 transaction";
                }
                else
                {
                    var _user = await this.context.TblUsers.FirstOrDefaultAsync(item => item.Username == username && item.Isactive == true);
                    if (_user != null)
                    {
                        _user.Password = password;
                        await this.context.SaveChangesAsync();
                        await UpdatePWDManager(username, password);
                        response.Result = "Pass";
                        response.Message = "Password cahnged";
                    }
                }
            }
            else
            {
                response.Result = "fail";
                response.Message = "Invalid OTP";
            }

            return response;
        }

        public async Task<APIResponse> UpdateStatus(string username, bool userstatus)
        {
            APIResponse response = new APIResponse();
            var _user = await this.context.TblUsers.FirstOrDefaultAsync(item => item.Username == username);
            if (_user != null)
            {
                _user.Isactive = userstatus;
                await this.context.SaveChangesAsync();
                response.Result = "Pass";
                response.Message = "Status changed";
            }
            else
            {
                response.Result = "Fail";
                response.Message = "Invalid user";
            }
            return response;
        }

        public async Task<APIResponse> UpdateRole(string username, string userrole)
        {
            APIResponse response = new APIResponse();
            var _user = await this.context.TblUsers.FirstOrDefaultAsync(item => item.Username == username && item.Isactive == true);
            if (_user != null)
            {
                _user.Role = userrole;
                await this.context.SaveChangesAsync();
                response.Result = "Pass";
                response.Message = "User Role changed";
            }
            else
            {
                response.Result = "Fail";
                response.Message = "Invalid user";
            }
            return response;
        }
        private async Task UpdateOtp(string username, string otptext, string otptype)
        {
            var _otp = new OtpManager()
            {
                Username = username,
                Otptext = otptext,
                Expiration = DateTime.Now.AddMinutes(30),
                Createddate = DateTime.Now,
                Otptype = otptype
            };
            await this.context.OtpManagers.AddAsync(_otp);
            await this.context.SaveChangesAsync();
        }

        private async Task<bool> ValidateOTP(string username, string OTPText)
        {
            bool response = false;
            var _data = await this.context.OtpManagers.FirstOrDefaultAsync(item => item.Username == username && item.Otptext == OTPText && item.Expiration > DateTime.Now);
            if (_data != null)
            {
                response = true;
            }
            return response;


        }

        private async Task UpdatePWDManager(string username, string password)
        {
            var _otp = new PwdManger()
            {
                Username = username,
                Password = password,
                ModifyDate = DateTime.Now
            };
            await this.context.PwdMangers.AddAsync(_otp);
            await this.context.SaveChangesAsync();
        }

        private string Generaterandomnumber()
        {
            Random random = new Random();
            string randomno = random.Next(0, 1000000).ToString("D6");
            return randomno;

        }
        private async Task SendOtpMail(string email, string OtpText, string Name)
        {

        }

        private async Task<bool> Validatetemphisory(string username, string password)
        {
            bool response = false;
            var _pwd = await this.context.PwdMangers.Where(item => item.Username == username).
                    OrderByDescending(p => p.ModifyDate).Take(3).ToListAsync();
            if (_pwd.Count > 0)
            {
                var validate = _pwd.Where(o => o.Password == password);
                if (validate.Any())
                {
                    response = true;
                }
            }
            return response;
        }


    }
}
