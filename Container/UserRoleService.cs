using Microsoft.EntityFrameworkCore;
using WebAPIProject.Helper;
using WebAPIProject.Model;
using WebAPIProject.Repos;
using WebAPIProject.Repos.Models;
using WebAPIProject.Service;

namespace WebAPIProject.Container
{
    public class UserRoleService : IUserRoleService
    {
        private readonly LearndataContext context;
        public UserRoleService(LearndataContext context)
        {
            this.context = context;
            
        }
        public async Task<APIResponse> AssignRolePermission(List<Rolepermission> _data)
        {
            APIResponse response = new APIResponse();
            int processcount = 0;
            try
            {
                using (var dbtransaction = await this.context.Database.BeginTransactionAsync())
                {
                    if (_data.Count > 0)
                    {
                        foreach (var item in _data)
                        {
                            // Corrected LINQ query
                            var userdata = await this.context.Rolepermissions
                                .FirstOrDefaultAsync(rp => rp.Userrole == item.Userrole && rp.Menucode == item.Menucode);

                            if (userdata != null)
                            {
                                // Update existing record
                                userdata.Haveview = item.Haveview;
                                userdata.Haveadd = item.Haveadd;
                                userdata.Havedelete = item.Havedelete;
                                userdata.Haveedit = item.Haveedit;
                            }
                            else
                            {
                                // Add new record
                                await this.context.Rolepermissions.AddAsync(item);
                            }

                            processcount++;
                        }

                        if (_data.Count == processcount)
                        {
                            await this.context.SaveChangesAsync();
                            await dbtransaction.CommitAsync();

                            response.Result = "Pass";
                            response.Message = "Saved successfully";
                        }
                        else
                        {
                            await dbtransaction.RollbackAsync();
                            response.Result = "Fail";
                            response.Message = "Transaction rolled back.";
                        }
                    }
                    else
                    {
                        response.Result = "Fail";
                        response.Message = "No data to process.";
                    }
                }
            }
            catch (Exception)
            {
                response = new APIResponse();
            }
            
            return response;
        }

        public async Task<List<Menu>> GetAllMenus()
        {
            return await this.context.Menus.ToListAsync();
        }

        public async Task<List<Role>> GetAllRoles()
        {
            return await this.context.Roles.ToListAsync();
        }

        public async Task<List<Appmenus>> GetAllMenubyrole(string userrole)
        {
           List<Appmenus> appmenus = new List<Appmenus>();

            var accessdata = (from menu in this.context.Rolepermissions.Where(o=>o.Userrole==userrole && o.Haveview)
                              join m in this.context.Menus on menu.Menucode equals m.Code into _jointable
                              from p in _jointable.DefaultIfEmpty()
                              select new {code = menu.Menucode, name= p.Name}).ToList();
            if(accessdata.Any())
            {
                accessdata.ForEach(item =>
                {
                    appmenus.Add(new Appmenus()
                    {
                        code = item.code,
                        Name = item.name
                    });
                });
            }

            return appmenus;
        }

        public async Task<Menupermission> GetAllMenupermissionbyrole(string userrole, string menucode)
        {
           Menupermission menupermission = new Menupermission();
            var _data = await this.context.Rolepermissions.FirstOrDefaultAsync(o => o.Userrole == userrole && o.Haveview
                        && o.Menucode == menucode);
            if(_data!=null)
            {
                menupermission.code = _data.Menucode;
                menupermission.Haveview = _data.Haveview;
                menupermission.Haveadd = _data.Haveadd;
                menupermission.Haveedit = _data.Haveedit;
                menupermission.Havedelete = _data.Havedelete;
            }
            return menupermission;
        }

    }
}
