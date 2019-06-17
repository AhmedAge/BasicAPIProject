using BasicAPIProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;  
using WebAPIAuthentication.Models.CustomModel;
using BasicAPIProject.Models.CustomModel;

namespace BasicAPIProject.Controllers
{
    public class CategoryController : ApiController
    {

        [HttpGet]
        //  [System.Web.Mvc.OutputCache(Duration = 60)]
        public async Task<IEnumerable<c_category>> Get()
        {
            //System.Threading.Thread.Sleep(1000);
            using (NORTHWNDEntities DB = new NORTHWNDEntities())
            {
                return await DB.Categories.Select(x => new c_category()
                {
                    CategoryID = x.CategoryID,
                    CategoryName = x.CategoryName,
                    Description = x.Description
                }).ToListAsync();
            }
        }

        [HttpOptions]
        public IHttpActionResult Category()
        {
            return Ok();
        }

        [HttpGet]
        public async Task<c_category> Get(int id)
        {
            using (NORTHWNDEntities DB = new NORTHWNDEntities())
            {
                return await DB.Categories.Select(x => new c_category()
                {
                    CategoryID = x.CategoryID,
                    CategoryName = x.CategoryName,
                    Description = x.Description
                    

                }).Where(x => x.CategoryID == id).FirstOrDefaultAsync();
            }
        }


        [HttpGet]
        [Route("api/Category/GetCategoryProducts/{id}", Name = "GetCategoryProducts")]
        public async Task<List<IProducts>> GetCategoryProducts(int id)
        {
            //System.Threading.Thread.Sleep(2000);
            using (NORTHWNDEntities DB = new NORTHWNDEntities())
            {
                return await DB.Products.Where(x => x.CategoryID == id).Select(i => new IProducts()
                {
                    ProductID = i.ProductID,
                    ProductName = i.ProductName,
                    QuantityPerUnit = i.QuantityPerUnit,
                    UnitPrice = i.UnitPrice
                }).ToListAsync();
            }
        }


        [HttpOptions]
        [Route("api/Category/GetCategoryProducts/{id}", Name = "CategoryProducts")]
        public IHttpActionResult CategoryProducts(int id)
        {
            string d = System.Web.HttpContext.Current.Request.UrlReferrer.AbsoluteUri ;
            return Ok();
        }
    }
}
