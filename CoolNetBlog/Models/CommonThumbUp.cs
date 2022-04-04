using SqlSugar;

namespace CoolNetBlog.Models
{
    /// <summary>
    /// 统一点赞表
    /// </summary>
    [SugarTable("CommonThumbUp")]
    public class CommonThumbUp
    {
        /// <summary>
        /// Default:源Id
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true)]
        public int SourceId { get; set; }
        /// <summary>
        /// Default:源Id所属类型：1"闲言碎语"组件表，2.. 
        /// (文章点赞不使用此表而是专门的文章点赞表ArticleThumbUp)
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true)]
        public int SourceType { get; set; }

        /// <summary>
        /// Desc:客户端点赞文章的Ip
        /// Default:
        /// Nullable:False
        /// </summary>         
        [SugarColumn(IsPrimaryKey = true)]
        public string ClientIp { get; set; }
        public string? ClientDevice { get; set; }
        public string? ClientBrowser { get; set; }
        public DateTime? UpTime { get; set; }
    }
}
