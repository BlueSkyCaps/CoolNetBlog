﻿using CommonObject.Classes;
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
        private SugarDataBaseStorage<Reply, int> _replySet;
        private SugarDataBaseStorage<CommentViewModel, int> _commentVmReader;
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
                result.HideMessage = "评论SourceType|SourceId不是有效值";
                result.TipMessage = "发表评论失败了，可以刷新再试试哦..";
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

                if (data.Name.Length>12 || data.Email.Length>25 ||  data.Content.Length>150)
                {
                    result.HideMessage = "长度超出";
                    result.TipMessage = "字数超出了规定长度..";
                    return result;
                }

                commentAte = await _articleSet.FindOneByIdAsync(data.SourceId);
                if (commentAte is null)
                {
                    result.HideMessage = "不存在此文章Id";
                    result.TipMessage = "该文章已经消失了，可以刷新看看哦..";
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
                var e = await _commentSet.InsertAsync(insertComment);
                _commentSet.TransCommit();
            }
            catch (Exception e)
            {
                _commentSet.TransRoll();
                result.Code = ValueCodes.Error;
                result.HideMessage = "插入评论数据时引发异常:"+e.Message+" "+e.StackTrace;
                result.TipMessage = "评论发表失败了，你可以再试一下?!";
            }
            result.Code = ValueCodes.Success;
            result.TipMessage = commentAte?.CommentType == 2? "经过审核后将会显示评论,感谢您的畅所欲言~" : "感谢您的畅所欲言~";
            return result;
        }

        public async Task<ValueResult> DealReplyPostAsync(ReplyViewModel data)
        {
            _replySet = new SugarDataBaseStorage<Reply, int>();

            ValueResult result = new()
            {
                Code = ValueCodes.UnKnow
            };
            if (data.CommentId <= 0 )
            {
                result.Code = ValueCodes.Error;
                result.HideMessage = "回复CommentId不是有效值";
                result.TipMessage = "回复失败了，可以刷新再试试哦..";
                return result;
            }
            Article? commentAte = null;
            Comment? cmt = null;
            
            if (string.IsNullOrWhiteSpace(data.Name) || string.IsNullOrWhiteSpace(data.Email) ||
                string.IsNullOrWhiteSpace(data.Content))
            {
                result.HideMessage = "没有填写完整";
                result.TipMessage = "请输入必要内容哦..";
                return result;
            }

            if (data.Name.Length > 12 || data.Email.Length > 25 || data.Content.Length > 150)
            {
                result.HideMessage = "长度超出";
                result.TipMessage = "字数超出了规定长度..";
                return result;
            }

            // 获取此回复对应评论实体
            cmt = await _commentSet.FindOneByIdAsync(data.CommentId);
            if (cmt is null)
            {
                result.HideMessage = "不存在此评论Id";
                result.TipMessage = "该评论已经消失了，可以刷新看看哦..";
                return result;
            }
            // 通过评论实体获取对应的文章实体
            commentAte = await _articleSet.FindOneByIdAsync(cmt.SourceId);

            Reply insertReply = new Reply
            {
                ReplyTime = DateTime.Now,
                Email = data.Email,
                Name = data.Name,
                Content = data.Content,
                CommentId = data.CommentId,
                
            };
            if (cmt.SourceType == 1)
            {
                // 要回复的评论的文章若是设置需要审核(CommentType为2) IsPassed就是false，否则公开评论、不设置评论直接true
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
                result.Code = ValueCodes.Error;
                result.HideMessage = "插入回复数据时引发异常:" + e.Message + " " + e.StackTrace;
                result.TipMessage = "回复失败了，你可以再试一下?!";
            }
            result.Code = ValueCodes.Success;
            result.TipMessage = commentAte?.CommentType == 2 ? "经过审核后将会显示回复,感谢您的畅所欲言~" : "感谢您的畅所欲言~";
            return result;
        }

        public async Task<ValueResult> GetArticleCommentsAsync(int sourceId, int index)
        {
            _replySet = new SugarDataBaseStorage<Reply, int>();
            _commentVmReader = new SugarDataBaseStorage<CommentViewModel, int>();

            ValueResult result = new()
            {
                Code = ValueCodes.UnKnow
            };
            Article? commentAte = null;
            commentAte = await _articleSet.FindOneByIdAsync(sourceId);
            if (commentAte is null)
            {
                result.HideMessage = $"获取文章id为{sourceId}的评论时失败，不存在此文章Id";
                result.TipMessage = "获取更多评论失败了，刷新在试试吧?!";
                return result;
            }
            try
            {
                // 最新评论在前 一次取10个
                var commentsVm = await _commentVmReader.GetListBuilder().Where(c=>c.SourceType==1&&c.SourceId==sourceId&&c.IsPassed==true)
                    .OrderBy(c=>c.CommentTime, SqlSugar.OrderByType.Desc)
                    .Skip((index - 1)*10).Take(10).ToListAsync();
                // 获取每个评论的回复
                foreach (var c in commentsVm)
                {
                    // 按回复时间从早到新排序
                    c.RelatedReplies = await _replySet.GetListBuilder().Where(r => r.CommentId == c.Id && r.IsPassed == true)
                    .OrderBy(c => c.ReplyTime, SqlSugar.OrderByType.Asc).ToListAsync();
                }
                result.Data = commentsVm;
                result.Code = ValueCodes.Success;
            }
            catch (Exception e)
            {
                result.Code = ValueCodes.Error;
                result.HideMessage = $"获取文章id为{sourceId}的评论时失败，引发异常:{e.Message} {e.StackTrace}";
                result.TipMessage = "获取此文章的评论失败了，你刷新在试试吧?!";
            }
            return result;
        }
    }
}
