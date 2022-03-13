using CoolNetBlog.Bll;
using Microsoft.AspNetCore.Mvc;

namespace CoolNetBlog.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class CommentController : Controller
    {
        private readonly CommentBll _commentBllBll;

        public CommentController()
        {
            _commentBllBll = new CommentBll();
        }

        [Route("{Controller}/Comment")]
        [HttpPost]
        public async Task<IActionResult> CommentAsync([FromForm] int articleId)
        {
            var res =await _commentBllBll.DealThumbsUpArticleAsync(articleId, type, HttpContext);
            return new JsonResult(res);
        }
    }

}
