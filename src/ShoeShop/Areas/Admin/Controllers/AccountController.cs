using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeShop.Models;
using ShoeShop.Repository;
using System.Text.RegularExpressions;

namespace ShoeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Account/[action]/{id?}")]
    public class AccountController : Controller
    {
        private readonly DataContext _dataContext;
        public AccountController(DataContext context)
        {
            _dataContext = context;
        }
        public IActionResult Index()
        {
            var users = _dataContext.Users.ToList();
            return View(users);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserModel Users)
        {
            var checkEmail = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == Users.Email);
            var checkPhone = await _dataContext.Users.FirstOrDefaultAsync(u => u.Phone == Users.Phone);

            if (Users.Password != Users.ConfirmPassword)
            {
                TempData["error"] = "Xác nhận mật khẩu không trùng khớp!";
                ModelState.AddModelError("Password", "Xác nhận mật khẩu không trùng khớp!");
                return View();
            }

            if (checkEmail != null)
            {
                TempData["error"] = "Email đã tồn tại!";
                ModelState.AddModelError("Email", "Email đã tồn tại!");
                return View();
            }

            if (checkPhone != null)
            {
                TempData["error"] = "Số điện thoại đã tồn tại!";
                ModelState.AddModelError("Phone", "Số điện thoại đã tồn tại!");
                return View();
            }

            string BCryptPassword = BCrypt.Net.BCrypt.HashPassword(Users.Password);
            //Lưu vào DB
            Users.Created_at = DateTime.Now;
            Users.Updated_at = DateTime.Now;            
            Users.Password = BCryptPassword;
            _dataContext.Add(Users);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Thêm tài khoản thành công";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var Users = await _dataContext.Users.FindAsync(id);
            if (Users == null)
            {
                return NotFound();
            }
            return View(Users);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UserModel Users)
        {
            var checkEmail = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == Users.Email);
            var checkPhone = await _dataContext.Users.FirstOrDefaultAsync(u => u.Phone == Users.Phone);
            var user = await _dataContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }            
            if (checkEmail != null)
            {
                if (checkEmail.Id != id)
                {
                    TempData["error"] = "Email đã tồn tại!";
                    ModelState.AddModelError("Email", "Email đã tồn tại!");
                    return View(Users);
                }
            }            
            if (checkPhone != null)
            {
                if (checkPhone.Id != id)
                {
                    TempData["error"] = "Số điện thoại đã tồn tại!";
                    ModelState.AddModelError("Phone", "Số điện thoại đã tồn tại!");
                    return View(Users);
                }
            }            
            if (!string.IsNullOrEmpty(Users.Password) || !string.IsNullOrEmpty(Users.ConfirmPassword))
            {
                var hasNumber = new Regex(@"[0-9]+");
                var hasUpperChar = new Regex(@"[A-Z]+");
                var hasMiniMaxChars = new Regex(@".{6,15}");
                var hasLowerChar = new Regex(@"[a-z]+");
                var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
                if (!hasLowerChar.IsMatch(Users.Password) || !hasLowerChar.IsMatch(Users.Password) || !hasLowerChar.IsMatch(Users.Password) || !hasLowerChar.IsMatch(Users.Password) || !hasLowerChar.IsMatch(Users.Password))
                {
                    TempData["error"] = "Mật khẩu không hợp lệ!";
                    ModelState.AddModelError("Password", "Mật khẩu không hợp lệ!");
                    return View(Users);
                }
                if (Users.Password.Contains(" "))
                {
                    TempData["error"] = "Mật khẩu không được có khoảng trắng!";
                    ModelState.AddModelError("Password", "Mật khẩu không được có khoảng trắng!");
                    return View(Users);
                }
                if (Users.Password != Users.ConfirmPassword)
                {
                    TempData["error"] = "Xác nhận mật khẩu không trùng khớp!";
                    ModelState.AddModelError("Password", "Xác nhận mật khẩu không trùng khớp!");
                    return View(Users);
                }                
                bool verifiPassword = BCrypt.Net.BCrypt.Verify(Users.Password, user.Password);
                if (verifiPassword == true)
                {
                    TempData["error"] = "Mật khẩu mới không được trùng với mật khẩu cũ!";
                    ModelState.AddModelError("Password", "Mật khẩu mới không được trùng với mật khẩu cũ!");
                    return View(Users);
                }
                string BCryptPassword = BCrypt.Net.BCrypt.HashPassword(Users.Password);
                user.Password = BCryptPassword;
            }
            else
            {
                user.Password = user.Password;
            }

            //Lưu vào DB
            if (checkPhone == null)
            {
                user.Phone = Users.Phone;
            }
            if (checkEmail == null)
            {
                user.Email = Users.Email;
            }
            user.Name = Users.Name;
            user.Actived = Users.Actived;
            user.Updated_at = DateTime.Now;
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Cập nhật tài khoản thành công";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int Id)
        {
            UserModel user = await _dataContext.Users.FindAsync(Id);

            _dataContext.Users.Remove(user);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Danh mục tài khoản thành công";
            return RedirectToAction("Index");
        }
    }
}
