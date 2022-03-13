using CoolNetBlog.Bll;
using CoolNetBlog.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CoolNetBlog.Controllers
{
    /// <summary>
    /// 评论处理控制接口
    /// </summary>
    public class CommentController : Controller
    {
        private readonly CommentBll _commentBllBll;

        public CommentController()
        {
            _commentBllBll = new CommentBll();
        }

        /// <summary>
        /// 发布评论 处理接口
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Route("{Controller}/Comment")]
        [HttpPost]
        public async Task<IActionResult> CommentAsync([FromBody] CommentViewModel data)
        {
            var res =await _commentBllBll.DealCommentPostAsync(data);
            return new JsonResult(res);
        }
    }

}
