using SqlSugar;

namespace CoolNetBlog.Models
{
    /// <summary>
    /// 回复评论表
    /// </summary>
    [SugarTable("Reply")]
    public class Reply
    {
        /// <summary>
        /// Desc:Id
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }

        /// <summary>
        /// Nullable:False
        /// </summary>
        public int CommentId { get; set; }
        /// <summary>
        /// Nullable:False
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Nullable:False
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Nullable:False
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// Nullable:true
        /// </summary>
        public bool? IsPassed { get; set; }
        /// <summary>
        /// Nullable:False
        /// </summary>
        public DateTime ReplyTime { get; set; }
    }
}
