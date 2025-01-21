using CalibreLib.Areas.Identity.Data;
using CalibreLib.Data;
using CalibreLib.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Graph;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace CalibreLib.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly CalibreLibContext _calibreLibContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly IEmailSender _emailSender;

        public AdminController(CalibreLibContext calibreLibContext, 
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            IEmailSender emailSender)
        {
            _calibreLibContext = calibreLibContext;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _emailSender = emailSender;
        }
    
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetUser(string? id)
        {
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

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string? id)
        {
            if (id == null)
                return BadRequest("Missing Id");

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return BadRequest("Cannot find user");

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                var errorsCreate = string.Empty;
                ModelState.AsEnumerable().ToList().ForEach(x => errorsCreate += String.Join(' ', x.Value.Errors.Select(y => y.ErrorMessage)));
                return BadRequest(errorsCreate);
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> SaveUser(UserViewModel model)
        {
            if (model == null)
                return BadRequest();

            if (model.SetPassword && !string.IsNullOrEmpty(model.Password))
            {
                if (!String.Equals(model.Password, model.ConfirmPassword))
                    return BadRequest("Password and Confirm Password must match.");
            }

            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                if (ModelState.IsValid)
                {
                    user = CreateUser();

                    await _userStore.SetUserNameAsync(user, model.UserName, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);
                    var resultCreate = await _userManager.CreateAsync(user, model.Password);

                    if (resultCreate.Succeeded)
                    {
                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code, returnUrl = Url.Content("~/") },
                            protocol: Request.Scheme);

                        try
                        {
                            await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                        }catch
                        {

                        }
                    }
                    else
                    {
                        foreach (var error in resultCreate.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        var errorsCreate = string.Empty;
                        ModelState.AsEnumerable().ToList().ForEach(x => errorsCreate += String.Join(' ', x.Value.Errors.Select(y => y.ErrorMessage)));
                        return BadRequest(errorsCreate);
                    }
                }
                else
                {
                    var errorsCreate2 = string.Empty;
                    ModelState.AsEnumerable().ToList().ForEach(x => errorsCreate2 += String.Join(' ', x.Value.Errors.Select(y => y.ErrorMessage)));
                    return BadRequest(errorsCreate2);
                } 
            }

            user.UserName = model.UserName;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.EReaderEmail = model.EReaderEmail;
            
            if (model.SetPassword && !string.IsNullOrEmpty(model.Password))
            {
                var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                //if (!removePasswordResult.Succeeded)
                //  return BadRequest("Failed to remove existing password");

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
        public string? Id { get; set; }
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
        public string? EReaderEmail { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        public bool SetPassword { get; set; } = false;
        public string? Password { get; set; }
        [DisplayName("Confirm Password")]
        public string? ConfirmPassword { get; set; }
    }
}
