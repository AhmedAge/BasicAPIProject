using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasicAPIProject.Models.CustomModel
{
    public class ISec_Users:Sec_Users
    {
        public Guid guid;
        public string DepartmentName;
        public string RoleName;

    }
}