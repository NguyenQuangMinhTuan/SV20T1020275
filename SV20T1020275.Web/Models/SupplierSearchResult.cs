using SV20T1020275.DomainModels;

namespace SV20T1020275.Web.Models
{
    /// <summary>
    /// kết quả tìm kiếm và lấy danh sách nhà cung cấp
    /// </summary>
    public class SupplierSearchResult : BasePaginationResult
    {
        public List<Supplier> Data { get; set; }
    }
}
