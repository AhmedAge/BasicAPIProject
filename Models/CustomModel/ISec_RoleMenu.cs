using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasicAPIProject.Models.CustomModel
{
    public class ISec_RoleMenu
    {
        public List<CustSec_RoleMenu> roleMenu;
        public string guid;
    }

    public class CustSec_RoleMenu : Sec_RoleMenu
    {
        public bool IsChecked;
        public string menuTitleEn;
        public string menuTitleAr;
        public bool? startupPage_menuId;
    }
}