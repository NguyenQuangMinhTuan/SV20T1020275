using Dapper;
using SV20T1020275.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020275.DataLayers.SQLServer
{
    public class ShipperDAL : _BaseDAL, ICommonDAL<Shipper>
    {
        public ShipperDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Shipper Data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"
                                    insert into Shippers(ShipperName,Phone)
                                    values(@ShipperName,@Phone);
                                    select @@identity;
                                ";
                var parameters = new
                {
                    ShipperName = Data.ShipperName ?? "",
                    Phone = Data.Phone ?? "",
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
                var sql = @"select count(*) from Shippers 
                            where (@searchValue = N'') or (ShipperName like @searchValue)";
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
                var sql = @"delete from Shippers where ShipperId = @ShipperId";
                var parameters = new
                {
                    ShipperId = id,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Shipper? Get(int ID)
        {
            Shipper? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Shippers where ShipperId = @ShipperId";
                var parameters = new
                {
                    ShipperId = ID
                };
                data = connection.QueryFirstOrDefault<Shipper>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool IsUsed(int ID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Orders where ShipperId = @ShipperId)
                                select 1
                            else 
                                select 0";
                var parameters = new { ShipperId = ID };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public IList<Shipper> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Shipper> data = new List<Shipper>(); ;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"with cte as
                            (
	                            select	*, row_number() over (order by ShipperName) as RowNumber
	                            from	Shippers
	                            where	(@searchValue = N'') or (ShipperName like @searchValue)
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
                data = connection.Query<Shipper>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public bool Update(Shipper Data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update Shippers 
                                    set ShipperName = @ShipperName,
                                        Phone = @Phone
                                    where ShipperId = @ShipperId";
                var parameters = new
                {
                    ShipperId = Data.ShipperID,
                    ShipperName = Data.ShipperName ?? "",
                    Phone = Data.Phone ?? "",


                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
