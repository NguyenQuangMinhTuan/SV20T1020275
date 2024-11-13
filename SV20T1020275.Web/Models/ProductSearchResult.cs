using SV20T1020275.DomainModels;
using SV20T1020275.Web.Models;

namespace SV20T1020275.Web.Models
{
    public class ProductSearchResult : BasePaginationResult
    {
        public List<Product> Data { get; set; }
    }
}
