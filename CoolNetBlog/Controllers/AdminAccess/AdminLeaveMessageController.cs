using CommonObject.Classes;
using CommonObject.Enums;
using ComponentsServices.Base;
using CoolNetBlog.Base;
using CoolNetBlog.Models;
using CoolNetBlog.ViewModels;
using CoolNetBlog.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

namespace CoolNetBlog.Controllers.Admin
{

    /// <summary>
    /// 后台留言管理操作
    /// </summary>

    public class AdminLeaveMessageController : BaseAdminController
    {
        private static LeaveMessageViewModel slvm = new();
        private BaseSugar _baseSugar;
        private SugarDataBaseStorage<Article, int> _articleReader;
        private SugarDataBaseStorage<CommentCarryViewModel, int> _commentVmReader;
        private SugarDataBaseStorage<ReplyCarryViewModel, int> _replyVmReader;

        public AdminLeaveMessageController():base()
        {
            _baseSugar = new BaseSugar();
            _articleReader = new SugarDataBaseStorage<Article, int>(_baseSugar._dbHandler);
            _commentVmReader = new SugarDataBaseStorage<CommentCarryViewModel, int>(_baseSugar._dbHandler);
            _replyVmReader = new SugarDataBaseStorage<ReplyCarryViewModel, int>(_baseSugar._dbHandler);
        }

        /// <summary>
        /// 根据Id删除评论或回复
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dType">1评论，2回复</param>
        /// <param name="dType">是否发生邮件通知</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteOneMsg([FromBody]DeleteOneMsgViewModel vm) {
            ValueResult result = new ValueResult { Code = ValueCodes.UnKnow };
            if (vm.DeleteId < 1)
            {
                result.HideMessage = "删除评论或回复失败：id无效";
                result.TipMessage = "删除评论或回复失败，请刷新重试";
                return Json(result);
            }
            if (vm.SendEmail&&string.IsNullOrWhiteSpace(vm.EmailMessage))
            {
                result.HideMessage = "删除评论或回复附带邮件但邮件信息未填";
                result.TipMessage = "附带邮件请填写邮件内容";
                return Json(result);
            }
            _baseSugar._dbHandler.BeginTran();
            try
            {
                if (vm.DType==1)
                {
                    var deleable = await _baseSugar._dbHandler.Queryable<Comment>().SingleAsync(c => c.Id == vm.DeleteId);
                    await _baseSugar._dbHandler.Deleteable<Comment>().Where(deleable).ExecuteCommandAsync();
                    // 删除此评论还要删除它的所有回复
                    await _baseSugar._dbHandler.Deleteable<Reply>().Where(r=>r.CommentId== vm.DeleteId).ExecuteCommandAsync();
                }
                else
                {
                    // 删除回复
                    var deleable = await _baseSugar._dbHandler.Queryable<Reply>().SingleAsync(c => c.Id == vm.DeleteId);
                    await _baseSugar._dbHandler.Deleteable<Reply>().Where(deleable).ExecuteCommandAsync();
                }
                _baseSugar._dbHandler.CommitTran();
            }
            catch (Exception e)
            {
                _baseSugar._dbHandler.RollbackTran();
                result.Code = ValueCodes.Error;
                result.HideMessage = $"删除某{(vm.DType == 1? "评论" : "回复")}失败:" + e.Message;
                result.TipMessage = "删除失败请刷新重试";
                return Json(result);
            }
            // 处理发送邮件提醒
            if (vm.SendEmail)
            {
                try
                {
                    // send email
                    result.Code = ValueCodes.Success;
                }
                catch (Exception e)
                {
                    result.HideMessage = $"删除某{(vm.DType == 1 ? "评论" : "回复")}成功，尝试发送邮件异常:" + e.Message;
                    result.TipMessage = "已被删除，但邮件发送失败";
                }

            }
            return Json(result);
        }

        /// <summary>
        /// 管理员回复某评论
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AdminReply(ReplyViewModel vm)
        {
            return RedirectToAction("LeaveMessageAmManagement", "AdminLeaveMessage", new { pt = slvm.PassToken });
        }

