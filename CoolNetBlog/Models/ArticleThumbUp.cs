using SqlSugar;

namespace CoolNetBlog.Models
{
    /// <summary>
    /// 文章点赞表
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
    }
}
