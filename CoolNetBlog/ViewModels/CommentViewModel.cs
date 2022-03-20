using CoolNetBlog.Models;
using SqlSugar;

namespace CoolNetBlog.ViewModels
{
    /// <summary>
    /// 评论视图模型
    /// </summary>
    public class CommentViewModel:Comment
    {
        /// <summary>
        /// 当前评论关联的回复
        /// </summary>
        [SugarColumn(IsIgnore =true)]
        public IList<ReplyViewModel> RelatedReplies { get; set; } = new List<ReplyViewModel>();

        /// <summary>
        /// 当前评论关联的回复是否还有回复数据在下一页
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public bool HasReplyInNext { get; set; } = false;
    }
}
