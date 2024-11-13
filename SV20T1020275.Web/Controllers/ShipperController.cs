using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020275.BusinessLayers;
using SV20T1020275.DomainModels;
using SV20T1020275.Web.Models;
using SV20T1020275.Web;
using System.Reflection;

namespace SV20T1020275.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Admin},{WebUserRoles.Employee}")]
    public class ShipperController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string SHIPPER_SEARCH = "shipper_search"; //Tên biến dùng để lưu trong session
        public IActionResult Index()
        {
            //Lấy đầu vào tìm kiếm hiện đang lưu lại trong session
            PaginationSearchInput input = ApplicationContext.GetSessionData<PaginationSearchInput>(SHIPPER_SEARCH);
            //Trường hợp session chưa có giá trị được lưu lại thì tạo giá trị mới làm đầu vào
            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }

            return View(input);
        }

        public IActionResult Search(PaginationSearchInput input)
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfShippers(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new Models.ShipperSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            //Lưu lại giá trị tìm kiếm vào session
            ApplicationContext.SetSessionData(SHIPPER_SEARCH, input);

            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung shipper";
            Shipper model = new Shipper()
            {
                ShipperID = 0,
            };
            return View("Edit", model);
        }
        [HttpPost]
        public IActionResult Save(Shipper data)
        {
            try
            {
                ViewBag.Title = data.ShipperID == 0 ? "Bổ sung người giao hàng" : "Cập nhật thông tin người giao hàng";
                //Kiểm soát đầu vào và đưa các thông báo lỗi vào trong ModelState
                if (string.IsNullOrWhiteSpace(data.ShipperName))
                {
                    ModelState.AddModelError(nameof(data.ShipperName), "Tên không được để trống!");
                }
                if (string.IsNullOrWhiteSpace(data.Phone))
                {
                    ModelState.AddModelError(nameof(data.Phone), "Mô tả không được để trống!");
                }

                //Thông qua thuộc tính IsValid của ModelState để kiểm tra xem có tồn tại lỗi hay không
                if (!ModelState.IsValid)
                {
                    return View("Edit", data);
                }

                if (data.ShipperID == 0)
                {
                    int id = CommonDataService.AddShipper(data);
                }
                else
                {
                    bool result = CommonDataService.UpdateShipper(data);
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Lỗi không thể lưu được dữ liệu. Vui lòng thử lại sau vài phút!");
                return View("Edit", data);
            }
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin shipper";
            Shipper? model = CommonDataService.GetShipper(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public ActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteShipper(id);
                return RedirectToAction("Index");
            }

            var model = CommonDataService.GetShipper(id);
            if (model == null)
                return RedirectToAction();
            ViewBag.AllowDelete = !CommonDataService.IsUsedShipper(id);
            return View(model);
        }
    }
}
