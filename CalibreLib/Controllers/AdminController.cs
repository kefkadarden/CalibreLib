using CalibreLib.Areas.Identity.Data;
using CalibreLib.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly CalibreLibContext _calibreLibContext;

        public AdminController(CalibreLibContext calibreLibContext)
        {
            _calibreLibContext = calibreLibContext;
        }
    
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetUsers()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            List<UserViewModel> apUsers = new List<UserViewModel>();
            var users = (from user in _calibreLibContext.Users select user);
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                System.Linq.Expressions.Expression<Func<ApplicationUser, string>> sortBy = sortColumn switch
                {
                    nameof(ApplicationUser.UserName) => user => user.UserName,
                    nameof(ApplicationUser.FirstName) => user => user.FirstName,
                    nameof(ApplicationUser.LastName) => user => user.LastName,
                    nameof(ApplicationUser.Email) => user => user.Email,
                    _ => throw new ArgumentException(),
                };
                if (sortColumnDirection == "asc")
                    users = users.OrderBy(sortBy);
                else
                    users = users.OrderByDescending(sortBy);
            }
            if (!string.IsNullOrEmpty(searchValue))
            {
                users = users.Where(m => m.FirstName.Contains(searchValue)
                                            || m.LastName.Contains(searchValue)
                                            || m.UserName.Contains(searchValue)
                                            || m.Email.Contains(searchValue));
            }
            
            
            recordsTotal = users.Count();
            var data = users.Skip(skip).Take(pageSize).ToList();
            data.ForEach(user => {
                apUsers.Add(new UserViewModel()
                {
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EReaderEmail = user.EReaderEmail,
                    Email = user.Email
                });
            });
            var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = apUsers };
            return Ok(jsonData);
        }
    }

    public class UserViewModel()
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EReaderEmail { get; set; }
        public string Email { get; set; }
    }
}
