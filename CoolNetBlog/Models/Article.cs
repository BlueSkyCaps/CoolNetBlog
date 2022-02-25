using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace CoolNetBlog.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("Article")]
    public partial class Article
    {
           public Article(){


           }
           /// <summary>
           /// Desc:文章(帖子)自增主键
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public int Id {get;set;}

           /// <summary>
           /// Desc:所属菜单Id
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int MenuId {get;set;}

           /// <summary>
           /// Desc:文章标题
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string Title {get;set;}

           /// <summary>
           /// Desc:文章内容
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string Content {get;set;}

           /// <summary>
           /// Desc:是否展示此文章的标题
           /// Default:
           /// Nullable:False
           /// </summary>           
           public bool IsShowTitle {get;set;}

           /// <summary>
           /// Desc:文章自定义摘要描述
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string Abstract {get;set;}

           /// <summary>
           /// Desc:首次创建时间
           /// Default:
           /// Nullable:False
           /// </summary>           
           public DateTime CreatedTime {get;set;}

           /// <summary>
           /// Desc:最后更新时间
           /// Default:
           /// Nullable:False
           /// </summary>
           public DateTime UpdateTime {get;set;}

           /// <summary>
           /// Desc:是否隐私文章，隐私文章需在页面解锁才显示主体内容
           /// Default:b'0'
           /// Nullable:False
           /// </summary>           
           public bool IsLock {get;set;}

           /// <summary>
           /// Desc:隐私文章密码
           /// Default:
           /// Nullable:true
           /// </summary>           
           public string LockPassword {get;set;}

           public bool IsDraft { get;set;}

        /// <summary>
        /// Desc:标签 标签字符串：“xx,xx..”|"xx，xx.."|"xx xx.."
        /// Default:
        /// Nullable:true
        /// </summary>           
        public string? Labels { get; set; }
        /// <summary>
        /// 自定义文章uri，唯一，可通过此字段或Id找寻文章
        /// Nullable:true
        /// </summary>
        public string? CustUri { get; set; }
        /// <summary>
        /// 文章点赞数
        /// </summary>
        public int ThumbUpStart { get;set;}
        /// <summary>
        /// 文章所属菜单名 非字段 只用于显示
        /// </summary>
        [SugarColumn(IsIgnore=true)]
        public string Ig_MenuName { get; set; }

    }
}
