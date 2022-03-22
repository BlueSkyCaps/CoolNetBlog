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
        private SugarDataBaseStorage<Comment, int> _commentReader;
        private SugarDataBaseStorage<ReplyCarryCmtViewModel, int> _replyVmReader;

        public AdminLeaveMessageController():base()
        {
            _baseSugar = new BaseSugar();
            _articleReader = new SugarDataBaseStorage<Article, int>(_baseSugar._dbHandler);
            _commentReader = new SugarDataBaseStorage<Comment, int>(_baseSugar._dbHandler);
            _replyVmReader = new SugarDataBaseStorage<ReplyCarryCmtViewModel, int>(_baseSugar._dbHandler);
        }

        /// <summary>
        /// 根据Id删除评论或回复
        /// </summary>
        /// <param name="id"></param>
        /// <param name="id">1评论，2回复</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Delete([FromForm]int id, [FromForm] int type) {
            if (id<1)
            {
                ModelState.AddModelError("", "删除失败:菜单Id无效，重启浏览器再试吧？。");
                return View("MenuAmManagement", slvm);
            }
            Comment delable;
            try
            {
                delable = await _commentReader.FindOneByIdAsync(id);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "删除失败:没有此菜单Id，刷新再试吧？。");
                return View("MenuAmManagement", slvm);
            }
            bool exd = await _articleReader.AnyAsync(a=>a.MenuId==id);
            if (exd)
            {
                ModelState.AddModelError("", "删除失败:还有文章属于此菜单，你可以将文章归类为其他菜单再尝试删除;" +
                    "也可以重新命名为合适的菜单名哦！");
                return View("MenuAmManagement", slvm);
            }
            
            var ef = await _commentReader.DeleteAsync(delable);
            return RedirectToAction("LeaveMessageAmManagement", "AdminLeaveMessage", new { pt = slvm.PassToken });

        }

        /// <summary>
        /// 管理员回复某评论
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AdminReply(ReplyViewModel vm)
        {
            RemoveSomeValid();
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
            slvm.NotPassComments = await _commentReader.GetListBuilder().Where(c=>c.IsPassed==false||c.IsPassed==null)
                .OrderBy(c=>c.CommentTime, SqlSugar.OrderByType.Desc).Take(5).ToListAsync();


            var notPassRepliesTmp = await _replyVmReader.GetListBuilder().Where(r => r.IsPassed == false || r.IsPassed == null)
                .OrderBy(r => r.ReplyTime, SqlSugar.OrderByType.Desc).Take(5).ToListAsync();
            
            foreach (var replyTmp in notPassRepliesTmp)
            {
                // 获取当前此条回复对应的评论
                replyTmp.RelatedComment = await _commentReader.FindOneByIdAsync(replyTmp.CommentId);
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
                var notPassComments = await _commentReader.GetListBuilder().Where(c => c.IsPassed == false || c.IsPassed == null)
                    .OrderBy(c => c.CommentTime, SqlSugar.OrderByType.Desc).Skip((index-1)*5).Take(5).ToListAsync();
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
                List<ReplyCarryCmtViewModel> notPassReplies = new List<ReplyCarryCmtViewModel>();
                foreach (var replyTmp in notPassRepliesTmp)
                {
                    // 获取当前此条回复所属的评论
                    replyTmp.RelatedComment = await _commentReader.FindOneByIdAsync(replyTmp.CommentId);
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
        public async Task<IActionResult> GetComments(string? pt,int type,  string? kw)
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
