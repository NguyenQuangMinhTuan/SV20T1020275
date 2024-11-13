using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020275.BusinessLayers;
using SV20T1020275.DomainModels;
using SV20T1020275.Web;
using SV20T1020275.Web.Models;
using System.Reflection;

namespace SV20T1020275.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Admin},{WebUserRoles.Employee}")]
    public class CustomerController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string CUSTOMER_SEARCH = "customer_search"; //Tên biến dùng để lưu trong session
        public IActionResult Index()
        {
            //Lấy đầu vào tìm kiếm hiện đang lưu lại trong session
            PaginationSearchInput input = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMER_SEARCH);
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
            var data = CommonDataService.ListOfCustomers(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new SV20T1020275.Web.Models.CustomerSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            //Lưu lại giá trị tìm kiếm vào session
            ApplicationContext.SetSessionData(CUSTOMER_SEARCH, input);

            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung khách hàng";
            Customer model = new Customer()
            {
                CustomerID = 0,
            };
            return View("Edit", model);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin khách hàng";
            Customer? model = CommonDataService.GetCustomer(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Save(Customer data)
        {
            try
            {
                ViewBag.Title = data.CustomerID == 0 ? "Bổ sung khách hàng" : "Cập nhật thông tin khách hàng";
                //Kiểm soát đầu vào và đưa các thông báo lỗi vào trong ModelState
                if (string.IsNullOrWhiteSpace(data.CustomerName))
                {
                    ModelState.AddModelError("CustomerName", "Tên không được để trống!");
                }
                if (string.IsNullOrWhiteSpace(data.ContactName))
                {
                    ModelState.AddModelError("ContactName", "Tên giao dịch không được để trống!");
                }
                if (string.IsNullOrWhiteSpace(data.Email))
                {
                    ModelState.AddModelError("Email", "Vui lòng nhập Email!");
                }
                if (string.IsNullOrWhiteSpace(data.Province))
                {
                    ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh thành!");
                }

                //Thông qua thuộc tính IsValid của ModelState để kiểm tra xem có tồn tại lỗi hay không
                if (!ModelState.IsValid)
                {
                    return View("Edit", data);
                }

                if (data.CustomerID == 0)
                {
                    int id = CommonDataService.AddCustomer(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Địa chỉ email bị trùng!");
                        return View("Edit", data);
                    }
                }
                else
                {
                    bool result = CommonDataService.UpdateCustomer(data);
                    if (!result)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Địa chỉ email bị trùng với khách hàng khác!");
                        return View("Edit", data);
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Lỗi không thể lưu được dữ liệu. Vui lòng thử lại sau vài phút!");
                return View("Edit", data);
            }
        }
        public ActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteCustomer(id);
                return RedirectToAction("Index");
            }

            var model = CommonDataService.GetCustomer(id);
            if (model == null)
                return RedirectToAction();
            ViewBag.AllowDelete = !CommonDataService.IsUsedCustomer(id);
            return View(model);
        }
    }
}