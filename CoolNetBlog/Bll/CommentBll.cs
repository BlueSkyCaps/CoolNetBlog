using CommonObject.Constructs;
using CommonObject.Enums;
using CommonObject.Methods;
using ComponentsServices.Base;
using CoolNetBlog.Models;
using CoolNetBlog.ViewModels;

namespace CoolNetBlog.Bll
{
    public class CommentBll
    {
        private readonly BaseSugar _baseSugar;
        private readonly SugarDataBaseStorage<Article, int> _articleSet;
        private readonly SugarDataBaseStorage<Comment, int> _commentSet;
        private readonly SugarDataBaseStorage<Reply, int> _replySet;
        private ValueResult _result;
        
        public CommentBll()
        {
            _result = new()
            {
                Code = ValueCodes.UnKnow
            };
            _baseSugar = new BaseSugar();
            _articleSet = new SugarDataBaseStorage<Article, int>(_baseSugar._dbHandler);
            _commentSet = new SugarDataBaseStorage<Comment, int>(_baseSugar._dbHandler);
            _replySet = new SugarDataBaseStorage<Reply, int>(_baseSugar._dbHandler);

        }

        public async Task<ValueResult> DealCommentPostAsync(CommentViewModel data, HttpContext httpContext) { 
            if (data.SourceType<=0||data.SourceId<=0)
            {
                _result.Code = ValueCodes.Error;
                _result.HideMessage = "评论SourceType|SourceId不是有效值";
                _result.TipMessage = "发表评论失败了，可以刷新再试试哦..";
                return _result;
            }
            Article? commentAte = null;
            // SourceType == 1 文章评论。
            // 不显示指明是文章评论，而是用SourceType判断，这是为了兼容之后的代码，因为评论不一定只能在文章模块
            if (data.SourceType == 1 && data.SourceId > 0)
            {
                if (string.IsNullOrWhiteSpace(data.Name)|| string.IsNullOrWhiteSpace(data.Email) || 
                    string.IsNullOrWhiteSpace(data.Content))
                {
                    _result.HideMessage = "没有填写完整";
                    _result.TipMessage = "请输入必要内容哦..";
                    return _result;
                }

                if (data.Name.Length>12 || data.Email.Length>25 ||  data.Content.Length>150)
                {
                    _result.HideMessage = "长度超出";
                    _result.TipMessage = "字数超出了规定长度..";
                    return _result;
                }
                if (!string.IsNullOrWhiteSpace(data.SiteUrl))
                {
                    bool passUri = PathProvider.TryCreateUrl(data.SiteUrl);
                    if (!passUri)
                    {
                        _result.HideMessage = "输入了网址uri但无效";
                        _result.TipMessage = "不是合格的网址哦..";
                        return _result;
                    }
                }

                commentAte = await _articleSet.FindOneByIdAsync(data.SourceId);
                if (commentAte is null)
                {
                    _result.HideMessage = "不存在此文章Id";
                    _result.TipMessage = "该文章已经消失了，可以刷新看看哦..";
                    return _result;
                }
                if (commentAte.CommentType == 3)
                {
                    _result.Code = ValueCodes.UnKnow;
                    _result.HideMessage = "不允许评论的内容尝试进行评论";
                    _result.TipMessage = "内容已被设置为不允许评论~";
                    return _result;
                }
            }
            var cip = httpContext.Connection.RemoteIpAddress?.ToString()??"";
            if (string.IsNullOrWhiteSpace(cip))
            {
                _result.HideMessage = "新增评论,获取不到客户端ip";
                _result.TipMessage = "评论失败了呢，可以重新试试。";
                return _result;
            }

            if (!await LeaveMessageLimitViaAsync(cip))
            {
                return _result;
            }


            Comment insertComment = new Comment
            {
                CommentTime = DateTime.Now,
                Email = data.Email.Trim(),
                Name = data.Name.Trim(),
                SiteUrl = data.SiteUrl?.Trim(),
                Content = data.Content.Trim(),
                SourceId = data.SourceId,
                SourceType = data.SourceType,
                ClientIp = cip,
            };

            if (await _baseSugar._dbHandler.Queryable<AdminUser>().AnyAsync(a => a.AccountName.ToLower() == insertComment.Name.ToLower()))
            {
                _result.Code = ValueCodes.UnKnow;
                _result.HideMessage = "昵称是管理员使用昵称";
                _result.TipMessage = "哈,该昵称被创造者'霸占'了哦~";
                return _result;
            }

            // SourceType == 1 文章评论
            if (data.SourceType==1)
            {
                // 要评论的文章若是设置需要审核(CommentType为2) IsPassed就是false，否则公开评论、不设置评论直接true
                insertComment.IsPassed = commentAte?.CommentType == 2?false:true;
            }
            _commentSet.TransBegin();
            try
            {
                var e = await _commentSet.InsertAsync(insertComment);
                _commentSet.TransCommit();
            }
            catch (Exception e)
            {
                _commentSet.TransRoll();
                _result.Code = ValueCodes.Error;
                _result.HideMessage = "插入评论数据时引发异常:"+e.Message+" "+e.StackTrace;
                _result.TipMessage = "评论发表失败了，你可以再试一下?!";
            }
            _result.Code = ValueCodes.Success;
            _result.TipMessage = commentAte?.CommentType == 2? "经过审核后将会显示评论,感谢您的畅所欲言~" : "感谢您的畅所欲言~";
            return _result;
        }

