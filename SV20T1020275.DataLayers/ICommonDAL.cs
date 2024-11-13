using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020275.DataLayers
{
    /// <summary>
    /// Mô tả các phép sử lý dữ liệu chung
    /// </summary>
    public interface ICommonDAL<T> where T : class
    {
        /// <summary>
        /// Tìm kiếm và lấy danh sách dữ liệu dưới dạng phân trang
        /// </summary>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng hiển thị trên mỗi trang (bằng 0 nếu không phân trang dữ liệu)</param>
        /// <param name="searchValue">Giá trị cần tìm kiếm (Chuỗi rỗng nếu lấy toàn bộ dữ liệu)</param>
        /// <returns></returns>
        IList<T> List(int page = 1, int pageSize = 0, string searchValue = "");

        /// <summary>
        /// Đếm số dòng dữ liệu tìm được
        /// </summary>
        /// <param name="searchValue">giá trị cần tìm (Chuỗi rỗng nếu lấy toàn bộ dữ liệu)</param>
        /// <returns></returns>
        int Count(string searchValue = "");

        /// <summary>
        /// Bổ sung dữ liệu vào cơ sở dữ liệu. Hàm trả về ID của dữ liệu được bổ sung
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        int Add(T Data);

        /// <summary>
        /// Cập nhật dữ liệu
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        bool Update(T Data);

        /// <summary>
        /// Xóa dữ liệu dựa trên ID
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        bool Delete(int id);

        /// <summary>
        /// Lấy một bản ghi dựa vào id (trả về null nếu dữ liệu không tồn tại)
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        T? Get(int ID);

        /// <summary>
        /// Kiểm tra xem bản ghi dữ liệu có mã id hiện đang sử dụng bởi các dữ liệu hay không ?
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        bool IsUsed (int ID);
    }
}
