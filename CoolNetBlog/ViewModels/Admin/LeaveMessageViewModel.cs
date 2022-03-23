using CoolNetBlog.Models;

namespace CoolNetBlog.ViewModels.Admin
{
    /// <summary>
    /// 留言管理(评论、回复)列表查询模型视图类
    /// </summary>
    public class LeaveMessageViewModel : PassBaseViewModel
    {
        /// <summary>
        /// 当前未审核的评论
        /// </summary>
        public IList<CommentCarryViewModel>? NotPassComments { get; set; } = new List<CommentCarryViewModel>();
        /// <summary>
        /// 当前未审核的回复
        /// </summary>
        public IList<ReplyCarryViewModel>? NotPassReplies { get; set; } = new List<ReplyCarryViewModel>();

    }

    public class CommentCarryViewModel : Comment
    {
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public Article? RelatedArticle { get; set; } = new Article();

        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string? RelatedArticleUrl { get; set; } = "";

    }
    public class ReplyCarryViewModel:Reply
    {
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public Article? RelatedArticle { get; set; } = new Article();
        
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string? RelatedArticleUrl { get; set; }="";
        
        [SqlSugar.SugarColumn(IsIgnore =true)]
        public Comment? RelatedComment { get; set; } = new Comment();
    }

    /// <summary>
    /// 留言管理(评论、回复) 删除某个评论或回复
    /// </summary>
    public class DeleteOneMsgViewModel
    {
        //↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ post表单属性----
        public int DeleteId { get; set; }
        public int DType { get; set; }
        public bool SendEmail { get; set; }
        public string EmailMessage { get; set; }
        // ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑
    }

    /// <summary>
    /// 留言管理(评论、回复) 审核公开某个评论或回复
    /// </summary>
    public class PassOneMsgViewModel
    {
        //↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ post表单属性----
        public int PassId { get; set; }
        public int DType { get; set; }
        public bool SupplyReply { get; set; }
        public bool SendEmail { get; set; }
        public string Message { get; set; }
        // ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑
    }
}
