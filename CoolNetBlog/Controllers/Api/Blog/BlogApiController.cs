using ComponentsServices.Base;
using CoolNetBlog.Models;
using CoolNetBlog.ViewModels.Api;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoolNetBlog.Controllers.Api.Blog
{
    public class BlogApiController : Controller
    {
        /// <summary>
        /// 隐私文章解锁接口
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ArticleUnLock([FromBody] ArticleUnLockViewModel data)
        {
            try
            {
                data.Code = 0;
                data.Content = "";
                SugarDataBaseStorage<Article, int> articleSet = new SugarDataBaseStorage<Article, int>();
                var article = articleSet.FindOneById(data.ArticleId);
                if (article is null)
                {
                    return Json(data);
                }
                // 若已经不是隐私文章或密码正确
                if ((!article.IsLock)|| data.Password == article.LockPassword)
                {
                    data.Content = article.Content;
                    data.Code=1;
                    return Json(data); 
                }
               
                return Json(data);

            }
            catch (Exception)
            {
                return Json(data);
            }

        }
    }
}
