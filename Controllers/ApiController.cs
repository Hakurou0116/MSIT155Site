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
        private readonly IWebHostEnvironment _environment;
        public ApiController(MyDBContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
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

            //string uploadPath = @"C:\Users\User\Documents\Ajax\MSIT155Site\wwwroot\uploads\a.jpg";
            string fileName = "empty.jpg";
            if (_user.Avatar != null)
            {
                fileName = _user.Avatar.FileName;
            }
            string uploadPath = Path.Combine(_environment.WebRootPath, "uploads", fileName);


            //return Content($"Hello {_user.Name}, {_user.Age}歲了,電子郵件是 {_user.Email}", "text/plain", Encoding.UTF8);
            //return Content($"{_user.Avatar?.FileName} - {_user.Avatar?.ContentType} - {_user.Avatar?.Length}");
            using (var fileStream = new FileStream(uploadPath, FileMode.Create))
            {
                _user.Avatar?.CopyTo(fileStream);
            }

            return Content(uploadPath);
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

        public IActionResult CheckAccount(string Name)
        {
            if (_context.Members.Any(m => m.Name == Name))
                return Content($"帳號 {Name} 已被註冊");
            else
                return Content($"帳號 {Name} 可以使用");
        }

        //景點資料
        [HttpPost]
        public IActionResult Spots([FromBody] SearchDTO _search)
        {
            //根據分類編號搜尋
            var spots = _search.CategoryId == 0 ? _context.SpotImagesSpots : _context.SpotImagesSpots.Where(s => s.CategoryId == _search.CategoryId);

            //根據關鍵字搜尋
            if (!string.IsNullOrEmpty(_search.Keyword))
            {
                spots = spots.Where(s => s.SpotTitle.Contains(_search.Keyword) || s.SpotDescription.Contains(_search.Keyword));
            }

            //排序
            switch (_search.SortBy)
            {
                case "spotTitle":
                    spots = _search.SortType == "asc" ? spots.OrderBy(s => s.SpotTitle) : spots.OrderByDescending(s => s.SpotTitle);
                    break;
                case "categoryId":
                    spots = _search.SortType == "asc" ? spots.OrderBy(s => s.CategoryId) : spots.OrderByDescending(s => s.CategoryId);
                    break;
                default: //spotId
                    spots = _search.SortType == "asc" ? spots.OrderBy(s => s.SpotId) : spots.OrderByDescending(s => s.SpotId);
                    break;
            }

            //總共有幾筆
            int totalCount = spots.Count();
            //一頁幾筆資料
            int pageSize = _search.PageSize ?? 9;
            //計算總共有幾頁
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            //目前第幾頁
            int page = _search.Page ?? 1;


            //分頁
            spots = spots.Skip((page - 1) * pageSize).Take(pageSize);


            SpotsPagingDTO spotsPaging = new SpotsPagingDTO();
            spotsPaging.TotalPages = totalPages;
            spotsPaging.SpotsResult = spots.ToList();

            return Json(spotsPaging);
        }

        public IActionResult SpotTitle(string title)
        {
            var titles = _context.Spots.Where(s => s.SpotTitle.Contains(title)).Select(s => s.SpotTitle).Take(8);
            return Json(titles);
        }

    }
}
