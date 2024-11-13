using System.Globalization;

namespace SV20T1020275.Web
{
    public static class Converter
    {
        /// <summary>
        /// Chuyển chuỗi s sang giá trị kiểu DateTime (nếu không thành công thì trả về Null)
        /// </summary>
        /// <param name="s"></param>
        /// <param name="fomats"></param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this string s, string fomats = "d/M/yyyy;d-M-yyy;d.M.yyyy")
        {
            try
            {
                return DateTime.ParseExact(s, fomats.Split(';'), CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }
    }
}
