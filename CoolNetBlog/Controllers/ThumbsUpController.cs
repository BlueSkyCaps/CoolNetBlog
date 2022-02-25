using CoolNetBlog.Bll;
using Microsoft.AspNetCore.Mvc;

namespace CoolNetBlog.Controllers
{
    /// <summary>
    /// 文章点赞控制
    /// </summary>
    public class ThumbsUpController : Controller
    {
        private readonly ThumbsUpBll _thumbsUpBll;
        public ThumbsUpController()
        {
            _thumbsUpBll = new ThumbsUpBll();
        }

        [Route("{Controller}/ThumbsUp")]
        [HttpPost]
        public async Task<IActionResult> ThumbsUpAsync([FromForm] int articleId)
        {
            var res =await _thumbsUpBll.DealThumbsUpArticleAsync(articleId, HttpContext);
            return new JsonResult(res);
        }
    }
    public class V
    {
        public int ArticleId { get; set; } 
    }
}
