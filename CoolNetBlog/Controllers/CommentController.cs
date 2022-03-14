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
        [HttpPost]
        public async Task<IActionResult> Comment([FromBody] CommentViewModel data)
        {
            var res =await _commentBllBll.DealCommentPostAsync(data);
            return new JsonResult(res);
        }

        /// <summary>
        /// 评论回复 处理接口
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Reply([FromBody] ReplyViewModel data)
        {
            var res = await _commentBllBll.DealReplyPostAsync(data);
            return new JsonResult(res);
        }

        /// <summary>
        /// 获取文章的评论数据
        /// </summary>
        /// <param name="sourceId">文章id</param>
        /// <param name="index">当前加载评论的索引(次数)，默认为1，开头，每点击一次“加载评论”按钮，前端传递数据+1</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetArticleComments(int sourceId, int index=1)
        {
            var res = await _commentBllBll.GetArticleCommentsAsync(sourceId, index);
            return new JsonResult(res);
        }
    }

}
