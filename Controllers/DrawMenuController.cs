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
    public class DrawMenuController : ApiController
    {
        [HttpGet]
        //  [System.Web.Mvc.OutputCache(Duration = 60)]
        [Route("api/DrawMenu/GetString/{data}")]
        public async Task<IEnumerable<MenusInfo>> GetString(string data)
        {
            if (!(await Authentication.IsAuthentication(Request)))
            {
                return null;
            }
            //System.Threading.Thread.Sleep(1000);
            using (NORTHWNDEntities DB = new NORTHWNDEntities())
            {
                var user = DB.Sec_Users.FirstOrDefault(x => x.UserName == data);
                try
                {
                    if (user != null)
                    {
                        var userMenus = await DB.Sec_RoleMenuUser.Where(x => x.userId == user.userId).Select(x => new OrderedMenus
                        {
                            menuId = x.menuId,
                            startupPage_menuId = x.startupPage_menuId
                        }).ToListAsync();

                        List<int> menuIds = userMenus.Select(x => x.menuId).ToList();

                        var menus = await DB.Sec_Menu.Where(x => menuIds.Contains(x.menuId) && x.showInMenu == true).OrderBy(x => x.MenuOrder).ToListAsync();
                        List<MenusInfo> menusInfoLst = new List<MenusInfo>();
                        MenusInfo parent = null;
                        Sec_Menu child = null;
                        IEnumerable<MenusInfo> orderedMenu = null;

                        foreach (Sec_Menu m in menus.Where(x => x.menuParentId == null))
                        {
                            parent = new MenusInfo
                            {
                                menuId = m.menuId,
                                menuTitleAr = m.menuTitleAr,
                                menuTitleEn = m.menuTitleEn,
                                menuPah = m.menuPah,
                                menuParentId = m.menuParentId,
                                menuIcon = m.menuIcon,
                                IsActive = m.IsActive,
                                MenuOrder = m.MenuOrder,
                                Images = m.Images,
                                Description = m.Description,
                                color = m.color,
                                UserName = user.FullName,
                                UserImage = user.image,
                                ShowStartup = userMenus.FirstOrDefault(x => x.menuId == m.menuId).startupPage_menuId.GetValueOrDefault(),

                                childMenusInfo = new List<Sec_Menu>()
                            };
                            foreach (Sec_Menu c in menus.Where(x => x.menuParentId != null && x.menuParentId == m.menuId))
                            {
                                child = new Sec_Menu
                                {
                                    menuId = c.menuId,
                                    menuTitleAr = c.menuTitleAr,
                                    menuTitleEn = c.menuTitleEn,
                                    menuPah = c.menuPah,
                                    menuParentId = c.menuParentId,
                                    menuIcon = c.menuIcon,
                                    IsActive = c.IsActive,
                                    MenuOrder = c.MenuOrder,
                                    Images = c.Images,
                                    Description = c.Description,
                                    color = c.color
                                };

                                parent.childMenusInfo.Add(child);
                            }
                            menusInfoLst.Add(parent);
                        }
                        orderedMenu = menusInfoLst.OrderByDescending(x => x.ShowStartup);
                        return orderedMenu;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return null;
            }
        }
        private class OrderedMenus
        {
            public int menuId;
            public bool? startupPage_menuId;
        }



        [HttpOptions]
        [Route("api/DrawMenu/GetString/{data}", Name = "GetDrawMenuTest")]
        public IHttpActionResult GetDrawMenu(string data)
        {
            return Ok();
        }
    }
}
