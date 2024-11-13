using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020275.BusinessLayers
{
    public static class Configuration
    {
        /// <summary>
        /// chuỗi thông số kết nối đến csdl
        /// </summary>
        public static string ConnectionString { get; private set; } = "";

        public static void Initialize(string connectionString) { ConnectionString = connectionString; }

    }
}