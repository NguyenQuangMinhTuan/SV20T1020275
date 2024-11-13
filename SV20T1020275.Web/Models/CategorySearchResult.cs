using SV20T1020275.DomainModels;

namespace SV20T1020275.Web.Models
{
    /// <summary>
    /// kết quả tìm kiếm và lấy danh sách loại hàng
    /// </summary>
    public class CategorySearchResult : BasePaginationResult
    {
        public List<Category> Data { get; set; }
    }
}
