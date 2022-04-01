using SqlSugar;

namespace CoolNetBlog.Models
{
    /// <summary>
    /// 文章专门点赞表
    /// (文章点赞使用此表 而"闲言碎语"组件等是用统一的点赞表CommonThumbUp)
    /// </summary>
    [SugarTable("ArticleThumbUp")]
    public class ArticleThumbUp
    {
        /// <summary>
        /// Desc:文章Id
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true)]
        public int ArticleId { get; set; }

        /// <summary>
        /// Desc:客户端点赞文章的Ip
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string ClientIp { get; set; }
        public string? ClientDevice { get; set; }
        public string? ClientBrowser { get; set; }
        public DateTime? UpTime { get; set; }
        /// <summary>
        /// 类型：1觉得很赞2有被笑到3不敢苟同
        /// </summary>
        public int? Type { get; set; }
    }
}