        public async Task<ValueResult> DealReplyPostAsync(ReplyViewModel data, HttpContext httpContext)
        {

            if (data.CommentId <= 0 )
            {
                _result.Code = ValueCodes.Error;
                _result.HideMessage = "回复CommentId不是有效值";
                _result.TipMessage = "回复失败了，可以刷新再试试哦..";
                return _result;
            }
            Article? commentAte = null;
            Comment? cmt = null;
            
            if (string.IsNullOrWhiteSpace(data.Name) || string.IsNullOrWhiteSpace(data.Email) ||
                string.IsNullOrWhiteSpace(data.Content))
            {
                _result.HideMessage = "没有填写完整";
                _result.TipMessage = "请输入必要内容哦..";
                return _result;
            }

            if (data.Name.Length > 12 || data.Email.Length > 25 || data.Content.Length > 150)
            {
                _result.HideMessage = "长度超出";
                _result.TipMessage = "字数超出了规定长度..";
                return _result;
            }
            if (!string.IsNullOrWhiteSpace(data.SiteUrl))
            {
                bool passUri = PathProvider.TryCreateUrl(data.SiteUrl);
                if (!passUri)
                {
                    _result.HideMessage = "输入了网址uri但无效";
                    _result.TipMessage = "不是合格的网址哦..";
                    return _result;
                }
            }
            // 获取此回复对应评论实体
            cmt = await _commentSet.FindOneByIdAsync(data.CommentId);
            if (cmt is null)
            {
                _result.HideMessage = "不存在此评论Id";
                _result.TipMessage = "评论已经消失了，可以刷新看看哦..";
                return _result;
            }
            // 通过评论实体获取对应的文章实体
            commentAte = await _articleSet.FindOneByIdAsync(cmt.SourceId);
            if (commentAte.CommentType == 3)
            {
                _result.Code = ValueCodes.UnKnow;
                _result.HideMessage = "不允许评论的内容尝试进行评论";
                _result.TipMessage = "内容已被设置为不允许评论~";
                return _result;
            }
            var cip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(cip))
            {
                _result.HideMessage = "新增回复,获取不到客户端ip";
                _result.TipMessage = "回复失败了呢，可以重新试试。";
                return _result;
            }

            if (!await LeaveMessageLimitViaAsync(cip))
            {
                return _result;
            }

            Reply insertReply = new Reply
            {
                ReplyTime = DateTime.Now,
                Email = data.Email,
                Name = data.Name,
                SiteUrl = data.SiteUrl?.Trim(),
                Content = data.Content,
                CommentId = data.CommentId,
                ClientIp = cip,
                
            };
            if (await _baseSugar._dbHandler.Queryable<AdminUser>().AnyAsync(a => a.AccountName.ToLower() == insertReply.Name.ToLower()))
            {
                _result.Code = ValueCodes.UnKnow;
                _result.HideMessage = "昵称是管理员使用昵称";
                _result.TipMessage = "哈,该昵称被创造者'霸占'了哦~";
                return _result;
            }

            if (cmt.SourceType == 1)
            {
                // 要回复的评论的文章若是设置需要审核(CommentType为2) IsPassed就是false，否则公开评论直接true
                insertReply.IsPassed = commentAte?.CommentType == 2 ? false : true;
            }
            _replySet.TransBegin();
            try
            {
                var e = await _replySet.InsertAsync(insertReply);
                _replySet.TransCommit();
            }
            catch (Exception e)
            {
                _replySet.TransRoll();
                _result.Code = ValueCodes.Error;
                _result.HideMessage = "插入回复数据时引发异常:" + e.Message + " " + e.StackTrace;
                _result.TipMessage = "回复失败了，可以再试一下?!";
            }
            _result.Code = ValueCodes.Success;
            _result.TipMessage = commentAte?.CommentType == 2 ? "审核后将显示,感谢发言~" : "感谢畅所欲言~";
            return _result;
        }

