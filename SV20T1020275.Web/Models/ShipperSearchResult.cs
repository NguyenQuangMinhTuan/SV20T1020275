using SV20T1020275.DomainModels;

namespace SV20T1020275.Web.Models
{
    /// <summary>
    /// kết quả tìm kiếm và lấy danh sách Người giao hàng
    /// </summary>
    public class ShipperSearchResult : BasePaginationResult
    {
        public List<Shipper> Data { get; set; }
    }
}
