using ComponentsServices.Base;
using CommonObject.Methods;
using CoolNetBlog.Base;
using CoolNetBlog.Models;
using CoolNetBlog.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using CommonObject.Constructs;
using CommonObject.Enums;
using System.Runtime.InteropServices;
using System.IO.Compression;

namespace CoolNetBlog.Controllers.AdminAccess
{
    /// <summary>
    /// 管理员后台访问入口控制器
    /// </summary>
    public class AdminController : BaseAdminController
    {
        protected SugarDataBaseStorage<AdminUser, int> _adminUserSet;
        protected SugarDataBaseStorage<SiteSetting, int> _siteSettingSet;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(IWebHostEnvironment webHostEnvironment) : base()
        {
            this._adminUserSet = new SugarDataBaseStorage<AdminUser, int>();
            this._siteSettingSet = new SugarDataBaseStorage<SiteSetting, int>();
            this._webHostEnvironment = webHostEnvironment;
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
                Email = am.Email,
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
            if (string.IsNullOrWhiteSpace(vm.Email))
            {
                ModelState.AddModelError("", "重置账户：请输入一个你的专用邮箱");
                return View("Login");
            }
            am.Password = ValueCompute.MakeMD5(vm.NewPassword);
            am.AccountName = vm.NewAccountName;
            am.Email = vm.Email;
            am.Token = Guid.NewGuid().ToString().Replace("-", string.Empty);
            try
            {
                var e = await _adminUserSet.UpdateByIgColsAsync(am, new String[] { "SmtpHost", "EmailPassword", "SmtpPort", "SmtpIsUseSsl" });
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

        /// <summary>
        /// 更新smtp邮箱数据
        /// </summary>
        /// <param name="smtpEmailVm"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> UpSmtpEmailInfo(string? pt, [FromBody]AdminUser smtpEmailVm)
        {
            ValueResult result = new ValueResult();
            result.Code = ValueCodes.UnKnow;
            if (string.IsNullOrWhiteSpace(smtpEmailVm.Email)|| string.IsNullOrWhiteSpace(smtpEmailVm.SmtpHost) ||
                smtpEmailVm.EmailPassword==null|| smtpEmailVm.EmailPassword.Length<=0 || smtpEmailVm.SmtpPort<=0 || smtpEmailVm.SmtpPort==null||
                smtpEmailVm.SmtpIsUseSsl==null)
            {
                result.TipMessage = "请将邮箱smtp信息全部填写完整。";
                return Json(result);
            }
            var am = await _adminUserSet.FirstOrDefaultAsync(a => a.AccountName == smtpEmailVm.AccountName);
            if (am is null)
            {
                result.Code = ValueCodes.Error;
                result.HideMessage = "修改邮箱smtp信息时失败，当前管理员AccountName在AdminUser表不存在。";
                result.TipMessage = "更新失败 请重新登录试试。";
                return Json(result);
            }

            // 更新当前管理员的邮箱服务数据字段
            //am.EmailPassword = ValueCompute.MakeMD5(smtpEmailVm.EmailPassword);
            am.EmailPassword = smtpEmailVm.EmailPassword;
            am.SmtpHost = smtpEmailVm.SmtpHost;
            am.Email = smtpEmailVm.Email;
            am.SmtpPort = smtpEmailVm.SmtpPort;
            am.SmtpIsUseSsl = smtpEmailVm.SmtpIsUseSsl;
            try
            {
                var e = _adminUserSet.Update(am);
                if (e != 1)
                {
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                result.Code = ValueCodes.Error;
                result.HideMessage = "修改邮箱smtp信息时失败，更新数据表引发异常："+e.Message;
                result.TipMessage = "更新失败 请重新登录试试。";
                return Json(result);
            }

            // 修改邮箱信息密码成功 当前管理员用户的全局Email变量重赋值
            spassVm.Email = smtpEmailVm.Email;
            result.Code = ValueCodes.Success;
            result.TipMessage = "更改成功。";
            return Json(result);
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

        /// <summary>
        /// 备份数据库
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult BackupData(string? pt, [FromBody] DataBackViewModel dbVm)
        {
            ValueResult result = new ValueResult();
            result.Code = ValueCodes.UnKnow;
            if (string.IsNullOrWhiteSpace(dbVm.DbUserName) || string.IsNullOrWhiteSpace(dbVm.dbPassword))
            {
                result.TipMessage = "请输入数据库用户名和密码。";
                return Json(result);
            }
            // 事先有备份主目录BACK-COOLNETBLOG 删除
            if (Directory.Exists(Path.Combine(_webHostEnvironment.ContentRootPath, "BACK-COOLNETBLOG")))
            {
                Directory.Delete(Path.Combine(_webHostEnvironment.ContentRootPath, "BACK-COOLNETBLOG"), true);    
            }
            // 创建备份主目录(位于app目录下面，不是wwwroot下面)
            //var tmpBackDataDownDir = Path.Combine(_webHostEnvironment.ContentRootPath, "BACK-COOLNETBLOG", ValueCompute.Guid16().Replace("-", ""));        
            var tmpBackDataDownDir = Path.Combine(_webHostEnvironment.ContentRootPath, "BACK-COOLNETBLOG");        
            Directory.CreateDirectory(tmpBackDataDownDir);
            //执行备份命令，并且生成sql，类似{ContentRootPath}/BACK-COOLNETBLOG/CoolNetBlog-Db.sql
            var dBSqlPath = Path.Combine(tmpBackDataDownDir, "CoolNetBlog-Db.sql");
            var cmdInput = @$"mysqldump -u{dbVm.DbUserName} -p{dbVm.dbPassword} CoolNetBlog > {dBSqlPath}";
            var bs = BashExecute.Bash(cmdInput, RuntimeInformation.IsOSPlatform(OSPlatform.Linux));
            if (bs.Code != ValueCodes.Success)
            {
                result.Code = ValueCodes.Error;
                result.TipMessage = "执行失败，请重试。";
                result.HideMessage = "数据备份执行失败：" + bs.HideMessage;
                return Json(result);
            }

            try
            {
                *//*mysql dump命令备份完毕 开始追加sql语句到sql文件头*//*
                string orgSqlStr = "";
                // 读出sql
                using (var rf = System.IO.File.OpenText(dBSqlPath))
                {
                    orgSqlStr = rf.ReadToEnd();
                }
                // 覆写sql文件
                using (FileStream sqlFile = new(dBSqlPath, FileMode.Open, FileAccess.Write))
                {
                    sqlFile.Seek(0, SeekOrigin.Begin);
                    using StreamWriter sw = new(sqlFile);
                    sw.WriteLine($"-- ------------------------------\r\n" +
                        $"-- SQL Dump By Admin Write Top {DateTime.Now}\r\n" +
                        $"-- ------------------------------");
                    sw.WriteLine("CREATE DATABASE IF NOT EXISTS CoolNetBlog CHARACTER SET utf8 COLLATE utf8_general_ci;");
                    sw.WriteLine("USE CoolNetBlog;");
                    sw.Write(orgSqlStr);
                }
                *//*sql文件完成 开始复制wwwroot文件夹、ssl-file文件夹(若有)到BACK-COOLNETBLOG文件夹下*//*
                PathProvider.CopyDir(_webHostEnvironment.WebRootPath, Path.Combine(tmpBackDataDownDir, "wwwroot"));
                PathProvider.CopyDir(Path.Combine(_webHostEnvironment.ContentRootPath, "ssl-file"), Path.Combine(tmpBackDataDownDir, "ssl-file"));
            }
            catch (Exception e)
            {
                result.Code = ValueCodes.Error;
                result.TipMessage = "执行失败，请重试。";
                result.HideMessage = "文件io执行失败：" + e.Message;
                return Json(result);
            }

            try
            {
                /* 开始压缩BACK-COOLNETBLOG文件夹 生成压缩文件于wwwroot/BACK-COOLNETBLOG/下*/
                var zipDir = Path.Combine(_webHostEnvironment.WebRootPath, "BACK-COOLNETBLOG");
                Directory.CreateDirectory(zipDir);
                var zipPath = Path.Combine(zipDir, ValueCompute.Guid16().Replace("-", "") + "-" + 
                    DateTime.Now.ToString("yyyyMMdd") + "-back.zip");
                // 生成zip压缩文件（注：压缩文件的父目录必须事先存在）
                ZipFile.CreateFromDirectory(tmpBackDataDownDir, zipPath);
                // 删除原app目录下的BACK-COOLNETBLOG
                Directory.Delete(tmpBackDataDownDir, true);
            }
            catch (Exception e)
            {
                result.Code = ValueCodes.Error;
                result.TipMessage = "执行失败，请重试。";
                result.HideMessage = "文件压缩生成执行失败：" + e.Message;
                return Json(result);
            }
            result.Code = ValueCodes.Success;
            result.TipMessage = "数据备份执行成功。";
            return Json(result);
        }
    }
}
