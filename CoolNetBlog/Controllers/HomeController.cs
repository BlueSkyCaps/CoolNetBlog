using CoolNetBlog.Base;
using CoolNetBlog.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CoolNetBlog.Controllers.Home
{
    /// <summary>
    /// 主页前台展示控制
    /// </summary>
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger):base()
        {
            _logger = logger;
            WrapsGlobalHomeData();
        }

        /// <summary>
        /// 主页索引 展示具体分页数据
        /// </summary>
        /// <param name="from">点击来源 菜单|外链</param>
        /// <param name="menuId">点击来源的具体值 菜单是菜单id ..</param>
        /// <param name="kw">点击来源的具体值 搜索是关键字..</param>
        /// <param name="pageIndex">分页码</param>
        /// <param name="onePageCount">每页文章条数</param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string? from, int? menuId,string? kw, int pageIndex=1, int onePageCount=5)
        {
            _homeGlobalView.CurrentTitle = _homeGlobalView.HomeSiteSettingData.SiteName;
            // 从配置里取设置的每页条数
            onePageCount = _homeGlobalView.HomeSiteSettingData.OnePageCount<=0?
                onePageCount: _homeGlobalView.HomeSiteSettingData.OnePageCount;
            // 跳到当前页 获取文章列表
            int c = await DealFilterData(from, menuId, kw, pageIndex, onePageCount);
            
            // 根据文章MenuId获取每篇文章所属菜单名
            foreach (var item in _homeGlobalView.HomeArticleViewModels)
            {
                item.Ig_MenuName = (await bdb._dbHandler.Queryable<Menu>().FirstAsync(m => m.Id == item.MenuId))?.Name ?? "未知菜单";
            }
            ComputePage(c, pageIndex, onePageCount);

            return View(_homeGlobalView);
        }
    }
}