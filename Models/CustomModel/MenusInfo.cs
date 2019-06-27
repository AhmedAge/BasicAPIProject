using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasicAPIProject.Models.CustomModel
{
    public class MenusInfo: Sec_Menu
    {
        public string UserName;
        public string UserImage;
        public bool ShowStartup;

        public List<Sec_Menu> childMenusInfo;
    } 
}