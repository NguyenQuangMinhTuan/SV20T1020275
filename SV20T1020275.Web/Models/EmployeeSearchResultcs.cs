using SV20T1020275.DomainModels;

namespace SV20T1020275.Web.Models
{
    /// <summary>
    /// kết quả tìm kiếm và lấy danh sách nhân viên
    /// </summary>
    public class EmployeeSearchResult : BasePaginationResult
    {
        public List<Employee> Data { get; set; }
    }
}
