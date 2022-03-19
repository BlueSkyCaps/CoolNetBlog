using ComponentsServices.Base;
using CoolNetBlog.Base;
using CoolNetBlog.Models;
using CoolNetBlog.ViewModels;
using CoolNetBlog.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

namespace CoolNetBlog.Controllers.Admin
{

    /// <summary>
    /// 后台文章(帖子)操作
    /// </summary>

    public class AdminArticleController : BaseAdminController
    {
        private static ArticleViewModel smvm = new();
        private SugarDataBaseStorage<Article, int> _articleSet;
        private SugarDataBaseStorage<Menu, int> _menuSet;
        private SugarDataBaseStorage<FilePath, int> _filePathSet;

        public AdminArticleController():base()
        {
            BaseSugar baseSugar = new BaseSugar();
            _articleSet = new SugarDataBaseStorage<Article, int>(baseSugar._dbHandler);
            _menuSet = new SugarDataBaseStorage<Menu, int>(baseSugar._dbHandler);
            _filePathSet = new SugarDataBaseStorage<FilePath, int>(baseSugar._dbHandler);
        }

        /// <summary>
        /// 根据Id删除一个文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Delete([FromForm] int id) {
            if (id<1)
            {
                ModelState.AddModelError("", "删除失败:文章Id无效，重启浏览器再试吧？。");
                return View("ArticleAmManagement", smvm);
            }
            Article delable;
            try
            {
                delable = await _articleSet.FindOneByIdAsync(id);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "删除失败:没有此文章Id，刷新再试吧？。");
                return View("ArticleAmManagement", smvm);
            }
           
            // 一并删除此文章的点赞表态、评论、回复数据
            SugarDataBaseStorage<Reply, int> replyDelHelper = new SugarDataBaseStorage<Reply, int>(_articleSet._dbHandler);
            SugarDataBaseStorage<Comment, int> commentDelHelper = new SugarDataBaseStorage<Comment, int>(_articleSet._dbHandler);
            SugarDataBaseStorage<ArticleThumbUp, int> thumbUpDelHelper = new SugarDataBaseStorage<ArticleThumbUp, int>(_articleSet._dbHandler);
            _articleSet.TransBegin();
            try
            {
                // 删除此文章的点赞表态
                await thumbUpDelHelper.DeleteEntitiesAsync(u => u.ArticleId == id);

                // 删除此文章所有评论下的所有回复
                var tmpCIdList = new List<int>();
                tmpCIdList = commentDelHelper.GetListBuilder()
                    .Where(c => c.SourceId == id && c.SourceType == 1)
                    .Select(c => c.Id).ToList();
                var tmpCIdString = string.Join(",", tmpCIdList);
                await replyDelHelper._dbHandler.Ado.ExecuteCommandAsync(
                    $"delete from Reply where CommentId in ({tmpCIdString})");
                
                // 删除此文章所有评论
                await commentDelHelper.DeleteEntitiesAsync(c => c.SourceId == id && c.SourceType == 1);
                _articleSet.TransCommit();

                // 删除文章实体
                await _articleSet.DeleteAsync(delable);
            }
            catch (Exception)
            {
                _articleSet.TransRoll();
                ModelState.AddModelError("", "删除失败:刷新再试吧？。");
                return View("ArticleAmManagement", smvm);
            }