        /// <summary>
        /// 留言管理页面入口 索引第一页 默认返回未审核的评论和回复列表
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> LeaveMessageAmManagement(string? pt)
        {
            // 清空列表。 因为是静态模型数据 刷新会追加回复列表
            slvm.NotPassComments?.Clear();
            slvm.NotPassReplies?.Clear();
            // 默认返回未审核的评论和回复列表 30条
            slvm.NotPassComments = await _commentVmReader.GetListBuilder().Where(c=>c.IsPassed==false||c.IsPassed==null)
                .OrderBy(c=>c.CommentTime, SqlSugar.OrderByType.Desc).Take(5).ToListAsync();


            var notPassRepliesTmp = await _replyVmReader.GetListBuilder().Where(r => r.IsPassed == false || r.IsPassed == null)
                .OrderBy(r => r.ReplyTime, SqlSugar.OrderByType.Desc).Take(5).ToListAsync();
            foreach (var cmv in slvm.NotPassComments)
            {
                // 获取评论所在文章
                if (cmv.SourceType==1)
                {
                    cmv.RelatedArticle = await _articleReader.FindOneByIdAsync(cmv.SourceId);
                    cmv.RelatedArticleUrl = "/Detail?articleId="+ cmv.SourceId;
                }
            }

            foreach (var replyTmp in notPassRepliesTmp)
            {
                // 获取当前此条回复对应的评论
                replyTmp.RelatedComment = await _commentVmReader.FindOneByIdAsync(replyTmp.CommentId);
                // 获取回复所在文章
                if (replyTmp.RelatedComment.SourceType == 1)
                {
                    replyTmp.RelatedArticle = await _articleReader.FindOneByIdAsync(replyTmp.RelatedComment.SourceId);
                    replyTmp.RelatedArticleUrl = "/Detail?articleId=" + replyTmp.RelatedComment.SourceId;
                }
                slvm.NotPassReplies?.Add(replyTmp);
            }
            // 自动封装已有的数据
            slvm = (LeaveMessageViewModel)WrapMustNeedPassFields(slvm);
            return View(slvm);
        }


        /// <summary>
        /// 获取未审核的某页评论列表
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetNotPassComments(string? pt, int index)
        {
            ValueResult result = new ValueResult { Code = ValueCodes.UnKnow };
            try
            {
                var notPassComments = await _commentVmReader.GetListBuilder().Where(c => c.IsPassed == false || c.IsPassed == null)
                    .OrderBy(c => c.CommentTime, SqlSugar.OrderByType.Desc).Skip((index-1)*5).Take(5).ToListAsync();
                foreach (var cmv in notPassComments)
                {
                    // 获取评论所在文章
                    if (cmv.SourceType == 1)
                    {
                        cmv.RelatedArticle = await _articleReader.FindOneByIdAsync(cmv.SourceId);
                        cmv.RelatedArticleUrl = "/Detail?articleId=" + cmv.SourceId;
                    }
                }
                result.Code = ValueCodes.Success;
                result.Data = new { NotPassComments= notPassComments };

            }
            catch (Exception e)
            {
                result.Code = ValueCodes.Error;
                result.HideMessage = "获取未审核的某页评论列表发生异常:"+e.Message;
                result.TipMessage = "加载数据失败请重试";
            }
            return Json(result);    
        }

        /// <summary>
        /// 获取未审核的某页回复列表
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetNotPassReplies(string? pt, int index)
        {
            ValueResult result = new ValueResult { Code = ValueCodes.UnKnow };
            try
            {
                var notPassRepliesTmp = await _replyVmReader.GetListBuilder().Where(r => r.IsPassed == false || r.IsPassed == null)
                    .OrderBy(r => r.ReplyTime, SqlSugar.OrderByType.Desc).Skip((index - 1) * 5).Take(5).ToListAsync();
                List<ReplyCarryViewModel> notPassReplies = new List<ReplyCarryViewModel>();
                foreach (var replyTmp in notPassRepliesTmp)
                {
                    // 获取当前此条回复所属的评论
                    replyTmp.RelatedComment = await _commentVmReader.FindOneByIdAsync(replyTmp.CommentId);
                    // 获取回复所在文章
                    if (replyTmp.RelatedComment.SourceType == 1)
                    {
                        replyTmp.RelatedArticle = await _articleReader.FindOneByIdAsync(replyTmp.RelatedComment.SourceId);
                        replyTmp.RelatedArticleUrl = "/Detail?articleId=" + replyTmp.RelatedComment.SourceId;
                    }
                    notPassReplies.Add(replyTmp);
                }
                result.Code = ValueCodes.Success;
                result.Data = new { NotPassReplies = notPassReplies };
            }
            catch (Exception e)
            {
                result.Code = ValueCodes.Error;
                result.HideMessage = "获取未审核的某页评回复列表发生异常:" + e.Message;
                result.TipMessage = "加载数据失败请重试";
            }
            return Json(result);
        }

        /// <summary>
        /// 根据关键词删除匹配的评论
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> UseWordDelete(string? pt,int type,  string? kw)
        {
            IList<Article> articles;
            if (!string.IsNullOrWhiteSpace(kw))
            {
                articles = await _articleReader.GetListBuilder().Where(a =>
                    (a.Id.ToString() == kw) ||
                    (a.Title != null && a.Title.Contains(kw)) ||
                    (a.Labels != null && a.Labels.Contains(kw)) ||
                    (a.Abstract != null && a.Abstract.Contains(kw)))
                    .OrderBy(a => a.UpdateTime, SqlSugar.OrderByType.Desc).Take(20).ToListAsync();
            }
            else
            {
                articles = await _articleReader.GetTopCountListAsync(20, a => a.UpdateTime, SqlSugar.OrderByType.Desc);
            }

            // 自动封装已有的数据
            slvm = (LeaveMessageViewModel)WrapMustNeedPassFields(slvm);
            return View(slvm);
        }

        /// <summary>
        /// 移除有些场景下表单不需要验证的属性
        /// </summary>
        private void RemoveSomeValid()
        {
            ModelState.Remove("AccountName");
            ModelState.Remove("Name");
        }
    }
}
