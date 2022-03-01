using ComponentsServices.Base;
using CommonObject.Methods;
using CoolNetBlog.Base;
using CoolNetBlog.Models;
using CoolNetBlog.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

namespace CoolNetBlog.Controllers.AdminAccess
{
    /// <summary>
    /// 管理员后台访问入口控制器
    /// </summary>
    public class AdminController : BaseAdminController
    {
        protected SugarDataBaseStorage<AdminUser, int> _adminUserSet;
        protected SugarDataBaseStorage<SiteSetting, int> _siteSettingSet;
        public AdminController() : base()
        {
            this._adminUserSet = new SugarDataBaseStorage<AdminUser, int>();
            this._siteSettingSet = new SugarDataBaseStorage<SiteSetting, int>();

        }

        public IActionResult Login(string? v)
        {
            var st = _siteSettingSet.FirstOrDefault(s => 1 == 1);
            if (st == null)
            {
                return RedirectToAction("Index", "Home");

            }


            if (!string.IsNullOrWhiteSpace(st?.LoginUriValue) && st.LoginUriValue != v)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            ModelState.Remove("PassToken");
            if (!ModelState.IsValid)
                return View();
            var am = await _adminUserSet.FirstOrDefaultAsync(a => a.AccountName == vm.AccountName &&
                a.Password == ValueCompute.MakeMD5(vm.Password));
            if (am is null)
            {
                ModelState.AddModelError("", "账户名或密码错误。");
                return View();
            }
            HttpContext.Response.Cookies.Delete("coolnetblogadminloginxiyuaneightfourone");
            _currentCookieValue = Guid.NewGuid().ToString();
            HttpContext.Response.Cookies.Append("coolnetblogadminloginxiyuaneightfourone", _currentCookieValue);
            // 绑定token钥匙传递给视图，此token将一直用于当前后台后续的操作中，且赋值后台全局所需变量
            spassVm = new PassBaseViewModel
            {
                PassToken = am.Token,
                AccountName = am.AccountName,
            };
            return RedirectToAction("AdminHome", "Admin", new { pt = am.Token });
        }

        [HttpPost]
        public async Task<IActionResult> Reset(ResetViewModel vm)
        {
            ModelState.Clear();
            ModelState.Remove("PassToken");
            ModelState.Remove("AccountName");
            ValueCompute.SetNotNullForObj(vm);
            if (vm.NewPassword != vm.NewPasswordRep)
            {
                ModelState.AddModelError("", "重置账户：输入的两次新密码不一致。");
                return View("Login");
            }
            var am = await _adminUserSet.FirstOrDefaultAsync(a => a.AccountName == vm.OrgAccountName &&
                a.Password == ValueCompute.MakeMD5(vm.OrgPassword));
            if (am is null)
            {
                ModelState.AddModelError("", "重置账户：原管理员昵称或密码错误。");
                return View("Login");
            }
            if (string.IsNullOrWhiteSpace(vm.NewAccountName)|| (string.IsNullOrWhiteSpace(vm.NewPassword)))
            {
                ModelState.AddModelError("", "重置账户：非法空格字符。");
                return View("Login"); 
            }
            if (vm.NewAccountName.Length < 6||vm.NewPassword.Length<8)
            {
                ModelState.AddModelError("", "重置账户：新管理员昵称或密码过于简短，昵称大于6位字符密码不少于八位。");
                return View("Login");
            }
            if (vm.NewAccountName.Length >16 || vm.NewPassword.Length > 18)
            {
                ModelState.AddModelError("", "重置账户：新管理员昵称或密码过于长。");
                return View("Login");
            }
            am.Password = ValueCompute.MakeMD5(vm.NewPassword);
            am.AccountName = vm.NewAccountName;
            am.Token = Guid.NewGuid().ToString().Replace("-", string.Empty);
            try
            {
                var e = _adminUserSet.Update(am);
                if (e!=1)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "重置账户：重置失败，请重试。");
                return View("Login");
            }
            // 重置密码成功 去除当前多余的会话cookie
            HttpContext.Response.Cookies.Delete("coolnetblogadminloginxiyuaneightfourone");
            _currentCookieValue = null;
            return View("Login");
        }

        public IActionResult AdminHome(string? pt)
        {
            return View(spassVm);
        }

        /// <summary>
        /// 退出登录 去除cookie 跳转登录页
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public IActionResult LoginOut(string? pt)
        {
            HttpContext.Response.Cookies.Delete("coolnetblogadminloginxiyuaneightfourone");
            _currentCookieValue = null;
            spassVm = null;
            return RedirectToAction("Login", "Admin");
        }
    }
}
