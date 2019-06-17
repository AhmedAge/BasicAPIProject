using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasicAPIProject.Models.CustomModel
{
    public class IProducts
    {
        public int ProductID;
        public string ProductName;
        public int? SupplierID;
        public int? CategoryID;
        public string QuantityPerUnit;
        public decimal? UnitPrice;
        public short? UnitsInStock;
        public short? UnitsOnOrder;
        public short? ReorderLevel;
        public bool Discontinued;
        public Guid guid;
    }
}