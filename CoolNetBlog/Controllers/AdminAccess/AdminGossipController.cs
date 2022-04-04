using ComponentsServices.Base;
using CoolNetBlog.Base;
using CoolNetBlog.Models;
using CoolNetBlog.ViewModels;
using CoolNetBlog.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

namespace CoolNetBlog.Controllers.Admin
{

    /// <summary>
    /// 后台"闲言碎语"内容操作
    /// </summary>

    public class AdminGossipController : BaseAdminController
    {
        private static GossipViewModel sgvm = new();
        private SugarDataBaseStorage<Gossip, int> _gossipSet;
        private SugarDataBaseStorage<FilePath, int> _filePathSet;

        public AdminGossipController():base()
        {
            BaseSugar baseSugar = new BaseSugar();
            _gossipSet = new SugarDataBaseStorage<Gossip, int>(baseSugar._dbHandler);
            _filePathSet = new SugarDataBaseStorage<FilePath, int>(baseSugar._dbHandler);
        }

        /// <summary>
        /// 根据Id删除一个"闲言碎语"
        /// </summary>
        /// <param name="id"></param>
        /// <param name="kw">有关键词搜索，带上关键词，删除成功后跳转带给首页过滤</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Delete([FromForm] int id, [FromForm] string? kw) {
            if (id<1)
            {
                ModelState.AddModelError("", "删除闲言碎语失败:Id无效，重启浏览器再试吧？。");
                return View("GossipAmManagement", sgvm);
            }
            Gossip delable;
            try
            {
                delable = await _gossipSet.FindOneByIdAsync(id);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "删除闲言碎语失败:没有此Id，刷新再试吧？。");
                return View("GossipAmManagement", sgvm);
            }
           
            // 一并删除此“闲言碎语”的点赞数据
            SugarDataBaseStorage<CommonThumbUp, int> cThumbUpDelHelper = new SugarDataBaseStorage<CommonThumbUp, int>(_gossipSet._dbHandler);
            _gossipSet.TransBegin();
            try
            {
                // 删除此条的所有点赞表态
                await cThumbUpDelHelper.DeleteEntitiesAsync(u => u.SourceId == id&&u.SourceType==1);

                // 删除此条实体
                await _gossipSet.DeleteAsync(delable);
                _gossipSet.TransCommit();
            }
            catch (Exception e)
            {
                _gossipSet.TransRoll();
                ModelState.AddModelError("", "删除失败:刷新再试吧？。");
                return View("GossipAmManagement", sgvm);
            }

            return RedirectToAction("GossipAmManagement", "AdminGossip", new { pt = sgvm.PassToken,kw= kw });

        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddGossip([FromForm] GossipViewModel vm)
        {
            RemoveSomeValid();
            ModelState.Clear();
            // 新增，赋值当前实体编辑的数据 避免新增错误时返回视图后丢失 
            sgvm.Content = vm.Content;
            sgvm.ImgUrl = vm.ImgUrl;
            sgvm.Type = vm.Type;
            if (vm.Type<=0)
            {
                ModelState.AddModelError("", "发表失败:请选择类型");
                return View("GossipAmManagement", sgvm);
            }
            if (vm.Type==2&&string.IsNullOrWhiteSpace(vm.ImgUrl))
            {
                // 内部图片下拉框没有 从第三方输入框的值去找
                vm.ImgUrl = HttpContext.Request.Form["UnofficialImg"];
                if (string.IsNullOrWhiteSpace(vm.ImgUrl))
                {
                    ModelState.AddModelError("", "发表失败:带图片的内容请选定图片url地址");
                    return View("GossipAmManagement", sgvm);
                }
            }
            if (vm.Type == 1)
            {
                vm.ImgUrl = null;
            }
            if (string.IsNullOrWhiteSpace(vm.Content)|| vm.Content.Length>60)
            {
                ModelState.AddModelError("", "发表失败:内容字数无效");
                return View("GossipAmManagement", sgvm);
            }
            Gossip editable = new Gossip();    
            editable.ImgUrl = vm.ImgUrl;
            editable.StarNumber = vm.StarNumber;
            editable.AddTime = DateTime.Now;
            editable.Content = vm.Content;
            editable.Type = vm.Type;
            try
            {
                _gossipSet.TransBegin();
                await _gossipSet.InsertAsync(editable);
                _gossipSet.TransCommit();
            }
            catch (Exception e)
            {
                _gossipSet.TransRoll();
                ModelState.AddModelError("", "发表失败:已回滚，复制输入好的的内容，返回重新再试一遍？");
                return View("GossipAmManagement", sgvm);
            }
            // 新增成功，直接重定向到第一页数据
            return RedirectToAction("GossipAmManagement", "AdminGossip", new { pt= sgvm.PassToken });
        }


        /// <summary>
        /// 列表页面入口
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GossipAmManagement(string? pt, string? kw, int index=1, int pageCount = 30)
        {
            // 获取'闲言碎语'列表
            IList<Gossip> gossipes;
            if (!string.IsNullOrWhiteSpace(kw))
            {
                gossipes = await _gossipSet.GetListBuilder().Where(a => 
                    (a.Id.ToString()==kw) ||
                    (a.Content != null && a.Content.Contains(kw)))
                    .OrderBy(a => a.AddTime, SqlSugar.OrderByType.Desc)
                    .Skip((index-1)* pageCount)
                    .Take(pageCount).ToListAsync(); 
            }
            else
            {
                gossipes = await _gossipSet.GetListBuilder()
                    .OrderBy(a => a.AddTime, SqlSugar.OrderByType.Desc)
                    .Skip((index - 1) * pageCount)
                    .Take(pageCount).ToListAsync();
            }

            sgvm = new GossipViewModel { GossipesOrg = gossipes };

            sgvm.ImgRelPaths = await _filePathSet.GetListBuilder().Where(f => f.Type == "img")
                .OrderBy(f => f.UploadTime, SqlSugar.OrderByType.Desc).Take(20).ToListAsync();

            // 处理分页按钮索引值
            sgvm.Keyword = kw;
            if (!sgvm.GossipesOrg.Any())
            {
                // 当前页没有任何数据了 不改动分页索引值
                sgvm.NextIndex = index;
                sgvm.PreIndex = (index-1) <= 0? 1: index - 1;
            }
            else
            {
                if (index<=1)
                {
                    sgvm.NextIndex = 2;
                    sgvm.PreIndex = 1;
                }
                else
                {
                    sgvm.NextIndex = index + 1;
                    sgvm.PreIndex = index - 1;
                }
            }
            // 自动封装已有的数据
            sgvm = (GossipViewModel)WrapMustNeedPassFields(sgvm);
            return View(sgvm);
        }


        /// <summary>
        /// 移除有些场景下表单不需要验证的属性
        /// </summary>
        private void RemoveSomeValid()
        {
        }
    }
}
