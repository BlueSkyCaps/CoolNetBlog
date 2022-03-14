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
        public IList<Reply> RelatedReplies { get; set; } = new List<Reply>();
    }
}
