using ComponentsServices.Base;
using CoolNetBlog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace CoolNetBlog.Base
{
    public class AdminEnterFilter : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        /// <summary>
        /// 权限过滤器，后台界面(不包括后台登陆入口)都需要判断权限
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            List<string> adminExpCtrName = new List<string> { 
                "admin","adminmenu", "adminarticle", "adminsitesetting", "adminfile", "adminlovelook" };

            var controllerName = ((ControllerBase)context.Controller).ControllerContext.ActionDescriptor.ControllerName;
            var actionName = ((ControllerBase)context.Controller).ControllerContext.ActionDescriptor.ActionName;
            if (adminExpCtrName.Contains(controllerName.ToLower()) && (actionName.ToLower()!="login" && actionName.ToLower() != "reset"))
            {

                StringValues pt;
                context.HttpContext.Request.Cookies.TryGetValue("coolnetblogadminloginxiyuaneightfourone", out string? cv);
                context.HttpContext.Request.Query.TryGetValue("pt", out pt);
                //query是空,尝试从form表单中找，因为更新删除是post
                if (String.IsNullOrWhiteSpace(pt.FirstOrDefault()))
                {
                    try
                    {
                        // 若get方式没有 而强制尝试从form中取，当前请求本是get不是post 就会取不到 捕获异常返回验证失败
                        context.HttpContext.Request.Form.TryGetValue("PassToken", out pt);
                        if (string.IsNullOrWhiteSpace(pt.FirstOrDefault()))
                        {
                            context.HttpContext.Request.Form.TryGetValue("pt", out pt);
                        }
                    }
                    catch (Exception)
                    {
                        context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
                    }
                }
                if (string.IsNullOrWhiteSpace(cv) || cv != BaseAdminController._currentCookieValue)
                    context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);

                var adminUserSet = new SugarDataBaseStorage<AdminUser, int>();
                var au = adminUserSet.FirstOrDefault(a => a.Token == pt);
                if (au is null)
                    context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }
        }
    }
}
