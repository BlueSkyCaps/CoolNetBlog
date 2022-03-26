using CommonObject.Constructs;
using CommonObject.Enums;
using ComponentsServices.Base;
using ComponentsServices.Mail.MailKit;
using CoolNetBlog.Base;
using CoolNetBlog.Models;
using CoolNetBlog.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

namespace CoolNetBlog.Controllers.Admin
{

    /// <summary>
    /// 后台留言(评论回复)管理操作
    /// </summary>

    public class AdminLeaveMessageController : BaseAdminController
    {
        public static LeaveMessageViewModel slvm = new();
        private BaseSugar _baseSugar;
        private SugarDataBaseStorage<Article, int> _articleReader;
        private SugarDataBaseStorage<CommentCarryViewModel, int> _commentVmReader;
        private SugarDataBaseStorage<ReplyCarryViewModel, int> _replyVmReader;
        private SugarDataBaseStorage<AdminUser, int> _adminUserReader;
        private SugarDataBaseStorage<SiteSetting, int> _siteSettingReader;

        public AdminLeaveMessageController():base()
        {
            _baseSugar = new BaseSugar();
            _articleReader = new SugarDataBaseStorage<Article, int>(_baseSugar._dbHandler);
            _commentVmReader = new SugarDataBaseStorage<CommentCarryViewModel, int>(_baseSugar._dbHandler);
            _replyVmReader = new SugarDataBaseStorage<ReplyCarryViewModel, int>(_baseSugar._dbHandler);
            _adminUserReader = new SugarDataBaseStorage<AdminUser, int>(_baseSugar._dbHandler);
            _siteSettingReader = new SugarDataBaseStorage<SiteSetting, int>(_baseSugar._dbHandler);
        }

