using CoolNetBlog.Models;

namespace CoolNetBlog.ViewModels.Home
{
    /// <summary>
    /// 主页前台 菜单 模型视图类
    /// </summary>
    public class HomeMenuViewModel
    {   
        public int Id { get; set; }
        public string Name { get; set; }
        public int PId { get; set; }

        public bool IsHome { get; set; }
        public bool IsShow { get; set; }
        public int OrderNumber { get; set; }
        public string? Tips { get; set; }


        /// <summary>
        /// 下级菜单
        /// </summary>
        public List<HomeMenuViewModel> Subs { get; set; } = new List<HomeMenuViewModel>();

    }
}
