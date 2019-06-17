using BasicAPIProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using BasicAPIProject.Models.CustomModel;
using BasicAPIProject.Auth;

namespace BasicAPIProject.Controllers
{
    public class UsersController : ApiController
    {
        [HttpGet]
        //  [System.Web.Mvc.OutputCache(Duration = 60)]
        public async Task<IEnumerable<ISec_Users>> Get()
        {
            //System.Threading.Thread.Sleep(1000);
            using (NORTHWNDEntities DB = new NORTHWNDEntities())
            {
                return await DB.Sec_Users.Select(i => new ISec_Users()
                {
                    UserName = i.UserName,
                    password = i.password,
                    isActive = i.isActive,
                    roleId = i.roleId,
                    mobile = i.mobile,
                    email = i.email,
                    address = i.address,
                    tel = i.tel,
                    Isdeleted = i.Isdeleted,
                    FullName = i.FullName,
                    Department = i.Department,
                    image = i.image,
                    userId = i.userId,
                    DepartmentName = DB.Departments.FirstOrDefault(x => x.deptId == i.Department).DeptName,
                    RoleName = DB.Sec_Role.FirstOrDefault(x => x.roleId == i.roleId).roleName
                }).ToListAsync();
            }
        }

        [HttpOptions]
        public IHttpActionResult Users()
        {
            return Ok();
        }

        [HttpGet]
        public async Task<ISec_Users> GetUsers(int id)
        {
            if (!(await Authentication.IsAuthentication(Request)))
            {
                return null;
            }

            using (NORTHWNDEntities DB = new NORTHWNDEntities())
            {
                var Sec_Users = await DB.Sec_Users.FirstOrDefaultAsync(x => x.userId == id);
                if (Sec_Users != null)
                {

                    ISec_Users data = new ISec_Users();
                    data.UserName = Sec_Users.UserName;
                    data.password = Sec_Users.password;
                    data.isActive = Sec_Users.isActive;
                    data.roleId = Sec_Users.roleId;
                    data.mobile = Sec_Users.mobile;
                    data.email = Sec_Users.email;
                    data.address = Sec_Users.address;
                    data.tel = Sec_Users.tel;
                    data.Isdeleted = Sec_Users.Isdeleted;
                    data.FullName = Sec_Users.FullName;
                    data.Department = Sec_Users.Department;
                    data.image = Sec_Users.image;
                    data.userId = Sec_Users.userId;
                    data.guid = Guid.NewGuid();
                    await Authentication.UpdateTokenPost(Request, data.guid.ToString());
                    return data;

                }
                return null;
            }
        }


        [HttpPut]
        public async Task<int> SaveUser(ISec_Users user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            using (NORTHWNDEntities DB = new NORTHWNDEntities())
            {
                if (await Authentication.UpdateTokenPostCheck(Request, RequestType.POST))
                {
                    var data = await DB.Sec_Users.FirstOrDefaultAsync(x => x.userId == user.userId);
                    if (data != null)
                    {
                        data.UserName = user.UserName;
                        data.password = user.password;
                        data.isActive = user.isActive;
                        data.roleId = user.roleId;
                        data.mobile = user.mobile;
                        data.email = user.email;
                        data.address = user.address;
                        data.tel = user.tel;
                        data.Isdeleted = user.Isdeleted;
                        data.FullName = user.FullName;
                        data.Department = user.Department;
                        data.image = user.image;

                        await DB.SaveChangesAsync();

                        #region Filling user role menu items
                        var oldRoleDel = DB.Sec_RoleMenuUser.Where(x => x.userId == user.userId).ToList();
                        DB.Sec_RoleMenuUser.RemoveRange(oldRoleDel);
                        await DB.SaveChangesAsync();

                        var role_menu = await DB.Sec_RoleMenu.Where(x => x.roleId == user.roleId).ToListAsync();
                        Sec_RoleMenuUser userMenus = null;
                        List<Sec_RoleMenuUser> userMenusLst = new List<Sec_RoleMenuUser>();
                        int firstMenuSelect = 0;
                        foreach (Sec_RoleMenu m in role_menu)
                        {
                            userMenus = new Sec_RoleMenuUser();
                            userMenus.roleMenuId = m.roleMenuId;
                            userMenus.roleId = m.roleId;
                            userMenus.menuId = m.menuId;
                            userMenus.startupPage_menuId = firstMenuSelect == 0 ? true : false;
                            userMenus.userId = (int)data.userId;
                            userMenus.canView = true;
                            userMenus.canEdit = true;
                            userMenus.canDelete = true;
                            userMenus.canAdd = true;
                            firstMenuSelect++;
                            userMenusLst.Add(userMenus);

                        }
                        DB.Sec_RoleMenuUser.AddRange(userMenusLst);
                        return await DB.SaveChangesAsync();
                        #endregion
                    }
                    return 0;
                }
                return 0;
            }
        }

        [HttpPost]
        public async Task<int> SaveNewUser(ISec_Users user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            using (NORTHWNDEntities DB = new NORTHWNDEntities())
            {
                if (await Authentication.UpdateTokenPostCheck(Request, RequestType.POST))
                {
                    Sec_Users data = new Sec_Users();
                    string username = user.email.Split('@')[0];
                    data.UserName = username;
                    data.password = user.password;
                    data.isActive = user.isActive;
                    data.roleId = user.roleId;
                    data.mobile = user.mobile;
                    data.email = user.email;
                    data.address = user.address;
                    data.tel = user.tel;
                    data.Isdeleted = user.Isdeleted;
                    data.FullName = user.FullName;
                    data.Department = user.Department;
                    data.image = user.image;
                    data.password = username+"@123";

                    DB.Sec_Users.Add(data);
                    await DB.SaveChangesAsync();

                    #region Filling user role menu items
                    var role_menu = await DB.Sec_RoleMenu.Where(x => x.roleId == user.roleId).ToListAsync();
                    Sec_RoleMenuUser userMenus = null;
                    List<Sec_RoleMenuUser> userMenusLst = new List<Sec_RoleMenuUser>();
                    int firstMenuSelect = 0;
                    foreach (Sec_RoleMenu m in role_menu)
                    {
                        userMenus = new Sec_RoleMenuUser();
                        userMenus.roleMenuId = m.roleMenuId;
                        userMenus.roleId = m.roleId;
                        userMenus.menuId = m.menuId;
                        userMenus.startupPage_menuId = firstMenuSelect == 0 ? true : false;
                        userMenus.userId = (int)data.userId;
                        userMenus.canView = true;
                        userMenus.canEdit = true;
                        userMenus.canDelete = true;
                        userMenus.canAdd = true;
                        firstMenuSelect++;
                        userMenusLst.Add(userMenus);

                    }
                    DB.Sec_RoleMenuUser.AddRange(userMenusLst);
                    return await DB.SaveChangesAsync();
                    #endregion




                }
                return -10;
            }
        }


    }
}
