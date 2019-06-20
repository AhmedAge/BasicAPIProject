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
    public class DepartmentsController : ApiController
    {
        [HttpGet]
        //  [System.Web.Mvc.OutputCache(Duration = 60)]
        public async Task<IEnumerable<Department>> Get()
        {
            if (!(await Authentication.IsAuthentication(Request)))
            {
                return null;
            }
            //System.Threading.Thread.Sleep(1000);
            using (NORTHWNDEntities DB = new NORTHWNDEntities())
            {
                return await DB.Departments.ToListAsync();
            }
        }

        [HttpOptions]
        public IHttpActionResult Users()
        {
            return Ok();
        }
    }
}
