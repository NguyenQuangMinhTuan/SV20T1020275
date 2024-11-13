using Dapper;
using SV20T1020275.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020275.DataLayers.SQLServer
{
    public class CategoryDAL : _BaseDAL, ICommonDAL<Category>
    {
        public CategoryDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Category Data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Categories where CategoryID = @CategoryID)
                                select -1
                            else
                                begin
                                    insert into Categories(CategoryName,Description,photo)
                                    values(@CategoryName,@Description,@photo);
                                    select @@identity;
                                end";
                var parameters = new
                {
                    CategoryID = Data.CategoryID,
                    photo = Data.photo ?? "",
                    CategoryName = Data.CategoryName ?? "",
                    Description = Data.Description ?? "",
                };
                //Thực thi câu lệnh
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"select count(*) from Categories 
                            where (@searchValue = N'') or (CategoryName like @searchValue)";
                var parameters = new
                {
                    searchValue = searchValue ?? "",
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Categories where CategoryId = @categoryId";
                var parameters = new
                {
                    CategoryId = id,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Category? Get(int ID)
        {
            Category? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Categories where CategoryId = @categoryId";
                var parameters = new
                {
                    CategoryId = ID
                };
                data = connection.QueryFirstOrDefault<Category>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool IsUsed(int ID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Products where CategoryId = @categoryId)
                                select 1
                            else 
                                select 0";
                var parameters = new { CategoryId = ID };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public IList<Category> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Category> data = new List<Category>(); ;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"with cte as
                            (
	                            select	*, row_number() over (order by CategoryName) as RowNumber
	                            from	Categories
	                            where	(@searchValue = N'') or (CategoryName like @searchValue)
                            )
                            select * from cte
                            where  (@pageSize = 0) 
	                            or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                            order by RowNumber";
                var parameter = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue ?? ""
                };
                data = connection.Query<Category>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public bool Update(Category Data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update Categories 
                                    set CategoryName = @CategoryName,
                                        Description = @Description,
                                        photo = @photo
                                    where CategoryID = @CategoryId";
                var parameters = new
                {
                    CategoryId = Data.CategoryID,
                    Photo = Data.photo ?? "",
                    CategoryName = Data.CategoryName ?? "",
                    Description = Data.Description ?? "",

                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
