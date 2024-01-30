using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSIT155Site.Models;
using System.Text;

namespace MSIT155Site.Controllers
{
    public class ApiController : Controller
    {
        private readonly MyDBContext _context;
        public ApiController(MyDBContext context)
        {
            _context = context;
        }

       
        public IActionResult Index()
        {
            Thread.Sleep(50000);
            //int x = 10;
            //int y = 0;
            //int z = x / y;
            //回傳純文字資料
            return Content("Hello 你好", "text/plain", Encoding.UTF8);
        }
        public IActionResult Register(string name,int age=28)
        {
            if (string.IsNullOrEmpty(name)) //預設
            {
                name = "guest";
            }

            return Content($"Hello {name}, {age}歲了", "text/plain", Encoding.UTF8);
        }

        public IActionResult Cities()
        {
            var cities = _context.Addresses.Select(c => c.City).Distinct();
            return Json(cities);
        }


        //回傳圖
        public IActionResult Avatar(int id=1)
        {
            Member? member = _context.Members.Find(id); //搜尋資料為PK可用Find();
            if(member != null)
            {
                byte[] img = member.FileData;
                if(img != null)
                {
                    return File(img, "image/jpeg");
                }
            }

            return NotFound();
        }

    }
}
