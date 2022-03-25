
using CoolNetBlog.Models;
using CoolNetBlog.ViewModels.Detail;

namespace CoolNetBlog.ViewModels.Home
{
    /// <summary>
    /// 主页所需展示视图模型
    /// </summary>
    public class HomeViewModel
    {
        /// <summary>
        /// 全局视图所需的全局菜单数据
        /// </summary>
        public List<HomeMenuViewModel> HomeMenusData { get; set; } = new List<HomeMenuViewModel>();
        /// <summary>
        /// 全局视图所需的全局基本配置数据
        /// </summary>
        public HomeSiteSettingViewModel HomeSiteSettingData { get; set; } = new HomeSiteSettingViewModel();
        /// <summary>
        /// 全局视图所需的全局链接显示("看看这些")数据
        /// </summary>
        public List<HomeLoveLookViewModel> HomeLoveLookData { get; set; } = new List<HomeLoveLookViewModel>();
        /// <summary>
        /// 分页列表视图所需用到的文章列表的视图模型类
        /// </summary>
        public List<HomeArticleViewModel> HomeArticleViewModels { get; set; } = new List<HomeArticleViewModel>();
        /// <summary>
        /// 显示当前浏览的菜单、搜索结果提示，但基本主页分页浏览不显示
        /// </summary>
        public string LocationTip { get; set; } = "";
        public string Location { get; set; } = "";
        /// <summary>
        /// 找不到文章的提示
        /// </summary>
        public string NotTips { get; set; } = "";
        /// <summary>
        /// 当前网页需要显示的title 首页是站点名 文章是文章标题 文章不显示标题仍是站点名
        /// </summary>
        public string CurrentTitle { get; set; } = "";

        /// <summary>
        /// 是否没有设置显示任何一个侧边栏组件
        /// </summary>
        public bool IsNotShowAnyOneCom { get; set; } = false;

        /// <summary>
        /// 分页计数
        /// </summary>
        public PageCompute PageCompute { get; set; } = new PageCompute();


        /// <summary>
        /// 此为某篇文章详细的视图模型类
        /// </summary>
        public DetailArticleViewModel? DetailArticleData { get; set; } = new DetailArticleViewModel();

    }
    public class PageCompute
    {
        public int PageIndex { get; set; } = 1;
        public int NextIndex { get; set; } = 1;
        public int PreIndex { get; set; } = 1;
        public bool ShowPreIndex { get; set; } = false;
        public bool ShowNextIndex { get; set; } = false;
    }
}
