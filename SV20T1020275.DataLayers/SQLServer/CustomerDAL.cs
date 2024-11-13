using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using SV20T1020275.DataLayers;
using SV20T1020275.DataLayers.SQLServer;
using SV20T1020275.DomainModels;

namespace SV20T1020275.DataLayers.SQLServer
{
    public class CustomerDAL : _BaseDAL, ICommonDAL<Customer>
    {
        public CustomerDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Customer Data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Customers where Email = @Email)
                                select -1
                            else
                                begin
                                    insert into Customers(CustomerName,ContactName,Province,Address,Phone,Email,IsLocked)
                                    values(@CustomerName,@ContactName,@Province,@Address,@Phone,@Email,@IsLocked);
                                    select @@identity;
                                end";
                var parameter = new
                {
                    CustomerName = Data.CustomerName ?? "",
                    ContactName = Data.ContactName ?? "",
                    Province = Data.Province ?? "",
                    Address = Data.Address ?? "",
                    Phone = Data.Phone ?? "",
                    Email = Data.Email ?? "",
                    IsLocked = Data.IsLocked
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
                return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            if (searchValue != "")
            {
                searchValue = "%" + searchValue + "%";
            }
            using (var connection = OpenConnection())
            {
                var sql = @"select count(*)from Customers 
                            where (@searchValue = N'') or (CustomerName like @searchValue)";
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
                var sql = @"delete from Customers where CustomerId = @customerId";
                var parameter = new
                {
                    CustomerId = id
                };
                result = connection.Execute(sql: sql, param: parameter, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Customer? Get(int ID)
        {
            Customer? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Customers where CustomerId = @CustomerId";
                var parameters = new
                {
                    CustomerId = ID
                };
                data = connection.QueryFirstOrDefault<Customer>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool IsUsed(int ID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Orders where CustomerId = @customerId)
                                select 1
                            else 
                                select 0";
                var parameters = new
                {
                    CustomerId = ID
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
                return result;
        }

        public IList<Customer> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Customer> data = new List<Customer>();
            if (searchValue != "")
            {
                searchValue = "%" + searchValue + "%";
            }

            using (var connection = OpenConnection())
            {
                var Sql = @"with cte as
                            (
	                            select	*, row_number() over (order by CustomerName) as RowNumber
	                            from	Customers 
	                            where	(@searchValue = N'') or (CustomerName like @searchValue)
                            )
                            select * from cte
                            where  (@pageSize = 0) 
	                            or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                            order by RowNumber";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue ?? ""
                };
                data = connection.Query<Customer>(sql: Sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public bool Update(Customer Data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Customers where CustomerId <> @customerId and Email = @email)
                                begin
                                    update Customers 
                                    set CustomerName = @customerName,
                                        ContactName = @contactName,
                                        Province = @province,
                                        Address = @address,
                                        Phone = @phone,
                                        Email = @email,
                                        IsLocked = @isLocked
                                     where CustomerId = @customerId
                                end";
                var parameters = new
                {
                    customerId = Data.CustomerID,
                    customerName = Data.CustomerName ?? "",
                    contactName = Data.ContactName ?? "",
                    province = Data.Province ?? "",
                    address = Data.Address ?? "",
                    phone = Data.Phone ?? "",
                    email = Data.Email ?? "",
                    isLocked = Data.IsLocked
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

                return result;
        }
    }
}
