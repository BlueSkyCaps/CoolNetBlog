using ComponentsServices.Base;
using CoolNetBlog.Bll;
using CoolNetBlog.Models;
using CoolNetBlog.ViewModels.Admin;
using CoolNetBlog.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;

namespace CoolNetBlog.Base
{
    public class BaseController:Controller
    {
        public static BaseSugar bdb;
        //
        public BaseLogicBll _bll;
        // 主页全局所需展示数据
        public HomeViewModel _homeGlobalView;


        public BaseController()
        {
            if (bdb is null)
                bdb = new BaseSugar();
            _bll = new BaseLogicBll();
            _homeGlobalView = new HomeViewModel();
        }

        /// <summary>
        /// 获取前台主页全局所需展示的数据
        /// </summary>
        protected async void WrapsGlobalHomeData()
        {
            // 处理菜单和其子菜单
            var allMenus = await bdb._dbHandler.SqlQueryable<HomeMenuViewModel>
                ("select * from Menu where IsShow=1 order by OrderNumber asc").ToListAsync();
            if (allMenus.Any())
            {
                // 先找出所有顶级菜单
                var pMenus = allMenus.Where(m => m.PId == 0).ToList();
                allMenus.RemoveAll(m => m.PId == 0);
                // 迭代顶级菜单 搜索下级菜单
                _bll.DealSubMenu(pMenus, allMenus);
                // 赋值菜单数据
                _homeGlobalView.HomeMenusData = pMenus;
            }
            // 获取基本配置
            _homeGlobalView.HomeSiteSettingData = await bdb._dbHandler.Queryable<HomeSiteSettingViewModel>().FirstAsync();
            _homeGlobalView.HomeLoveLookData = await bdb._dbHandler.Queryable<HomeLoveLookViewModel>().ToListAsync();

            // 判断是否没有显示任何一个侧边栏组件
            _homeGlobalView.IsNotShowAnyOneCom = !_homeGlobalView.HomeSiteSettingData.IsShowEdgeSearch
                && !_homeGlobalView.HomeSiteSettingData.IsShowLoveLook && !_homeGlobalView.HomeSiteSettingData.IsShowWishPicture;

        }

        /// <summary>
        /// 处理所需文章数据，主页分页 或具体菜单下的文章 或关键词搜索
        /// </summary>
        /// <param name="from"></param>
        /// <param name="menuId">若有，具体菜单下的文章 菜单Id</param>
        /// <param name="kw">若有，关键字搜索 关键字</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected async Task<int> DealFilterData(string? from, int? menuId, string? kw, int pageIndex, int onePageCount)
        {
            return await _bll.DealFilterData(_homeGlobalView, bdb, from, menuId, kw, pageIndex, onePageCount);          
        }


        /// <summary>
        /// 处理分页逻辑
        /// </summary>
        /// <param name="c">当前按条件过滤后的总数</param>
        /// <param name="pageIndex"></param>
        /// <param name="onePageCount"></param>
        protected void ComputePage(int c,int pageIndex, int onePageCount)
        {
            _bll.ComputePage(_homeGlobalView, c,pageIndex, onePageCount);           
        }
    }
}