        public async Task<ValueResult> GetArticleCommentsAsync(int sourceId, int index)
        {
            SugarDataBaseStorage<CommentViewModel, int> commentVmReader= new SugarDataBaseStorage<CommentViewModel, int>(_baseSugar._dbHandler);
            SugarDataBaseStorage<ReplyViewModel, int> replyVmReader = new SugarDataBaseStorage<ReplyViewModel, int>(_baseSugar._dbHandler);

            Article? commentAte = null;
            commentAte = await _articleSet.FindOneByIdAsync(sourceId);
            if (commentAte is null)
            {
                _result.HideMessage = $"获取文章id为{sourceId}的评论时失败，不存在此文章Id";
                _result.TipMessage = "加载评论失败了，刷新在试试吧?!";
                return _result;
            }
            try
            {
                // 最新评论在前 当前页取10个
                var commentsVm = await commentVmReader.GetListBuilder().Where(c=>c.SourceType==1&&c.SourceId==sourceId&&c.IsPassed==true)
                    .OrderBy(c=>c.CommentTime, SqlSugar.OrderByType.Desc)
                    .Skip((index - 1)*10).Take(10).ToListAsync();
                // 获取当前页每个评论的回复
                foreach (var c in commentsVm)
                {
                    // 按回复时间从早到新排序 只取当前评论最前的5个回复 因为评论默认都显示第一页的回复
                    var queryable = replyVmReader.GetListBuilder().Where(r => r.CommentId == c.Id && r.IsPassed == true)
                    .OrderBy(c => c.ReplyTime, SqlSugar.OrderByType.Asc);
                    c.RelatedReplies = await queryable.Take(5).ToListAsync();
                    // 计算是否不止5个
                    c.HasReplyInNext = (await queryable.CountAsync())>5;
                }
                _result.Data = commentsVm;
                _result.Code = ValueCodes.Success;
            }
            catch (Exception e)
            {
                _result.Code = ValueCodes.Error;
                _result.HideMessage = $"获取文章id为{sourceId}的评论时失败，引发异常:{e.Message} {e.StackTrace}";
                _result.TipMessage = "加载评论失败了，刷新在试试吧?!";
            }
            return _result;
        }

        public async Task<ValueResult> GetCommentReplysAsync(int commentId, int index)
        {
            SugarDataBaseStorage<ReplyViewModel, int> replyVmReader = new SugarDataBaseStorage<ReplyViewModel, int>(_baseSugar._dbHandler);

            Comment? comment = null;
            comment = await _commentSet.FindOneByIdAsync(commentId);
            if (comment is null)
            {
                _result.HideMessage = $"获取评论id为{commentId}的评论失败，不存在此评论Id";
                _result.TipMessage = "评论已经消失了，可以刷新看看哦..";
                return _result;
            }
            try
            {
                // 最早回复在前 每次取5个
                var queryable = replyVmReader.GetListBuilder().Where(c => c.CommentId == commentId && c.IsPassed == true)
                    .OrderBy(c => c.ReplyTime, SqlSugar.OrderByType.Asc);
                var replies = await queryable.Skip((index - 1) * 5).Take(5).ToListAsync();
                // 计算是否总数大于当前页*条数
                var HasReplyInNext = (await queryable.CountAsync()) > index * 5;
                _result.Data = new { Replies = replies, HasReplyInNext };
                _result.Code = ValueCodes.Success;
            }
            catch (Exception e)
            {
                _result.Code = ValueCodes.Error;
                _result.HideMessage = $"获取评论id为{commentId}的评论时失败，引发异常:{e.Message} {e.StackTrace}";
                _result.TipMessage = "获取回复失败了，刷新在试试吧?!";
            }
            return _result;
        }


        /// <summary>
        /// 当前评论回复的ip是否已超过当日留言限制次数
        /// </summary>
        /// <returns></returns>
        private async Task<bool> LeaveMessageLimitViaAsync(string cip)
        {
            SugarDataBaseStorage<SiteSetting, int> sittSet = new SugarDataBaseStorage<SiteSetting, int>(_baseSugar._dbHandler);
            var theSitt = await sittSet.FirstOrDefaultAsync(s => 1 == 1);
            if (theSitt is null)
            {
                _result.Code = ValueCodes.Error;
                _result.HideMessage = "新增评论,获取不到SiteSetting表";
                _result.TipMessage = "评论失败了呢，可以重新试试。";
                return false;
            }
            int? lmt = (await sittSet.FirstOrDefaultAsync(s => 1 == 1))?.LeaveLimitCount;
            if (!(lmt == null || lmt <= 0))
            {
                var currentBeginDay = DateTime.Now.Date;//当前日期的零点开头 1998-10-15 00:00:00
                var currentNextDay = DateTime.Now.Date.AddDays(1);//当前日期的下一天零点开头 1998-10-16 00:00:00
                // 当前ip此时日的回复+评论数是否超过限制数
                var theCmtCount = await _commentSet.GetListBuilder().Where(c => c.ClientIp == cip)
                    .Where(c => c.CommentTime >= currentBeginDay)
                    .Where(c => c.CommentTime <= currentNextDay).CountAsync();
                var theRpCount = await _replySet.GetListBuilder().Where(r => r.ClientIp == cip)
                    .Where(r => r.ReplyTime >= currentBeginDay)
                    .Where(r => r.ReplyTime <= currentNextDay).CountAsync();
                if ((theCmtCount+ theRpCount) > lmt)
                {
                    _result.HideMessage = "新增评论,此ip超过当日限制次数";
                    _result.TipMessage = "你当日发言次数超过限制，第二天再来哦~";
                    return false;
                }
            }
            return true;
        }

           
    }
}
