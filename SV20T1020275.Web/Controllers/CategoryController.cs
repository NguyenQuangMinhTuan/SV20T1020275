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
    public class CategoryController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string CATEGORY_SEARCH = "categogy_search"; //Tên biến dùng để lưu trong session
        public IActionResult Index()
        {
            //Lấy đầu vào tìm kiếm hiện đang lưu lại trong session
            PaginationSearchInput input = ApplicationContext.GetSessionData<PaginationSearchInput>(CATEGORY_SEARCH);
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
            var data = CommonDataService.ListOfCategorys(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new Models.CategorySearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            //Lưu lại giá trị tìm kiếm vào session
            ApplicationContext.SetSessionData(CATEGORY_SEARCH, input);

            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung loại hàng";
            Category model = new Category()
            {
                CategoryID = 0,
            };
            return View("Edit", model);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin loại hàng";
            Category? model = CommonDataService.GetCategory(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult Save(Category data, IFormFile? uploadPhoto)
        {
            try
            {
                ViewBag.Title = data.CategoryID == 0 ? "Bổ sung loại hàng" : "Cập nhật thông tin loại hàng";
                //Kiểm soát đầu vào và đưa các thông báo lỗi vào trong ModelState
                if (string.IsNullOrWhiteSpace(data.CategoryName))
                {
                    ModelState.AddModelError(nameof(data.CategoryName), "Tên không được để trống!");
                }
                if (string.IsNullOrWhiteSpace(data.Description))
                {
                    ModelState.AddModelError(nameof(data.Description), "Mô tả không được để trống!");
                }

               

                //Thông qua thuộc tính IsValid của ModelState để kiểm tra xem có tồn tại lỗi hay không
                if (!ModelState.IsValid)
                {
                    return View("Edit", data);
                }

                if (uploadPhoto != null)
                {
                    string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";   //Tên file sẽ lưu
                    string folder = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, "images\\category");   //Đường đãn lưu file
                    string filePath = Path.Combine(folder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        uploadPhoto.CopyTo(stream);
                    }
                    data.photo = fileName;
                }

                if (data.CategoryID == 0)
                {
                    int id = CommonDataService.AddCategory(data);
                }
                else
                {
                    bool result = CommonDataService.UpdateCategory(data);
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
                CommonDataService.DeleteCategory(id);
                return RedirectToAction("Index");
            }

            var model = CommonDataService.GetCategory(id);
            if (model == null)
                return RedirectToAction();
            ViewBag.AllowDelete = !CommonDataService.IsUsedCategory(id);
            return View(model);
        }
    }
}
