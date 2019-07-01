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
    public class HaveAccessController : ApiController
    {
        public class check
        {
            public string email;
            public string url;
        }

        [HttpPost] 
        public async Task<bool> CheckUserAccess(check check)
        {
            if (!(await Authentication.IsAuthentication(Request)))
            {
                return false;
            }

            using (NORTHWNDEntities DB = new NORTHWNDEntities())
            {
                int path = Request.Headers.Referrer.AbsolutePath.LastIndexOf('/') + 1;

                string lastpart = Request.Headers.Referrer.AbsolutePath.Substring(path,
                    Request.Headers.Referrer.AbsolutePath.Length - path);

                int res;
                bool bat = Int32.TryParse(lastpart, out res);

                int haveAccess = 0;
                if (bat == false)
                {
                    haveAccess = (from i in DB.Sec_RoleMenuUser
                                  join m in DB.Sec_Menu on i.menuId equals m.menuId
                                  where m.menuPah == check.url
                                  join u in DB.Sec_Users on i.userId equals u.userId
                                  where u.UserName == check.email && i.userId == u.userId
                                  select i).Count();
                }
                else
                {
                    string p = check.url.Substring(0, path - 1);
                    haveAccess = (from i in DB.Sec_RoleMenuUser
                                  join m in DB.Sec_Menu on i.menuId equals m.menuId
                                  where m.menuPah == p
                                  join u in DB.Sec_Users on i.userId equals u.userId
                                  where u.UserName == check.email && i.userId == u.userId
                                  select i).Count();
                }

                if (haveAccess > 0)
                    return true;

                return false;
            }
        }

        [HttpOptions]
        public IHttpActionResult test(check check)
        {
            return Ok();
        }

    }
}