            return RedirectToAction("ArticleAmManagement", "AdminArticle", new { pt = smvm.PassToken });

        }

        /// <summary>
        /// 更新|创建 文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ArticleEdit([FromForm] int? id, ArticleViewModel vm)
        {
            RemoveSomeValid();
            ModelState.Clear();
            Article editable = new Article();    
            editable.Abstract = vm.Abstract;
            editable.Title = vm.Title;
            editable.LockPassword = vm.LockPassword;
            editable.IsLock = vm.IsLock;
            editable.IsDraft = vm.IsDraft;
            editable.CustUri = string.IsNullOrWhiteSpace(vm.CustUri)?null: vm.CustUri?.Trim();
            editable.IsShowTitle = vm.IsShowTitle;
            editable.CommentType = vm.CommentType;
            editable.Content = vm.Content;
            editable.MenuId = vm.MenuId;
            editable.Labels = vm.Labels;
            editable.CommentType = vm.CommentType;
            var belongMenu = await _menuSet.FindOneByIdAsync(editable.MenuId);
            if (belongMenu.IsHome)
            {
                ModelState.AddModelError("", "发表失败:选择的菜单是主页菜单，主页菜单不能拥有文章。\r\n" +
                    "主页菜单的作用：重定向到站点首页，显示全部菜单中非草稿的文章，通常是第一次进站点时的第一页");
                // 因为是提交表单数据，所以下拉框的值是空的，此处从事先的静态数据中取出赋值
                vm.MenuSelectList = smvm.MenuSelectList;
                vm = (ArticleViewModel)WrapMustNeedPassFields(vm);

                return View(vm);
            }
            var belongMenuHasSubMenu = (await _menuSet.GetListByExpAsync(m=>m.PId== belongMenu.Id)).Any();

            if (belongMenuHasSubMenu)
            {
                ModelState.AddModelError("", "发表失败:选择的菜单不是最下级菜单，它还有子菜单。\r\n" +
                    "您应该把文章的所属菜单设置为最下级的菜单，否则是没有意义的。");
                // 因为是提交表单数据，所以下拉框的值是空的，此处从事先的静态数据中取出赋值
                vm.MenuSelectList = smvm.MenuSelectList;
                vm = (ArticleViewModel)WrapMustNeedPassFields(vm);

                return View(vm);
            }
            if (editable.IsLock&& String.IsNullOrWhiteSpace(editable.LockPassword))
            {
                ModelState.AddModelError("", "发表失败:隐私文章必须得设置一个密码");
                // 因为是提交表单数据，所以下拉框的值是空的，此处从事先的静态数据中取出赋值
                vm.MenuSelectList = smvm.MenuSelectList;
                vm = (ArticleViewModel)WrapMustNeedPassFields(vm);

                return View(vm);
            }
            if (string.IsNullOrWhiteSpace(editable.Content)|| string.IsNullOrWhiteSpace(editable.Abstract))
            {
                ModelState.AddModelError("", "发表失败:文章得有内容和摘要呀！");
                // 因为是提交表单数据，所以下拉框的值是空的，此处从事先的静态数据中取出赋值
                vm.MenuSelectList = smvm.MenuSelectList;
                vm = (ArticleViewModel)WrapMustNeedPassFields(vm);

                return View(vm);
            }
            var hasSamecu =  await _articleSet.AnyAsync(a=>!string.IsNullOrWhiteSpace(a.CustUri) && 
                !string.IsNullOrWhiteSpace(vm.CustUri) && a.CustUri==vm.CustUri &&a.Id!=vm.Id);
            if (hasSamecu)
            {
                ModelState.AddModelError("", "发表失败:自定义Uri已经被某文章使用，自定义Uri必须是唯一的哦！");
                // 因为是提交表单数据，所以下拉框的值是空的，此处从事先的静态数据中取出赋值
                vm.MenuSelectList = smvm.MenuSelectList;
                vm = (ArticleViewModel)WrapMustNeedPassFields(vm);
                return View(vm);
            }
            // 处理标签格式
            if (!string.IsNullOrWhiteSpace(vm.Labels))
            {
                var dsc = new string[] { " ", ",", "，" };
                var lbs = vm.Labels.Split(dsc, StringSplitOptions.RemoveEmptyEntries).ToList();
                lbs.RemoveAll(c => dsc.Contains(c));
                vm.Labels = lbs.Aggregate((x1,x2)=>x1+","+x2);
            }
            try
            {
                _articleSet.TransBegin();
                // 若id不为空，则是更新某文章
                if (id != null && id > 0)
                {
                    editable.Id = vm.Id;
                    if (vm.UpTimeLine)
                    {
                        editable.UpdateTime = DateTime.Now;
                        // 更新，忽略创建时间
                        await _articleSet.UpdateByIgColsAsync(editable, "CreatedTime");
                    }
                    else
                    {
                        // 更新，忽略创建时间 且若勾选不更新更新时间 也忽略更新时间
                        await _articleSet.UpdateByIgColsAsync(editable, "CreatedTime", "UpdateTime");
                    }
                }
                else
                {
                    editable.CreatedTime = DateTime.Now;
                    editable.UpdateTime = DateTime.Now;
                    await _articleSet.InsertAsync(editable);
                }
                _articleSet.TransCommit();
            }
            catch (Exception e)
            {
                _articleSet.TransRoll();
                ModelState.AddModelError("", "发表失败:已回滚，复制输入好的的内容，返回重新再试一遍？");
                vm.MenuSelectList = smvm.MenuSelectList;
                vm = (ArticleViewModel)WrapMustNeedPassFields(vm);
                return View(vm);
            }
            return RedirectToAction("ArticleAmManagement", "AdminArticle", new { pt= smvm.PassToken });

        }

        /// <summary>
        /// 显示文章编辑的页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> ArticleEdit(string? pt, int? id)
        {
            ArticleViewModel vm = new ArticleViewModel();
            // 若id不为空，则是更新某文章
            if (id!= null && id>0)
            {
                var orgArticle = await _articleSet.FindOneByIdAsync(id);
                vm.Abstract = orgArticle.Abstract;
                vm.Title = orgArticle.Title;
                vm.LockPassword = orgArticle.LockPassword;
                vm.IsLock = orgArticle.IsLock;
                vm.CustUri = orgArticle.CustUri;
                vm.CommentType = orgArticle.CommentType;
                vm.IsDraft = orgArticle.IsDraft;
                vm.Labels = orgArticle.Labels;
                vm.IsShowTitle = orgArticle.IsShowTitle;
                vm.Content = orgArticle.Content;
                vm.CreatedTime = orgArticle.CreatedTime;
                vm.MenuId = orgArticle.MenuId;
                vm.UpdateTime = orgArticle.UpdateTime;
                vm.Id = orgArticle.Id;
                // 获取关联的菜单实体，编辑文章时
                vm.RelatedMenu = _menuSet.FindOneById(orgArticle.MenuId);
            }
            // 封装菜单下拉框选择列表，用于在设置归属菜单时显示
            vm.MenuSelectList = (await _menuSet.GetAllListAsync())
                .Select(m=>new SelectList { Text=m.Name, Value=m.Id})
                .ToList();
            vm.ImgRelPaths = await _filePathSet.GetListBuilder().Where(f => f.Type == "img")
                .OrderBy(f=>f.UploadTime, SqlSugar.OrderByType.Desc).Take(20).ToListAsync();
            vm = (ArticleViewModel)WrapMustNeedPassFields(vm);
            return View(vm);
        }


        /// <summary>
        /// 文章管理页面入口
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ArticleAmManagement(string? pt, string? kw)
        {
            // 获取文章列表
            var _articleSet = new SugarDataBaseStorage<Article, int>();
            IList<Article> articles;
            if (!string.IsNullOrWhiteSpace(kw))
            {
                articles = await _articleSet.GetListBuilder().Where(a => 
                    (a.Title != null && a.Title.Contains(kw)) ||
                    (a.Labels != null && a.Labels.Contains(kw)) ||
                    (a.Abstract != null && a.Abstract.Contains(kw)))
                    .OrderBy(a => a.UpdateTime, SqlSugar.OrderByType.Desc).Take(20).ToListAsync(); 
            }
            else
            {
                articles = await _articleSet.GetTopCountListAsync(20, a => a.UpdateTime, SqlSugar.OrderByType.Desc);
            }
            foreach (var item in articles)
            {
                item.Ig_MenuName = _menuSet.FindOneById(item.MenuId)?.Name??"未命名的菜单";
            }
            smvm = new ArticleViewModel { ArticlesOrg = articles };
            smvm.MenuSelectList = (await _menuSet.GetAllListAsync())
                .Select(m => new SelectList { Text = m.Name, Value = m.Id })
                .ToList();
            smvm.HasAnyOneMenu = await _menuSet.AnyAsync(null);
            // 自动封装已有的数据
            smvm = (ArticleViewModel)WrapMustNeedPassFields(smvm);
            return View(smvm);
        }


        /// <summary>
        /// 移除有些场景下表单不需要验证的属性
        /// </summary>
        private void RemoveSomeValid()
        {
            ModelState.Remove("AccountName");
            ModelState.Remove("LockPassword");
            ModelState.Remove("Title");
            ModelState.Remove("Abstract");
            ModelState.Remove("Content");
            ModelState.Remove("Name");
        }
    }
}
