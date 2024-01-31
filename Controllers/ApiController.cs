using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSIT155Site.Models;
using MSIT155Site.Models.DTO;
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
        //public IActionResult Register(string name,int age=28)
        public IActionResult Register(UserDTO _user)
        {
            if (string.IsNullOrEmpty(_user.Name)) //預設
            {
                _user.Name = "guest";
            }

            return Content($"Hello {_user.Name}, {_user.Age}歲了,電子郵件是 {_user.Email}", "text/plain", Encoding.UTF8);
        }
        //回傳城市的JSON資料

        public IActionResult Cities()
        {
            var cities = _context.Addresses.Select(c => c.City).Distinct();
            return Json(cities);
        }

        //回傳鄉鎮區的JSON資料
        public IActionResult Districts(string city)
        {
            var districts = _context.Addresses.Where(c=>c.City==city).Select(d=>d.SiteId).Distinct();
            return Json(districts);
        }

        //回傳道路的JSON資料
        public IActionResult Roads(string siteId)
        {
            var roads = _context.Addresses.Where(d=>d.SiteId== siteId).Select(r=>r.Road).Distinct();
            return Json(roads);
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

        //public IActionResult CheckAccount()
        //{
        //    if (_context.Members.Any(m => m.Name == Name))
        //    {

        //    }
        //}

    }
}
