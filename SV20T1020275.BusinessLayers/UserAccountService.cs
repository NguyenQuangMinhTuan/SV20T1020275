using SV20T1020275.BusinessLayers;
using SV20T1020275.DataLayers;
using SV20T1020275.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020275.BusinessLayers
{
    public static class UserAccountService
    {
        private static readonly IUserAccountDAL employeeAccountDB;

        static UserAccountService()
        {
            employeeAccountDB = new EmployeeAccountDAL(Configuration.ConnectionString);
        }
        public static UserAccount? Authorize(string userName, string password)
        {
            return employeeAccountDB.Authorize(userName, password);
            //TODO: Kiểm tra thông tin đăng nhập của Employee
        }

        public static bool ChangePassword(string userName, string oldPassword, string newPassword)
        {

            //TODO: Thay đổi mật khẩu của Employee
            return employeeAccountDB.ChangePassword(userName, oldPassword, newPassword);

        }
    }
}