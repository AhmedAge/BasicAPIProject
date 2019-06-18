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
        public IEnumerable<MenusInfo> GetString(string data)
        {
            //System.Threading.Thread.Sleep(1000);
            using (NORTHWNDEntities DB = new NORTHWNDEntities())
            {
                var user = DB.Sec_Users.FirstOrDefault(x => x.UserName == data);
                if (user != null)
                {
                    var userMenus = DB.Sec_RoleMenuUser.Where(x => x.userId == user.userId).Select(x=>x.menuId).ToList();
                    var menus =  DB.Sec_Menu.Where(x => userMenus.Contains(x.menuId)).ToList();
                    List<MenusInfo> menusInfoLst = new List<MenusInfo>();
                    MenusInfo parent = null;
                    Sec_Menu child = null;

                    foreach (Sec_Menu m in menus.Where(x => x.menuParentId == null))
                    {
                        parent = new MenusInfo();
                        parent.menuId = m.menuId;
                        parent.menuTitleAr = m.menuTitleAr;
                        parent.menuTitleEn = m.menuTitleEn;
                        parent.menuPah = m.menuPah;
                        parent.menuParentId = m.menuParentId;
                        parent.menuIcon = m.menuIcon;
                        parent.IsActive = m.IsActive;
                        parent.MenuOrder = m.MenuOrder;
                        parent.Images = m.Images;
                        parent.Description = m.Description;
                        parent.color = m.color;
                        parent.UserName = user.FullName;
                        parent.UserImage = user.image;

                        parent.childMenusInfo = new List<Sec_Menu>();
                        foreach (Sec_Menu c in menus.Where(x => x.menuParentId != null && x.menuParentId == m.menuId))
                        {
                            child = new Sec_Menu();
                            child.menuId = c.menuId;
                            child.menuTitleAr = c.menuTitleAr;
                            child.menuTitleEn = c.menuTitleEn;
                            child.menuPah = c.menuPah;
                            child.menuParentId = c.menuParentId;
                            child.menuIcon = c.menuIcon;
                            child.IsActive = c.IsActive;
                            child.MenuOrder = c.MenuOrder;
                            child.Images = c.Images;
                            child.Description = c.Description;
                            child.color = c.color;

                            parent.childMenusInfo.Add(child);
                        }
                        menusInfoLst.Add(parent);
                    }
                    return menusInfoLst;
                }
                return null;
            }
        }

        [HttpOptions]
        [Route("api/DrawMenu/GetString/{data}", Name = "GetDrawMenuTest")] 
        public IHttpActionResult GetDrawMenu(string data)
        {
            return Ok();
        }
    }
}
