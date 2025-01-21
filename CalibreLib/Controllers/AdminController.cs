using CalibreLib.Areas.Identity.Data;
using CalibreLib.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CalibreLib.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly CalibreLibContext _calibreLibContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(CalibreLibContext calibreLibContext, UserManager<ApplicationUser> userManager)
        {
            _calibreLibContext = calibreLibContext;
            _userManager = userManager;
        }
    
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetUser(string? id)
        {
            if (id == null)
                return NotFound();

            UserViewModel model = new UserViewModel();
            var appUser = _calibreLibContext.Users.FirstOrDefault(x => x.Id == id);

            if (appUser != null)
            {
                model.Id = appUser.Id;
                model.UserName = appUser.UserName;
                model.EReaderEmail = appUser.EReaderEmail;
                model.Email = appUser.Email;
                model.FirstName = appUser.FirstName;
                model.LastName = appUser.LastName;
            }
            return PartialView("EditUserPartial", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveUser(UserViewModel model)
        {
            if (model == null)
                return BadRequest();

            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
                return BadRequest("Cannot locate user to update");

            user.UserName = model.UserName;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.EReaderEmail = model.EReaderEmail;
            
            if (model.SetPassword && !string.IsNullOrEmpty(model.Password))
            {
                if (!String.Equals(model.Password, model.ConfirmPassword))
                    return BadRequest("Password and Confirm Password must match.");

                var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                if (!removePasswordResult.Succeeded)
                    return BadRequest("Failed to remove existing password");

                var addPasswordResult = await _userManager.AddPasswordAsync(user, model.Password);
                if (!addPasswordResult.Succeeded)
                    return BadRequest(String.Join(' ', addPasswordResult.Errors.Select(x => x.Description)));
            }
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest("Failed to update user");

            return Ok(model.Id);
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
                    nameof(ApplicationUser.Id) => user => user.Id,
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
                    Id = user.Id,
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
        public string Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [EmailAddress]
        [DisplayName("Send to EReader Email")]
        public string EReaderEmail { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public bool SetPassword { get; set; } = false;
        public string? Password { get; set; }
        [DisplayName("Confirm Password")]
        public string? ConfirmPassword { get; set; }
    }
}