        /// <summary>
        /// 根据Id审核通过评论或回复
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isPub">是来自已公开的评论或回复，因此不需要审核操作，而是直接回复或发送邮件，此参数兼容判断已公开的言论管理员需要回复</param>
        /// <returns></returns>
        [Route("{controller}/PassOneMsg")]
        [HttpPost]
        public async Task<IActionResult> DealOneNotPassOrPubMsg([FromBody] PassOneMsgViewModel vm, string? pt, bool isPub = false)
        {
            ValueResult result = new ValueResult { Code = ValueCodes.UnKnow };
            if (vm.Id < 1)
            {
                result.HideMessage = "DealOneNotPassOrPubMsg-操作评论或回复失败：id无效";
                result.TipMessage = "失败，请刷新重试";
                return Json(result);
            }
  
            SiteSetting siteSett = await _siteSettingReader.FirstOrDefaultAsync(s=>1==1);
            AdminUser adminUser = await _adminUserReader.FirstOrDefaultAsync(a=>a.AccountName==slvm.AccountName);
            
            if (vm.SupplyReply)
            {
                if (string.IsNullOrWhiteSpace(vm.Message))
                {
                    result.HideMessage = "DealOneNotPassOrPubMsg-操作评论或回复时附带回复信息，回复信息未填";
                    result.TipMessage = "附带回复请填写回复信息";
                    return Json(result);
                }
                if (siteSett == null)
                {
                    result.Code = ValueCodes.Error;
                    result.HideMessage = "DealOneNotPassOrPubMsg操作评论或回复时附带回复信息，SiteSetting表找不到";
                    result.TipMessage = "引发错误，请重新登录试试";
                    return Json(result);
                }
                if (string.IsNullOrWhiteSpace(siteSett.Domain))
                {
                    result.HideMessage = "DealOneNotPassOrPubMsg-操作评论或回复时附带回复信息，SiteSetting中域名没有填写需补充";
                    result.TipMessage = "请先去基本管理面板填写你的网站域名";
                    return Json(result);
                }
                if (adminUser == null)
                {
                    result.Code = ValueCodes.Error;
                    result.HideMessage = "DealOneNotPassOrPubMsg-操作评论或回复时附带回复信息，AdminUser表找不到当前管理员数据";
                    result.TipMessage = "引发错误，请重新登录试试";
                    return Json(result);
                }
                if (string.IsNullOrWhiteSpace(adminUser.Email))
                {
                    result.HideMessage = "DealOneNotPassOrPubMsg-操作评论或回复时附带回复信息，当前管理员没有设置邮箱";
                    result.TipMessage = "邮箱信息填写不全，请去邮箱设置面板进行设置。";
                    return Json(result);
                }
                // 需要抄送邮件，验证
                if (vm.SendEmail)
                {
                    // 判断EmailPassword是不是空足矣，因为邮箱设置必须全部填写完整才会更新数据表
                    if (string.IsNullOrWhiteSpace(adminUser.EmailPassword))
                    {
                        result.HideMessage = "DealOneNotPassOrPubMsg-操作评论或回复时抄送邮件，当前管理员没有设置邮箱";
                        result.TipMessage = "邮箱信息填写不全，请去邮箱设置面板进行设置。";
                        return Json(result);
                    }

                }
            }
            _baseSugar._dbHandler.BeginTran();
            string tmpMessagerName = "";
            string tmpMessagerEmail = "";
            string tmpAdmToMessagerContent = "";
            string tmpMessagerOrgContent = "";
            Article tmpMessagerRelatedArt;
            int opId = 0;
            try
            {
                if (vm.DType == 1)
                {
                    var cPassable = await _baseSugar._dbHandler.Queryable<Comment>().SingleAsync(c => c.Id == vm.Id);
                    // 不必判断isPub，无论是未审核的还是已公开的，全部更新IsPassed 不违背业务逻辑 减少多余代码判断
                    cPassable.IsPassed=true;
                    await _baseSugar._dbHandler.Updateable<Comment>(cPassable).ExecuteCommandAsync();
                    tmpMessagerName = cPassable.Name;
                    tmpMessagerEmail = cPassable.Email;
                    tmpMessagerOrgContent = cPassable.Content;
                    // 通过当前要处理通过的评论找到文章
                    tmpMessagerRelatedArt = await _articleReader.FindOneByIdAsync(cPassable.SourceId);
                    // 当前是审核评论 直接记录当前评论id
                    opId = vm.Id;
                }
                else
                {
                    var rPassable = await _baseSugar._dbHandler.Queryable<Reply>().SingleAsync(c => c.Id == vm.Id);
                    // 不必判断isPub，无论是未审核的还是已公开的，全部更新IsPassed 不违背业务逻辑 减少多余代码判断
                    rPassable.IsPassed = true;
                    await _baseSugar._dbHandler.Updateable<Reply>(rPassable).ExecuteCommandAsync();
                    tmpMessagerName = rPassable.Name;
                    tmpMessagerEmail = rPassable.Email;
                    tmpMessagerOrgContent = rPassable.Content;
                    // 通过当前要处理通过的回复找到评论
                    var relatedCmt = await _commentVmReader.FindOneByIdAsync(rPassable.CommentId);
                    // 通过评论找到文章
                    tmpMessagerRelatedArt = await _articleReader.FindOneByIdAsync(relatedCmt.SourceId);
                    // 当前是审核回复 记录当前回复对应的评论id
                    opId = relatedCmt.Id;
                }
                result.TipMessage = "已公开此条言论，会在评论区显示。";

                // 处理给这条内容附带回复
                if (vm.SupplyReply)
                {
                    Reply supReply = new Reply
                    {
                        CommentId = opId,
                        ReplyTime = DateTime.Now,
                        IsPassed = true,
                        IsAdmin = true,
                        ClientIp = "localhost",
                        Name = adminUser.AccountName,
                        Email = adminUser.Email,
                        SiteUrl = siteSett.Domain,
                    };
                    // 2回复某个回复，加上艾特符号；1回复评论，不用加
                    supReply.Content = vm.DType == 1 ? vm.Message : $"@{tmpMessagerName}：{vm.Message}";
                    tmpAdmToMessagerContent = supReply.Content;
                    await _baseSugar._dbHandler.Insertable<Reply>(supReply).ExecuteCommandAsync();
                    result.TipMessage = "已公开此条言论，会在评论区显示。且附带了你的回复。";
                }
                _baseSugar._dbHandler.CommitTran();
            }
            catch (Exception e)
            {
                _baseSugar._dbHandler.RollbackTran();
                result.Code = ValueCodes.Error;
                result.HideMessage = $"DealOneNotPassOrPubMsg-操作某{(vm.DType == 1 ? "评论" : "回复")}失败:" + e.Message;
                result.TipMessage = !isPub ?"通过失败请刷新重试": "给这条公开言论回复失败，请刷新重试";
                return Json(result);
            }
            result.Code = ValueCodes.Success;
            // 处理发送邮件提醒
            if (vm.SupplyReply&&vm.SendEmail&& adminUser != null)
            {
                try
                {
                    // send email
                    MailSendHelper mailSend = new MailSendHelper();
                    mailSend.InputSmtpServerHost(adminUser.SmtpHost, (int)adminUser.SmtpPort, (bool)adminUser.SmtpIsUseSsl);
                    mailSend.InputYourEmail(adminUser.AccountName, adminUser.Email);
                    mailSend.InputFriendEmail(tmpMessagerName, tmpMessagerEmail);
                    var t = tmpMessagerRelatedArt.IsShowTitle ? tmpMessagerRelatedArt.Title : "无题链接";
                    var n = !string.IsNullOrWhiteSpace(siteSett.SiteName) ? "-"+siteSett.SiteName : "";
                    var distinctionText = !isPub ? "，经过审核此条发言已被<i>公开</i>" : "，我对此进行了回复";
                    tmpAdmToMessagerContent = $"您在内容为<a href='{siteSett.Domain.TrimEnd('/')}/Detail?articleId={tmpMessagerRelatedArt.Id}'><b>{t}</b></a>上进行了发言" +
                        $"{distinctionText}，并且特意向您抄送这封邮件已表达我对此的兴趣。我的回复内容如下：" +
                        $"<br><p><mark>{tmpAdmToMessagerContent}</mark></p><br>您的发言原内容：<br><small>{tmpMessagerOrgContent}</small>" +
                        $"<br>您使用的昵称为：<small>{tmpMessagerName}</small>" +
                        $"<br><br>--该邮件为系统自动发送请勿回复--<br>--若您没有上述操作请忽略此邮件--";
                    mailSend.InputContent("发言收到了博主回应" + n, tmpAdmToMessagerContent, true);
                    mailSend.SendByAuthenticate(adminUser.Email, adminUser.EmailPassword);
                    result.TipMessage = result.TipMessage+"且邮件已发送给此网友。";
                }
                catch (Exception e)
                {
                    result.HideMessage = $"通过某{(vm.DType == 1 ? "评论" : "回复")}成功，尝试发送邮件异常:" + e.Message;
                    result.TipMessage = !isPub ? result.TipMessage + "已将其公开，但邮件发送失败。" : "已回复，但邮件发送失败。";
                }
            }
            result.TipMessage = !isPub ? result.TipMessage: $"已回复。{(vm.SendEmail? "且邮件已发送给此网友" : "")}";

            return Json(result);
        }

