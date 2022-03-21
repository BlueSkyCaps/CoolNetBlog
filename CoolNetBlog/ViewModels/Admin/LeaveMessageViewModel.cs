using CoolNetBlog.Models;

namespace CoolNetBlog.ViewModels.Admin
{
    /// <summary>
    /// 留言管理(评论、回复)模型视图类
    /// </summary>
    public class LeaveMessageViewModel : PassBaseViewModel
    {   
        /// <summary>
        /// 当前未审核的评论
        /// </summary>
        public IList<Comment>? NotPassComments { get; set; } = new List<Comment>();
        /// <summary>
        /// 当前未审核的回复
        /// </summary>
        public IList<ReplyCarryCmtViewModel>? NotPassReplies { get; set; } = new List<ReplyCarryCmtViewModel>();

    }
    public class ReplyCarryCmtViewModel:Reply
    {
        [SqlSugar.SugarColumn(IsIgnore =true)]
        public Comment? RelatedComment { get; set; } = new Comment();
    }
}
