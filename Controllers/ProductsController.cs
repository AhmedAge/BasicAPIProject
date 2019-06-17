using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;
using BasicAPIProject.Models.CustomModel;
using WebAPIAuthentication.Authentication;
using BasicAPIProject.Models;
using System;
using System.Net.Http;
using BasicAPIProject.Auth; 

namespace BasicAPIProject.Controllers
{
    public class ProductsController : ApiController
    {
        //[WebApiOutputCache(120, 60, false)]
        public async Task<IEnumerable<IProducts>> Get()
        {
            System.Threading.Thread.Sleep(500);
            if (!(await Authentication.IsAuthentication(Request)))
            {
                return null;
            }

            using (NORTHWNDEntities DB = new NORTHWNDEntities())
            {
                return await DB.Products.Select(i => new IProducts()
                {
                    ProductID = i.ProductID,
                    ProductName = i.ProductName,
                    SupplierID = i.SupplierID,
                    CategoryID = i.CategoryID,
                    QuantityPerUnit = i.QuantityPerUnit,
                    UnitPrice = i.UnitPrice,
                    UnitsInStock = i.UnitsInStock,
                    UnitsOnOrder = i.UnitsOnOrder,
                    ReorderLevel = i.ReorderLevel,
                    Discontinued = i.Discontinued,
                }).ToListAsync();
            }
        }

        [HttpOptions]
        public string test()
        {
            return string.Empty;

        }


        [HttpGet]
        public async Task<IProducts> GetProducts(int id)
        {
            if (!(await Authentication.IsAuthentication(Request)))
            {
                return null;
            }

            using (NORTHWNDEntities DB = new NORTHWNDEntities())
            {
                var data = await DB.Products.FirstOrDefaultAsync(x => x.ProductID == id);
                if (data != null)
                {

                    IProducts p = new IProducts();
                    p.ProductID = data.ProductID;
                    p.ProductName = data.ProductName;
                    p.SupplierID = data.SupplierID;
                    p.CategoryID = data.CategoryID;
                    p.QuantityPerUnit = data.QuantityPerUnit;
                    p.UnitPrice = data.UnitPrice;
                    p.UnitsInStock = data.UnitsInStock;
                    p.UnitsOnOrder = data.UnitsOnOrder;
                    p.ReorderLevel = data.ReorderLevel;
                    p.Discontinued = data.Discontinued;
                    p.guid = Guid.NewGuid();
                    await Authentication.UpdateTokenPost(Request, p.guid.ToString());
                    return p;

                }
                return null;
            }
        }


        [HttpPut]
        public async Task<int> SaveProduct(IProducts product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            using (NORTHWNDEntities DB = new NORTHWNDEntities())
            {
                var data = await DB.Products.FirstOrDefaultAsync(x => x.ProductID == product.ProductID);
                if (data != null)
                {
                    if (await Authentication.UpdateTokenPostCheck(Request, RequestType.POST))
                    {
                        data.ProductName = product.ProductName;
                        data.SupplierID = product.SupplierID;
                        data.CategoryID = product.CategoryID;
                        data.QuantityPerUnit = product.QuantityPerUnit;
                        data.UnitPrice = product.UnitPrice;
                        data.UnitsInStock = product.UnitsInStock;
                        data.UnitsOnOrder = product.UnitsOnOrder;
                        data.ReorderLevel = product.ReorderLevel;
                        data.Discontinued = product.Discontinued;
                        return await DB.SaveChangesAsync();
                    }
                    return 0;
                }
                return 0;
            }
        }


        [HttpGet]
        [Route("api/Products/GetGu", Name = "GetGu")]
        public async Task<string> GetGu()
        {
            string guid = Guid.NewGuid().ToString();
            await Authentication.UpdateTokenPost(Request, guid);
            return guid;
        }

        [HttpOptions]
        [Route("api/Products/GetGu", Name = "GetGuTest")]
        public void GetGuTest()
        { 
        }

        [HttpPost]
        public async Task<int> SaveNewProduct(IProducts product)
        {
            using (NORTHWNDEntities DB = new NORTHWNDEntities())
            {
             
                if (product != null)
                {
                    if (await Authentication.UpdateTokenPostCheck(Request, RequestType.POST))
                    {
                        try
                        {
                            Product data = new Product();
                            data.ProductName = product.ProductName;
                            data.SupplierID = product.SupplierID;
                            data.CategoryID = product.CategoryID;
                            data.QuantityPerUnit = product.QuantityPerUnit;
                            data.UnitPrice = product.UnitPrice;
                            data.UnitsInStock = product.UnitsInStock;
                            data.UnitsOnOrder = product.UnitsOnOrder;
                            data.ReorderLevel = product.ReorderLevel;
                            data.Discontinued = product.Discontinued;
                            DB.Products.Add(data);
                            return await  DB.SaveChangesAsync();
                             
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    return 0;
                }
                return 0;
            }
        }
    }
}
