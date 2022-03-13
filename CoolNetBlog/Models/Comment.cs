using SqlSugar;

namespace CoolNetBlog.Models
{
    /// <summary>
    /// 评论表
    /// </summary>
    [SugarTable("Comment")]
    public class Comment
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
        public int SourceId { get; set; }
        /// <summary>
        /// Nullable:False
        /// </summary>
        public int SourceType { get; set; }
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
        public DateTime CommentTime { get; set; }
    }
}
