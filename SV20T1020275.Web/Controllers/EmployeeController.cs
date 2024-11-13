using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020275.BusinessLayers;
using SV20T1020275.DomainModels;
using SV20T1020275.Web.Models;
using SV20T1020275.Web;
using System.Reflection;

namespace SV20T1020275.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Admin}")]
    public class EmployeeController : Controller
    {
        private const int PAGE_SIZE = 9;
        private const string EMPLOYEE_SEARCH = "employee_search"; //Tên biến dùng để lưu trong session
        public IActionResult Index()
        {
            //Lấy đầu vào tìm kiếm hiện đang lưu lại trong session
            PaginationSearchInput input = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SEARCH);
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
            var data = CommonDataService.ListOfEmployees(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new SV20T1020275.Web.Models.EmployeeSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            //Lưu lại giá trị tìm kiếm vào session
            ApplicationContext.SetSessionData(EMPLOYEE_SEARCH, input);

            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhân viên";
            Employee model = new Employee()
            {
                EmployeeID = 0,
                BirthDate = new DateTime(1990, 1, 1),
                Photo = "nophoto.png"
            };
            return View("Edit", model);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin nhân viên";
            var model = CommonDataService.GetEmployee(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            if (string.IsNullOrEmpty(model.Photo))
            {
                model.Photo = "photo.png";
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult Save(Employee data, string birthDateInput, IFormFile? uploadPhoto)
        {
            try
            {
                ViewBag.Title = data.EmployeeID == 0 ? "Bổ sung nhân viên" : "Cập nhật thông tin nhân viên";
                //Kiểm soát đầu vào và đưa các thông báo lỗi vào trong ModelState
                if (string.IsNullOrWhiteSpace(data.FullName))
                {
                    ModelState.AddModelError(nameof(data.FullName), "Tên không được để trống!");
                }
                if (string.IsNullOrWhiteSpace(data.Address))
                {
                    ModelState.AddModelError(nameof(data.Address), "Địa chỉ không được để trống!");
                }
                if (string.IsNullOrWhiteSpace(data.Email))
                {
                    ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập Email!");
                }
                if (string.IsNullOrWhiteSpace(data.Phone))
                {
                    ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được để trống!");
                }

                //Thông qua thuộc tính IsValid của ModelState để kiểm tra xem có tồn tại lỗi hay không
                if (!ModelState.IsValid)
                {
                    return View("Edit", data);
                }

                //Xử lý ngày sinh
                DateTime? birthDate = birthDateInput.ToDateTime();
                if (birthDate.HasValue)
                {
                    data.BirthDate = birthDate.Value;
                }

                //Xử lý ảnh upload (nếu có ảnh thì lưu ảnh và gán lại tên file ảnh cho Employee
                if (uploadPhoto != null)
                {
                    string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";   //Tên file sẽ lưu
                    string folder = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, "images\\employees");   //Đường đãn lưu file
                    string filePath = Path.Combine(folder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        uploadPhoto.CopyTo(stream);
                    }
                    data.Photo = fileName;
                }

                if (data.EmployeeID == 0)
                {
                    int id = CommonDataService.AddEmployee(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Địa chỉ email bị trùng!");
                        return View("Edit", data);
                    }
                }
                else
                {
                    bool result = CommonDataService.UpdateEmployee(data);
                    if (!result)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Địa chỉ email bị trùng với nhân viên khác!");
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
                CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }

            var model = CommonDataService.GetEmployee(id);
            if (model == null)
                return RedirectToAction();
            ViewBag.AllowDelete = !CommonDataService.IsUsedEmployee(id);
            return View(model);
        }
    }
}
