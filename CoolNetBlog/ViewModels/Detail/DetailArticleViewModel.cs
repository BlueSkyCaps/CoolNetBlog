using CoolNetBlog.Models;
using CoolNetBlog.ViewModels.Home;
using SqlSugar;

namespace CoolNetBlog.ViewModels.Detail
{
    /// <summary>
    /// 前台某篇具体文章内容 模型视图类
    /// </summary>
    public class DetailArticleViewModel:Article
    {
        /// <summary>
        /// 标签的字符串列表形式 文章详情用到 
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<string> LabelsList { get; set; } = new List<string>();
    }
}
