using Microsoft.AspNetCore.Mvc;
using SV20T1020275.Web.Models;
using System.Globalization;

namespace SV20T1020275.Web.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Create()
        {
            var model = new Models.Person()
            {
                name = "Nguyễn Văn A",
                BirthDay = new DateTime(1990, 10, 25),
                Salary = 10.2m
            };
            return View(model);
        }

        public IActionResult Save(Models.Person model, string BirthDateInput = "")
        {
            DateTime? dValue = StringToDateTime(BirthDateInput);
            if (dValue.HasValue)
            {
                model.BirthDay = dValue.Value;
            }
            return Json(model);
        }

        private DateTime? StringToDateTime(string s, string format = "d/M/yyyy;d-M-yyyy;d.M.yyyy")
        {
            try
            {
                return DateTime.ParseExact(s, format.Split(';'), CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }
    }
}
