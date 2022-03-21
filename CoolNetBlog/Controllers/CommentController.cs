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
        private readonly CommentBll _commentBll;

        public CommentController()
        {
            _commentBll = new CommentBll();
        }

        /// <summary>
        /// 发布评论 处理接口
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Comment([FromBody] CommentViewModel data)
        {
            var res =await _commentBll.DealCommentPostAsync(data, HttpContext);
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
            var res = await _commentBll.DealReplyPostAsync(data, HttpContext);
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
            var res = await _commentBll.GetArticleCommentsAsync(sourceId, index);
            return new JsonResult(res);
        }

        /// <summary>
        /// 获取评论的回复数据
        /// </summary>
        /// <param name="commentId">评论id</param>
        /// <param name="index">当前加载回复的索引(次数)，默认为1，开头，每点击一次“加载回复”按钮，前端传递数据+1</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCommentReplys(int commentId, int index = 1)
        {
            var res = await _commentBll.GetCommentReplysAsync(commentId, index);
            return new JsonResult(res);
        }
    }

}
