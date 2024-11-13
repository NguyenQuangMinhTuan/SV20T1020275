using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020275.Web.Models;
using SV20T1020275.BusinessLayers;
using SV20T1020275.DomainModels;
using System.Reflection;

namespace SV20T1020275.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Admin},{WebUserRoles.Employee}")]
    public class ProductController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string PRODUCT_SEARCH = "product_search"; //Tên biến dùng để lưu trong session
        public IActionResult Index()
        {
            //Lấy đầu vào tìm kiếm hiện đang lưu lại trong session
            ProductSearchInput input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);
            //Trường hợp session chưa có giá trị được lưu lại thì tạo giá trị mới làm đầu vào
            if (input == null)
            {
                input = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(input);
        }

        public IActionResult Search(ProductSearchInput input)
        {
            int rowCount = 0;
            int categoryId = input.CategoryID;
            int supplierId = input.SupplierID;
            var data = ProductDataService.ListProducts(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "", categoryId, supplierId);
            var model = new Models.ProductSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            //Lưu lại giá trị tìm kiếm vào session
            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);

            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng";

            Product model = new Product()
            {
                ProductID = 0,
                Photo = "nophoto.png"
            };
            ViewBag.isEdit = false;
            return View("Edit", model);
        }


        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Cập nhật thông tin mặt hàng";
            ViewBag.isEdit = true;
            Product? model = ProductDataService.GetProduct(id);
            if (model == null)
                return RedirectToAction("Index");

            if (string.IsNullOrEmpty(model.Photo))
                model.Photo = "nophoto.png";
            return View(model);
        }

        [HttpPost]
        public IActionResult Save(Product data, IFormFile? uploadPhoto)
        {
            try
            {
                ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng" : "Cập nhật thông tin mặt hàng";
                ViewBag.IsEdit = data.ProductID == 0 ? false : true;
                //Kiểm soát đầu vào và đưa các thông báo lỗi vào trong ModelState
                if (string.IsNullOrWhiteSpace(data.ProductName))
                {
                    ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng không được để trống!");
                }
                if (string.IsNullOrWhiteSpace(data.ProductDescription))
                {
                    ModelState.AddModelError(nameof(data.ProductDescription), "Mô tả mặt hàng không được để trống!");
                }
                if (string.IsNullOrWhiteSpace(data.Unit))
                {
                    ModelState.AddModelError(nameof(data.Unit), "Đơn vị tính không được để trống!");
                }
                if (data.CategoryID.ToString().Equals("0"))
                {
                    ModelState.AddModelError(nameof(data.CategoryID), "Vui lòng chọn loại hàng!");
                }
                if (data.SupplierID.ToString().Equals("0"))
                {
                    ModelState.AddModelError(nameof(data.SupplierID), "Vui lòng chọn nhà cung cấp!");
                }

                //Thông qua thuộc tính IsValid của ModelState để kiểm tra xem có tồn tại lỗi hay không
                if (!ModelState.IsValid)
                {
                    return View("Edit", data);
                }

                //Xử lý ảnh
                if (uploadPhoto != null)
                {
                    string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                    string folder = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, "images\\products");//đường dẫn đến thư mục lưu file 
                    string filePath = Path.Combine(folder, fileName); //Đường dẫn đến file cần lưu
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        uploadPhoto.CopyTo(stream);
                    }
                    data.Photo = fileName;
                }

                //Xử lý Checkbox
                if (Request.Form["IsSelling"] == "on")
                {
                    data.IsSelling = true;
                }

                if (data.ProductID == 0)
                {
                    int id = ProductDataService.AddProduct(data);
                }
                else
                {
                    bool result = ProductDataService.UpdateProduct(data);

                }
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Lỗi không thể lưu được dữ liệu. Vui lòng thử lại sau vài phút!");
                return View("Edit", data);
            }

        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }

            var model = ProductDataService.GetProduct(id);
            if (model == null)
                return RedirectToAction("Index");


            ViewBag.AllowDelete = !ProductDataService.IsUsedProduct(id);
            return View(model);
        }

        public IActionResult Photo(int id, string method, long photoId = 0)
        {
            var model = new ProductPhoto();
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung Ảnh";
                    model = new ProductPhoto
                    {
                        PhotoID = 0,
                        ProductID = id,
                    };

                    return View("Photo", model);
                case "edit":
                    ViewBag.Title = "Thay đổi Ảnh";
                    model = ProductDataService.GetPhoto(photoId);
                    if (model == null)
                        return RedirectToAction("Index");
                    return View("Photo", model);
                case "delete":
                    ProductDataService.DeletePhoto(photoId);
                    ViewBag.Title = "Xóa Ảnh";
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }

        public IActionResult Attribute(int id, string method, int attributeId = 0)
        {
            var model = new ProductAttribute();
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính";
                    model = new ProductAttribute
                    {
                        AttributeID = 0,
                        ProductID = id,
                    };
                    return View("Attribute", model);
                case "edit":
                    ViewBag.Title = "Thay đổi thuộc tính";
                    model = ProductDataService.GetAttribute(attributeId);
                    if (model == null) return RedirectToAction("Index");
                    return View("Attribute", model);
                case "delete":
                    ViewBag.Title = "Xóa thuộc tính";
                    ProductDataService.DeleteAttribute(attributeId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }

        public IActionResult SavePhoto(ProductPhoto data, IFormFile? uploadPhoto)
        {
            if (uploadPhoto != null)
            {
                //tránh việc trùng tên file nên thêm time trước tên
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string folder = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\products");//đường dẫn đến thư mục
                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;
            }
            try
            {
                if (Request.Form["IsHidden"] == "on")
                {
                    data.IsHidden = true;
                }
                if (data.PhotoID == 0)
                {
                    long id = ProductDataService.AddPhoto(data);

                }
                else
                {
                    bool result = ProductDataService.UpdatePhoto(data);
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            return RedirectToAction("Edit", new { id = data.ProductID });
        }
        public IActionResult SaveAttribute(ProductAttribute data)
        {
            try
            {

                ViewBag.Title = data.AttributeID == 0 ? "Bổ sung thuộc tính mặt hàng" : "Cập nhật thông tin thuộc tính mặt hàng";
                //Kiểm soát đầu vào và đưa các thông báo lỗi vào trong ModelState
                if (string.IsNullOrWhiteSpace(data.AttributeName))
                {
                    ModelState.AddModelError(nameof(data.AttributeName), "Tên thuộc tính không được để trống!");
                }
                if (string.IsNullOrWhiteSpace(data.AttributeValue))
                {
                    ModelState.AddModelError(nameof(data.AttributeValue), "Mô tả thuộc tính mặt hàng không được để trống!");
                }

                //Thông qua thuộc tính IsValid của ModelState để kiểm tra xem có tồn tại lỗi hay không
                if (!ModelState.IsValid)
                {
                    return View("Attribute", data);
                }

                if (data.AttributeID == 0)
                {
                    long id = ProductDataService.AddAttribute(data);
                }
                else
                {
                    bool result = ProductDataService.UpdateAttribute(data);
                }

                return RedirectToAction("Edit", new { id = data.ProductID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Lỗi không thể lưu được dữ liệu. Vui lòng thử lại sau vài phút!");
                return View("Attribute", data);
            }
        }
    }
}
