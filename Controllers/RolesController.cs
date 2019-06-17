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
    public class RolesController : ApiController
    {
        [HttpGet]
        //  [System.Web.Mvc.OutputCache(Duration = 60)]
        public async Task<IEnumerable<Sec_Role>> Get()
        {
            //System.Threading.Thread.Sleep(1000);
            using (NORTHWNDEntities DB = new NORTHWNDEntities())
            {
                return await DB.Sec_Role.ToListAsync();
            }
        }

        [HttpOptions]
        public IHttpActionResult Roles()
        {
            return Ok();
        }


        [HttpGet]
        //  [System.Web.Mvc.OutputCache(Duration = 60)]
        public async Task<ISec_RoleMenu> Get(int id)
        {
            //System.Threading.Thread.Sleep(1000);
            using (NORTHWNDEntities DB = new NORTHWNDEntities())
            {
                ISec_RoleMenu obj = new ISec_RoleMenu();
                CustSec_RoleMenu cus = null;
                List<CustSec_RoleMenu> listRoleMenu = new List<CustSec_RoleMenu>();

                var menu = await DB.Sec_Menu.ToListAsync();
                var RoleMenu = await DB.Sec_RoleMenu.Where(x => x.roleId == id).Select(x => new RoleMenu()
                {
                    roleId = x.roleId,
                    menuId = x.menuId
                }).ToListAsync();
                foreach (Sec_Menu m in menu)
                {
                    cus = new CustSec_RoleMenu();
                    cus.menuId = m.menuId;
                    cus.roleId = id;
                    cus.IsChecked = RoleMenu.Where(x => x.menuId == m.menuId).Count() > 0 ? true : false;
                    cus.menuTitleAr = m.menuTitleAr;
                    cus.menuTitleEn = m.menuTitleEn;

                    listRoleMenu.Add(cus);
                }
                obj.guid = Guid.NewGuid().ToString();
                obj.roleMenu = listRoleMenu;
                await Authentication.UpdateTokenPost(Request, obj.guid);

                return obj;
            }

        }
        private class RoleMenu
        {
            public int menuId;
            public int roleId;

        }

        [HttpPost]
        public async Task<int> SaveRoleMenus(ISec_RoleMenu user)
        {
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
                        int roleid = user.roleMenu[0].roleId;
                        var alreadyExists = DB.Sec_RoleMenu.Where(x => x.roleId == roleid).ToList();
                        DB.Sec_RoleMenu.RemoveRange(alreadyExists);
                        await DB.SaveChangesAsync();
                        List<Sec_RoleMenu> lst = new List<Sec_RoleMenu>();
                        Sec_RoleMenu menu = null;
                        foreach (Sec_RoleMenu m in user.roleMenu.Where(x => x.IsChecked == true))
                        {
                            menu = new Sec_RoleMenu();
                            menu.menuId = m.menuId;
                            menu.roleId = m.roleId;
                            lst.Add(menu);
                        }
                        DB.Sec_RoleMenu.AddRange(lst);

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
