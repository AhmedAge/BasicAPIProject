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
    public class RoleMenuUserController : ApiController
    {
        [HttpOptions]
        public IHttpActionResult RoleMenuUser()
        {
            return Ok();
        }


        [HttpGet]
        //  [System.Web.Mvc.OutputCache(Duration = 60)]
        public async Task<ISec_RoleMenu> Get(int id)
        {
            if (!(await Authentication.IsAuthentication(Request)))
            {
                return null;
            }
            //System.Threading.Thread.Sleep(1000);
            using (NORTHWNDEntities DB = new NORTHWNDEntities())
            {
                ISec_RoleMenu obj = new ISec_RoleMenu();
                CustSec_RoleMenu cus = null;
                List<CustSec_RoleMenu> listRoleMenu = new List<CustSec_RoleMenu>();

                var menu = await DB.Sec_Menu.ToListAsync();
                var sec_RoleMenuUser = await DB.Sec_RoleMenuUser.Where(x => x.userId == id).ToListAsync();
                var roleId = DB.Sec_Users.FirstOrDefault(x=>x.userId == id).roleId;
                var menu_roles = await DB.Sec_RoleMenu.Where(x => x.roleId == roleId).ToListAsync();

                foreach (Sec_Menu m in menu)
                {
                    cus = new CustSec_RoleMenu
                    {
                        menuId = m.menuId,
                        roleId = id,
                        IsChecked = sec_RoleMenuUser.Where(x => x.menuId == m.menuId).Count() > 0 ? true : false,
                        startupPage_menuId = sec_RoleMenuUser.Where(x => x.menuId == m.menuId && x.startupPage_menuId == true).Count() > 0 ? true : false,

                        menuTitleAr = m.menuTitleAr,
                        menuTitleEn = m.menuTitleEn
                    };

                    listRoleMenu.Add(cus);
                }
                obj.guid = Guid.NewGuid().ToString();
                obj.roleMenu = listRoleMenu;
                await Authentication.UpdateTokenPost(Request, obj.guid);

                return obj;
            }

        }


        [HttpPost]
        public async Task<int> SaveRoleMenusUsers(ISec_RoleMenuUser user)
        {
            if (!(await Authentication.IsAuthentication(Request)))
            {
                return -10;
            }
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            using (NORTHWNDEntities DB = new NORTHWNDEntities())
            {
                if (await Authentication.UpdateTokenPostCheck(Request, RequestType.POST))
                {
                    try
                    {
                        int UserId = user.UserId;
                        var alreadyExists = DB.Sec_RoleMenuUser.Where(x => x.userId == UserId).ToList();
                        DB.Sec_RoleMenuUser.RemoveRange(alreadyExists);
                        await DB.SaveChangesAsync();
                        List<Sec_RoleMenuUser> lst = new List<Sec_RoleMenuUser>();
                        Sec_RoleMenuUser menu = null;
                        foreach (CustSec_RoleMenu m in user.roleMenu.Where(x => x.IsChecked == true))
                        {
                            menu = new Sec_RoleMenuUser
                            {
                                menuId = m.menuId,
                                roleId = m.roleId,
                                userId = UserId,
                                startupPage_menuId = m.startupPage_menuId
                            };

                            lst.Add(menu);
                        }
                        DB.Sec_RoleMenuUser.AddRange(lst);

                        return await DB.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                }
                return -10;
            }
        }
    }
}
