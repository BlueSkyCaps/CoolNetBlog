using CoolNetBlog.Models;

namespace CoolNetBlog.ViewModels.Admin
{
    /// <summary>
    /// 菜单操作模型视图类
    /// </summary>
    public class MenuViewModel:PassBaseViewModel
    {   //----⬇⬇⬇⬇⬇⬇⬇⬇⬇⬇⬇ POST表单属性
        public int Id { get; set; }
        public string Name { get; set; }
        public int PId { get; set; }

        public bool IsHome { get; set; }
        public bool IsShow { get; set; }
        public int OrderNumber { get; set; }
        public string? Tips { get; set; }
        // -----⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆

        /// <summary>
        /// 列表迭代显示的菜单属性原值
        /// </summary>
        public IList<Menu> MenusOrg { get; set; } = new List<Menu>();
        /// <summary>
        /// 菜单下拉框选择列表选定的值
        /// </summary>
        public int MenuSelectedValue { get; set; }
        /// <summary>
        /// 菜单下拉框选择列表
        /// </summary>
        public List<SelectList> MenuSelectList { get; set; } = new List<SelectList>();

    }
}
