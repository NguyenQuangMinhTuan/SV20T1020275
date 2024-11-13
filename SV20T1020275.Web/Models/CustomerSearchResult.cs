using SV20T1020275.DomainModels;

namespace SV20T1020275.Web.Models
{
    /// <summary>
    /// kết quả tìm kiếm và lấy danh sách khách hàng
    /// </summary>
    public class CustomerSearchResult : BasePaginationResult
    {
        public List<Customer> Data { get; set; }
    }
}
