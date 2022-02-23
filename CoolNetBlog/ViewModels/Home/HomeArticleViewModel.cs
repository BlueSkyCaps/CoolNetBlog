using CoolNetBlog.Models;
using SqlSugar;

namespace CoolNetBlog.ViewModels.Home
{
    /// <summary>
    /// 前台文章 模型视图类
    /// </summary>
    public class HomeArticleViewModel:Article
    {


        /// <summary>
        /// 当前文章关联的菜单实体  非字段 只用于显示
        /// </summary
        [SugarColumn(IsIgnore = true)]
        public Menu RelatedMenu { get; set; } = new Menu();


       

    }
}
