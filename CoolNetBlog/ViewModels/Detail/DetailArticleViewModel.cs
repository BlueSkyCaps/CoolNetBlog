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
      
        /// <summary>
        /// 文章表态类型数量，文章点赞数ThumbUpStart；文章"有被笑到"数ThumbUpFun；文章"不敢苟同"数ThumbUpSilence
        /// </summary>
        public Dictionary<string,int> ThumbUpNumbers = new Dictionary<string, int>();
    }
}
