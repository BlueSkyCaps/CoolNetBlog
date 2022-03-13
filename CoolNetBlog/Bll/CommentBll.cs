using CommonObject.Classes;
using CommonObject.Enums;
using ComponentsServices.Base;
using CoolNetBlog.Models;
using CoolNetBlog.ViewModels;

namespace CoolNetBlog.Bll
{
    public class CommentBll
    {
        private readonly SugarDataBaseStorage<Article, int> _articleSet;
        private readonly SugarDataBaseStorage<Comment, int> _commentSet;
        public CommentBll()
        {
            _articleSet = new SugarDataBaseStorage<Article, int>();
            _commentSet = new SugarDataBaseStorage<Comment, int>();
        }

        public async Task<ValueResult> DealCommentPostAsync(CommentViewModel data) { 
            ValueResult result = new()
            { 
                 Code = ValueCodes.UnKnow
            };
            if (data.SourceType<=0||data.SourceId<=0)
            {
                result.Code = ValueCodes.Error;
                result.HideMessage = "SourceType|SourceId不是有效值";
                result.TipMessage = "发表评论失败了，你可以刷新再试试哦..";
                return result;
            }
            Article? commentAte = null;
            // SourceType == 1 文章评论。
            // 不显示指明是文章评论，而是用SourceType判断，这是为了兼容之后的代码，因为评论不一定只能在文章模块
            if (data.SourceType == 1 && data.SourceId > 0)
            {
                if (string.IsNullOrWhiteSpace(data.Name)|| string.IsNullOrWhiteSpace(data.Email) || 
                    string.IsNullOrWhiteSpace(data.Content))
                {
                    result.HideMessage = "没有填写完整";
                    result.TipMessage = "请输入必要内容哦..";
                    return result;
                }
                commentAte = _articleSet.FindOneById(data.SourceId);
                if (commentAte is null)
                {
                    result.HideMessage = "不存在此文章Id";
                    result.TipMessage = "该文章已经被删了，你可以刷新看看哦..";
                    return result;
                }
            }
            Comment insertComment = new Comment
            {
                CommentTime = DateTime.Now,
                Email = data.Email,
                Name = data.Name,
                Content = data.Content,
                SourceId = data.SourceId,
                SourceType = data.SourceType,
            };
            // SourceType == 1 文章评论
            if (data.SourceType==1)
            {
                // 要评论的文章若是设置需要审核(CommentType为2) IsPassed就是false，否则公开评论、不设置评论直接true
                insertComment.IsPassed = commentAte?.CommentType == 2?false:true;
            }
            _commentSet.TransBegin();
            try
            {
                await _commentSet.InsertAsync(insertComment);
            }
            catch (Exception e)
            {
                _commentSet.TransRoll();
                result.Code = ValueCodes.Error;
                result.HideMessage = "插入评论数据时是引发异常:"+e.Message+" "+e.StackTrace;
                result.TipMessage = "评论发表失败了，你可以再试一下?!";
            }
            result.Code = ValueCodes.Success;
            result.TipMessage = "感谢你的畅所欲言~";
            return result;
        }
    }
}
