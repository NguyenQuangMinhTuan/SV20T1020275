using SV20T1020275.BusinessLayers;
using SV20T1020275.DataLayers;
using SV20T1020275.DataLayers.SQLServer;
using SV20T1020275.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020275.BusinessLayers
{
    public class ProductDataService
    {
        private static readonly IProductDAL productDB;

        /// <summary>
        /// Ctor
        /// </summary>
        static ProductDataService()
        {
            productDB = new ProductDAL(Configuration.ConnectionString);
        }

        public static List<Product> ListProducts(string searchValue = "")
        {
            return (List<Product>)productDB.List();
        }

        public static List<Product> ListProducts(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "", int categoryId = 0, int supplierId = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            rowCount = productDB.Count(searchValue, categoryId, supplierId);
            return productDB.List(page, pageSize, searchValue, categoryId, supplierId).ToList();
        }

        public static Product? GetProduct(int productID)
        {
            return productDB.Get(productID);

        }

        public static int AddProduct(Product data)
        {
            return productDB.Add(data);
        }

        public static bool UpdateProduct(Product data)
        {
            return productDB.Update(data);
        }

        public static bool DeleteProduct(int productID)
        {
            if (productDB.IsUsed(productID))
                return false;
            return productDB.Delete(productID);
        }

        public static bool IsUsedProduct(int productID)
        {
            return productDB.IsUsed(productID);
        }

        public static List<ProductPhoto> ListPhotos(int productID)
        {
            return (List<ProductPhoto>)productDB.ListPhotos(productID);
        }

        public static ProductPhoto? GetPhoto(long photoID)
        {
            return productDB.GetPhoto(photoID);
        }

        public static long AddPhoto(ProductPhoto data)
        {
            return productDB.AddPhoto(data);
        }

        public static bool UpdatePhoto(ProductPhoto data)
        {
            return productDB.UpdatePhoto(data);
        }

        public static bool DeletePhoto(long photoID)
        {
            return productDB.DeletePhoto(photoID);
        }

        public static List<ProductAttribute> ListAttributes(int productID)
        {
            return (List<ProductAttribute>)productDB.ListAttributes(productID);
        }

        public static ProductAttribute? GetAttribute(int attributeID)
        {
            return productDB.GetAttribute(attributeID);
        }

        public static long AddAttribute(ProductAttribute data)
        {
            return productDB.AddAttribute(data);
        }

        public static bool UpdateAttribute(ProductAttribute data)
        {
            return productDB.UpdateAttribute(data);
        }

        public static bool DeleteAttribute(long attributeID)
        {
            return productDB.DeleteAttribute(attributeID);
        }
    }
}
