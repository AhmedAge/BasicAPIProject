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

            if (!(await Authentication.IsAuthentication(Request)))
            {
                return null;
            }

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
            if (!(await Authentication.IsAuthentication(Request)))
            {
                return null;
            }

            using (NORTHWNDEntities DB = new NORTHWNDEntities())
            {
                ISec_RoleMenu obj = new ISec_RoleMenu();
                CustSec_RoleMenu cus = null;
                List<CustSec_RoleMenu> listRoleMenu = new List<CustSec_RoleMenu>();

                var menu = await DB.Sec_Menu.OrderBy(x=>x.MenuOrder).ToListAsync();
                var RoleMenu = await DB.Sec_RoleMenu.Where(x => x.roleId == id).Select(x => new RoleMenu()
                {
                    roleId = x.roleId,
                    menuId = x.menuId
                }).ToListAsync();
                foreach (Sec_Menu m in menu)
                {
                    cus = new CustSec_RoleMenu
                    {
                        menuId = m.menuId,
                        roleId = id,
                        IsChecked = RoleMenu.Where(x => x.menuId == m.menuId).Count() > 0 ? true : false,
                        menuTitleAr = m.menuTitleAr,
                        menuTitleEn = m.menuTitleEn,
                        parentmenuId = m.menuParentId
                       
                    };

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
            //if (!(await Authentication.IsAuthentication(Request)))
            //{
            //    return -10;
            //}
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

                        var users = DB.Sec_Users.Where(x => x.roleId == roleid).Select(x => x.userId);
                        var secrolemenuuser = DB.Sec_RoleMenuUser.Where(x => users.Contains(x.userId)).ToList();
                        DB.Sec_RoleMenuUser.RemoveRange(secrolemenuuser);
                        await DB.SaveChangesAsync();

                        //List<int> menuIds = user.roleMenu.Select(x => x.menuId).ToList();

                        //var notShownInMenu = from i in DB.Sec_Menu
                        //                     where i.menuParentId != null
                        //                     where i.showInMenu == false && menuIds.Contains(i.menuParentId.Value)
                        //                     select i.menuId;
                        //foreach (int m in notShownInMenu)
                        //{
                        //    menu = new Sec_RoleMenu
                        //    {
                        //        menuId = m,
                        //        roleId = roleid
                        //    };
                        //    lst.Add(menu);
                        //}

                        foreach (Sec_RoleMenu m in user.roleMenu.Where(x => x.IsChecked == true))
                        {
                            menu = new Sec_RoleMenu
                            {
                                menuId = m.menuId,
                                roleId = m.roleId
                            };
                            lst.Add(menu);
                        }

                        DB.Sec_RoleMenu.AddRange(lst);

                        bool startPage = true;
                        List<Sec_RoleMenuUser> sec_RoleMenuUserLst = new List<Sec_RoleMenuUser>();
                        Sec_RoleMenuUser sec_RoleMenuUser = null;
                        foreach (int userId in users)
                        {
                            foreach (Sec_RoleMenu m in lst)
                            {
                                sec_RoleMenuUser = new Sec_RoleMenuUser
                                {
                                    menuId = m.menuId,
                                    roleId = m.roleId,
                                    userId = Convert.ToInt32(userId),
                                    canAdd = true,
                                    canDelete = true,
                                    canEdit = true,
                                    canView = true,
                                    startupPage_menuId = startPage == true ? true : false
                                };
                                startPage = false;
                                sec_RoleMenuUserLst.Add(sec_RoleMenuUser);
                            }
                            startPage = true;
                        }

                        DB.Sec_RoleMenuUser.AddRange(sec_RoleMenuUserLst);
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
