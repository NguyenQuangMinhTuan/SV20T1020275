using Dapper;
using SV20T1020275.DataLayers.SQLServer;
using SV20T1020275.DomainModels;
using SV20T1020275.DataLayers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020275.DataLayers
{
    public class ProductDAL : _BaseDAL, IProductDAL
    {
        public ProductDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Product data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"insert into Products(ProductName, ProductDescription, SupplierID, CategoryID, Unit, Price, Photo, isSelling)
                                    values(@ProductName,@ProductDescription,@SupplierID,@CategoryID,@Unit, @Price, @Photo, @isSelling);
                                    select @@identity;";
                var parameters = new
                {
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription,

                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    Photo = data.Photo,
                    isSelling = data.IsSelling

                };
                //Thực thi câu lệnh
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();

            }
            return id;
        }



        public long AddAttribute(ProductAttribute data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from ProductAttributes where AttributeName = @AttributeName)
                                select -1
                            else
                                begin
                                    insert into ProductAttributes(ProductID,AttributeName,AttributeValue,DisplayOrder)
                                    values(@ProductID,@AttributeName,@AttributeValue,@DisplayOrder);
                                    select @@identity;
                                end";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }


        public long AddPhoto(ProductPhoto data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"insert into ProductPhotos(ProductID,Photo,Description,DisplayOrder,IsHidden)
                            values(@ProductID,@Photo,@Description,@DisplayOrder,@IsHidden);
                            select @@identity;";

                var parameters = new
                {
                    ProductID = data.ProductID,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden
                };

                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }



        public int Count(string searchValue = "", int categoryID = 0, int supplier = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = "%" + searchValue + "%";
            }
            using (var connection = OpenConnection())
            {
                var sql = @"select count(*)from Products 
                 where ((@searchValue = N'') or (ProductName like @searchValue))  
                        and (@CategoryID = 0 or CategoryID = @CategoryID)
                        and (@SupplierID = 0 or SupplierId = @SupplierID)";
                var parameters = new
                {
                    searchValue = searchValue,
                    CategoryID = categoryID,
                    SupplierID = supplier,
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return count;
        }



        public bool Delete(int ProductID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Products where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = ProductID,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }


        public bool DeleteAttribute(long attributeID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductAttributes where AttributeID = @attributeID";
                var parameters = new
                {
                    AttributeID = attributeID,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public bool DeletePhoto(long PhotoID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductPhotos where PhotoID = @photoID";
                var parameters = new
                {
                    PhotoID = PhotoID,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }


        public Product? Get(int productID)
        {
            Product? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Products where ProductId = @ProductId";
                var parameters = new
                {
                    ProductId = productID
                };
                data = connection.QueryFirstOrDefault<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }


        public ProductAttribute? GetAttribute(long attributeID)
        {
            ProductAttribute? data = null;
            using (var connection = OpenConnection())
            {
                string sql = @"select * from ProductAttributes where AttributeID = @AttributeID";
                var parameters = new
                {
                    AttributeID = attributeID,
                };
                data = connection.QueryFirstOrDefault<ProductAttribute>(sql, param: parameters, commandType: System.Data.CommandType.Text) ?? null;
                connection.Close();
            }
            return data;
        }


        public ProductPhoto? GetPhoto(long photoID)
        {
            ProductPhoto? data = null;
            using (var connection = OpenConnection())
            {
                string sql = @"select * from ProductPhotos where PhotoID = @PhotoID";
                var parameters = new
                {
                    PhotoID = photoID,
                };
                data = connection.QueryFirstOrDefault<ProductPhoto>(sql, param: parameters, commandType: System.Data.CommandType.Text) ?? null;
                connection.Close();
            }
            return data;
        }



        public bool IsUsed(int ProductID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from OrderDetails where ProductID = @ProductID)
                                select 1
                           
                            else select 0";
                var parameters = new { ProductID = ProductID };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }


        public IList<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            List<Product> data = new List<Product>(); ;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"with cte as
                            (
                                select  *,
                                        row_number() over(order by ProductName) as RowNumber
                                from    Products
                                where   (@SearchValue = N'' or ProductName like @SearchValue)
                                    and (@CategoryID = 0 or CategoryID = @CategoryID)
                                    and (@SupplierID = 0 or SupplierId = @SupplierID)
                                    and (Price >= @MinPrice)
                                    and (@MaxPrice <= 0 or Price <= @MaxPrice)
                            )
                            select * from cte
                            where   (@PageSize = 0)
                                or (RowNumber between (@Page - 1)*@PageSize + 1 and @Page * @PageSize)
                                                                
";
                var parameter = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue ?? "",
                    MaxPrice = maxPrice,
                    MinPrice = minPrice,
                    CategoryID = categoryID,
                    SupplierID = supplierID,
                };
                data = connection.Query<Product>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }


        public IList<ProductAttribute> ListAttributes(int ProductID)
        {
            List<ProductAttribute> data = new List<ProductAttribute>();

            using (var connection = OpenConnection())
            {
                var sql = @"select    *, row_number() over (order by AttributeName) as RowNumber
                 from    ProductAttributes 
                 where    ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = ProductID
                };
                data = connection.Query<ProductAttribute>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }


        public IList<ProductPhoto> ListPhotos(int ProductID)
        {
            List<ProductPhoto> data = new List<ProductPhoto>();

            using (var connection = OpenConnection())
            {
                var sql = @"select    *, row_number() over (order by Photo) as RowNumber
                 from    ProductPhotos 
                 where    ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = ProductID
                };
                data = connection.Query<ProductPhoto>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }


        public bool Update(Product data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update Products 
                                    set ProductName = @ProductName,
                                        ProductDescription = @ProductDescription,
                                        SupplierID = @SupplierID,
                                        CategoryID = @CategoryID,
                                        Unit = @Unit,
                                        Price = @Price,
                                        Photo = @Photo,
                                        isSelling = @isSelling
                                    where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription,
                    CategoryID = data.CategoryID,
                    SupplierID = data.SupplierID,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    Photo = data.Photo,
                    isSelling = data.IsSelling
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }


        public bool UpdateAttribute(ProductAttribute data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update ProductAttributes 
                       set ProductID = @productID,
                           AttributeName = @attributeName,
                           AttributeValue = @attributeValue,
                           DisplayOrder = @displayOrder
                       where AttributeID = @attributeID";
                var parameters = new
                {
                    attributeID = data.AttributeID,
                    productID = data.ProductID,
                    attributeName = data.AttributeName ?? "",
                    attributeValue = data.AttributeValue ?? "",
                    displayOrder = data.DisplayOrder
                };
                result = connection.Execute(sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }


        public bool UpdatePhoto(ProductPhoto data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update ProductPhotos 
                                set Photo = @Photo,
                                   Description = @Description,
                                   DisplayOrder = @DisplayOrder,
                                   IsHidden = @IsHidden
                                where PhotoId = @PhotoId";
                var parameters = new
                {
                    PhotoId = data.PhotoID,
                    Description = data.Description ?? "",
                    Photo = data.Photo ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden
                };
                result = connection.Execute(sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

    }
}