        /// <summary>
        /// 根据Id删除评论或回复
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dType">1评论，2回复</param>
        /// <param name="dType">是否发生邮件通知</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteOneMsg(string? pt, [FromBody]DeleteOneMsgViewModel vm) {
            ValueResult result = new ValueResult { Code = ValueCodes.UnKnow };
            if (vm.Id < 1)
            {
                result.HideMessage = "删除评论或回复失败：id无效";
                result.TipMessage = "删除评论或回复失败，请刷新重试";
                return Json(result);
            }
            SiteSetting siteSett = await _siteSettingReader.FirstOrDefaultAsync(s => 1 == 1);
            AdminUser adminUser = await _adminUserReader.FirstOrDefaultAsync(a => a.AccountName == slvm.AccountName);
            if (vm.SendEmail)
            {
                if (siteSett == null)
                {
                    result.Code = ValueCodes.Error;
                    result.HideMessage = "删除评论或回复时抄送邮件提醒，SiteSetting表找不到";
                    result.TipMessage = "引发错误，请重新登录试试";
                    return Json(result);
                }
                if (string.IsNullOrWhiteSpace(vm.Message))
                {
                    result.HideMessage = "删除评论或回复时抄送邮件提醒，邮件提醒内容未填写";
                    result.TipMessage = "邮件提醒内容未填写。";
                    return Json(result);
                }
                if (adminUser == null)
                {
                    result.Code = ValueCodes.Error;
                    result.HideMessage = "删除评论或回复时抄送邮件提醒，AdminUser表找不到当前管理员数据";
                    result.TipMessage = "引发错误，请重新登录试试";
                    return Json(result);
                }
                // 判断EmailPassword是不是空足矣，因为邮箱设置必须全部填写完整才会更新数据表
                if (string.IsNullOrWhiteSpace(adminUser.EmailPassword))
                {
                    result.HideMessage = "删除评论或回复时抄送邮件提醒，当前管理员没有设置邮箱";
                    result.TipMessage = "邮箱信息填写不全，请去邮箱设置面板进行设置。";
                    return Json(result);
                }
            }
            string tmpMessagerName = "";
            string tmpMessagerEmail = "";
            string tmpMessagerOrgContent = "";
            string tmpAdmToMessagerContent = "";
            Article tmpMessagerRelatedArt;
            _baseSugar._dbHandler.BeginTran();
            try
            {
                if (vm.DType==1)
                {
                    var cPassable = await _baseSugar._dbHandler.Queryable<Comment>().SingleAsync(c => c.Id == vm.Id);
                    await _baseSugar._dbHandler.Deleteable<Comment>().Where(cPassable).ExecuteCommandAsync();
                    // 删除此评论还要删除它的所有回复
                    await _baseSugar._dbHandler.Deleteable<Reply>().Where(r=>r.CommentId== vm.Id).ExecuteCommandAsync();
                    tmpMessagerName = cPassable.Name;
                    tmpMessagerEmail = cPassable.Email;
                    tmpMessagerOrgContent = cPassable.Content;
                    // 通过评论找到文章
                    tmpMessagerRelatedArt = await _articleReader.FindOneByIdAsync(cPassable.SourceId);
                }
                else
                {
                    // 删除回复
                    var rPassable = await _baseSugar._dbHandler.Queryable<Reply>().SingleAsync(c => c.Id == vm.Id);
                    await _baseSugar._dbHandler.Deleteable<Reply>().Where(rPassable).ExecuteCommandAsync();
                    tmpMessagerName = rPassable.Name;
                    tmpMessagerEmail = rPassable.Email;
                    tmpMessagerOrgContent = rPassable.Content;
                    // 通过当前要处理通过的回复找到评论
                    var relatedCmt = await _commentVmReader.FindOneByIdAsync(rPassable.CommentId);
                    // 通过评论找到文章
                    tmpMessagerRelatedArt = await _articleReader.FindOneByIdAsync(relatedCmt.SourceId);
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
            result.Code = ValueCodes.Success;
            result.TipMessage = "删除成功。";

            // 处理发送邮件提醒
            if (vm.SendEmail)
            {
                try
                {
                    // send email
                    MailSendHelper mailSend = new MailSendHelper();
                    mailSend.InputSmtpServerHost(adminUser.SmtpHost, (int)adminUser.SmtpPort, (bool)adminUser.SmtpIsUseSsl);
                    mailSend.InputYourEmail(adminUser.AccountName, adminUser.Email);
                    mailSend.InputFriendEmail(tmpMessagerName, tmpMessagerEmail);
                    var t = tmpMessagerRelatedArt.IsShowTitle ? tmpMessagerRelatedArt.Title : "无题链接";
                    var n = !string.IsNullOrWhiteSpace(siteSett.SiteName) ? "-" + siteSett.SiteName : "";
                    tmpAdmToMessagerContent = $"您在内容为<a href='{siteSett.Domain.TrimEnd('/')}/Detail?articleId={tmpMessagerRelatedArt.Id}'><b>{t}</b></a>上进行了发言，" +
                        $"经过决定此条发言已被<i>删除</i>，并且特意向您抄送这封邮件提示。删除原因如下：" +
                        $"<br><p><mark>{vm.Message}</mark></p><br>您被删除的发言原内容：<br><small>{tmpMessagerOrgContent}</small>" +
                        $"<br>您使用的昵称为：<small>{tmpMessagerName}</small>" + 
                        $"<br><br>--该邮件为系统自动发送请勿回复--<br>--若您没有上述操作请忽略此邮件--";
                    mailSend.InputContent("发言被删除"+n, tmpAdmToMessagerContent, true);
                    mailSend.SendByAuthenticate(adminUser.Email, adminUser.EmailPassword);
                    result.TipMessage = "删除成功，邮件已发送。";
                }
                catch (Exception e)
                {
                    result.HideMessage = $"删除某{(vm.DType == 1 ? "评论" : "回复")}成功，尝试发送邮件异常:" + e.Message;
                    result.TipMessage = "已被删除，但邮件发送失败。";
                }

            }
            return Json(result);
        }


        /// <summary>
        /// 留言管理(未审核的)页面入口 索引第一页 默认返回未审核的评论和回复列表
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> LeaveNotPassAmManagement(string? pt)
        {
            // 清空列表。 因为是静态模型数据 刷新会追加回复列表
            slvm.NotPassComments?.Clear();
            slvm.NotPassReplies?.Clear();
            slvm.PublicComments?.Clear();
            slvm.PublicReplies?.Clear();
            // 默认返回未审核的评论和回复列表
            slvm.NotPassComments = await _commentVmReader.GetListBuilder().Where(c=>c.IsAdmin==false&&(c.IsPassed==false||c.IsPassed==null))
                .OrderBy(c=>c.CommentTime, SqlSugar.OrderByType.Desc).Take(20).ToListAsync();


            var notPassRepliesTmp = await _replyVmReader.GetListBuilder().Where(r => r.IsAdmin == false&&(r.IsPassed == false || r.IsPassed == null))
                .OrderBy(r => r.ReplyTime, SqlSugar.OrderByType.Desc).Take(20).ToListAsync();
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
                var notPassComments = await _commentVmReader.GetListBuilder().Where(c => c.IsAdmin == false && (c.IsPassed == false || c.IsPassed == null))
                    .OrderBy(c => c.CommentTime, SqlSugar.OrderByType.Desc).Skip((index-1) * 20).Take(20).ToListAsync();
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
                var notPassRepliesTmp = await _replyVmReader.GetListBuilder().Where(r => r.IsAdmin == false && (r.IsPassed == false || r.IsPassed == null))
                    .OrderBy(r => r.ReplyTime, SqlSugar.OrderByType.Desc).Skip((index - 1) * 20).Take(20).ToListAsync();
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
        /// 留言管理(已公开的)页面入口 索引第一页 默认返回已公开的评论和回复列表
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> LeavePublicAmManagement(string? pt)
        {
            // 清空列表。 因为是静态模型数据 刷新会追加回复列表
            slvm.NotPassComments?.Clear();
            slvm.NotPassReplies?.Clear();
            slvm.PublicComments?.Clear();
            slvm.PublicReplies?.Clear();
            // 默认返回已公开的评论和回复列表
            slvm.PublicComments = await _commentVmReader.GetListBuilder().Where(c => c.IsAdmin == false && c.IsPassed == true)
                .OrderBy(c => c.CommentTime, SqlSugar.OrderByType.Desc).Take(20).ToListAsync();


            var pubRepliesTmp = await _replyVmReader.GetListBuilder().Where(r => r.IsAdmin == false&&r.IsPassed == true)
                .OrderBy(r => r.ReplyTime, SqlSugar.OrderByType.Desc).Take(20).ToListAsync();
            foreach (var cmv in slvm.PublicComments)
            {
                // 获取评论所在文章
                if (cmv.SourceType == 1)
                {
                    cmv.RelatedArticle = await _articleReader.FindOneByIdAsync(cmv.SourceId);
                    cmv.RelatedArticleUrl = "/Detail?articleId=" + cmv.SourceId;
                }
            }

            foreach (var replyTmp in pubRepliesTmp)
            {
                // 获取当前此条回复对应的评论
                replyTmp.RelatedComment = await _commentVmReader.FindOneByIdAsync(replyTmp.CommentId);
                // 获取回复所在文章
                if (replyTmp.RelatedComment.SourceType == 1)
                {
                    replyTmp.RelatedArticle = await _articleReader.FindOneByIdAsync(replyTmp.RelatedComment.SourceId);
                    replyTmp.RelatedArticleUrl = "/Detail?articleId=" + replyTmp.RelatedComment.SourceId;
                }
                slvm.PublicReplies?.Add(replyTmp);
            }
            // 自动封装已有的数据
            slvm = (LeaveMessageViewModel)WrapMustNeedPassFields(slvm);
            return View(slvm);
        }

        /// <summary>
        /// 获取已公开的某页评论列表
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetPublicComments(string? pt, int index)
        {
            ValueResult result = new ValueResult { Code = ValueCodes.UnKnow };
            try
            {
                var publicComments = await _commentVmReader.GetListBuilder().Where(c => c.IsAdmin == false&&c.IsPassed == true)
                    .OrderBy(c => c.CommentTime, SqlSugar.OrderByType.Desc).Skip((index - 1) * 20).Take(20).ToListAsync();
                foreach (var cmv in publicComments)
                {
                    // 获取评论所在文章
                    if (cmv.SourceType == 1)
                    {
                        cmv.RelatedArticle = await _articleReader.FindOneByIdAsync(cmv.SourceId);
                        cmv.RelatedArticleUrl = "/Detail?articleId=" + cmv.SourceId;
                    }
                }
                result.Code = ValueCodes.Success;
                result.Data = new { PublicComments = publicComments };

            }
            catch (Exception e)
            {
                result.Code = ValueCodes.Error;
                result.HideMessage = "获取已公开的某页评论列表发生异常:" + e.Message;
                result.TipMessage = "加载数据失败请重试";
            }
            return Json(result);
        }

        /// <summary>
        /// 获取已公开的某页回复列表
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetPublicReplies(string? pt, int index)
        {
            ValueResult result = new ValueResult { Code = ValueCodes.UnKnow };
            try
            {
                var publicRepliesTmp = await _replyVmReader.GetListBuilder().Where(r => r.IsAdmin == false&&r.IsPassed == true )
                    .OrderBy(r => r.ReplyTime, SqlSugar.OrderByType.Desc).Skip((index - 1) * 20).Take(20).ToListAsync();
                List<ReplyCarryViewModel> publicReplies = new List<ReplyCarryViewModel>();
                foreach (var replyTmp in publicRepliesTmp)
                {
                    // 获取当前此条回复所属的评论
                    replyTmp.RelatedComment = await _commentVmReader.FindOneByIdAsync(replyTmp.CommentId);
                    // 获取回复所在文章
                    if (replyTmp.RelatedComment.SourceType == 1)
                    {
                        replyTmp.RelatedArticle = await _articleReader.FindOneByIdAsync(replyTmp.RelatedComment.SourceId);
                        replyTmp.RelatedArticleUrl = "/Detail?articleId=" + replyTmp.RelatedComment.SourceId;
                    }
                    publicReplies.Add(replyTmp);
                }
                result.Code = ValueCodes.Success;
                result.Data = new { PublicReplies = publicReplies };
            }
            catch (Exception e)
            {
                result.Code = ValueCodes.Error;
                result.HideMessage = "获取已公开某页评回复列表发生异常:" + e.Message;
                result.TipMessage = "加载数据失败请重试";
            }
            return Json(result);
        }

        /// <summary>
        /// 根据关键词删除时查询匹配到的评论和回复总数
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> QueryUseWordCount(string? pt, string? kw)
        {

            ValueResult result = new ValueResult { Code = ValueCodes.UnKnow };
            var matchedCount = 0;
            kw = kw?.Trim();
            if (!string.IsNullOrWhiteSpace(kw))
            {
                matchedCount = await _commentVmReader.GetListBuilder().Where(c =>
                    c.Content != null && c.Content.Contains(kw)).CountAsync();
                matchedCount += await _replyVmReader.GetListBuilder().Where(r =>
                    r.Content != null && r.Content.Contains(kw)).CountAsync();
            }
            result.Code = ValueCodes.Success;
            result.Data = new { MatchedCount = matchedCount };
            return Json(result);
        }

        /// <summary>
        /// 根据关键词删除
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UseWordDelete(string? pt, string? kw)
        {
            ValueResult result = new ValueResult { Code = ValueCodes.UnKnow };
            var matchedCount = 0;
            kw = kw?.Trim();
            if (!string.IsNullOrWhiteSpace(kw))
            {
                matchedCount += await _commentVmReader.GetListBuilder().Where(c =>
                    c.Content != null && c.Content.Contains(kw)).CountAsync();
                matchedCount += await _replyVmReader.GetListBuilder().Where(r =>
                    r.Content != null && r.Content.Contains(kw)).CountAsync();
            }
            if (matchedCount<=0)
            {
                result.TipMessage = "删除关键字匹配不到任何数据";
                return Json(result);
            }
            _baseSugar._dbHandler.BeginTran();
            try
            {
                //---1先删除匹配的评论,然后评论下面的无辜回复也一并删除 2再删除匹配的所有回复
                var tmpCIdList = new List<int>();
                tmpCIdList = _commentVmReader.GetListBuilder()
                    .Where(c => c.Content != null && c.Content.Contains(kw))
                    .Select(c => c.Id).ToList();
                if (tmpCIdList.Any())
                {
                    var tmpCIdString = string.Join(",", tmpCIdList);
                    // 删除匹配的所有评论
                    await _baseSugar._dbHandler.Ado.ExecuteCommandAsync(
                            $"delete from Comment where Id in ({tmpCIdString})");
                    // 删除匹配的所有评论下的所有回复
                    await _baseSugar._dbHandler.Ado.ExecuteCommandAsync(
                        $"delete from Reply where CommentId in ({tmpCIdString})");
                }

                // 最后 删除匹配的所有回复
                SugarDataBaseStorage<Reply, int> rReplySet = new SugarDataBaseStorage<Reply, int>(_baseSugar._dbHandler);
                await rReplySet.DeleteEntitiesAsync(r=>r.Content != null && r.Content.Contains(kw));
                _baseSugar._dbHandler.CommitTran();
            }
            catch (Exception e)
            {
                _baseSugar._dbHandler.RollbackTran();
                result.Code = ValueCodes.Error;
                result.HideMessage = "匹配删除失败,"+e.Message;
                result.TipMessage = "匹配删除失败，未作任何更改，请重新再试。";
                return Json(result);
            }

            result.Code = ValueCodes.Success; 
            return Json(result);
        }
    }
}
