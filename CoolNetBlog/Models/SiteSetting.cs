
using SqlSugar;

namespace CoolNetBlog.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("SiteSetting")]
    public partial class SiteSetting
    {
           public SiteSetting(){


           }
           /// <summary>
           /// Desc:站点名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string SiteName {get;set;}

           /// <summary>
           /// Desc:站点IP
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string Host {get;set;}

           /// <summary>
           /// Desc:站点域名
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string Domain {get;set;}

           /// <summary>
           /// Desc:个性签名
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string FashionQuotes {get;set;}

           /// <summary>
           /// Desc:是否显示站点名
           /// Default:b'1'
           /// Nullable:false
           /// </summary>           
           public bool IsShowSiteName {get;set;}

            /// <summary>
            /// Desc:是否侧边栏也显示搜索框组件
            /// Default:b'1'
            /// Nullable:false
            /// </summary>     
            public bool IsShowEdgeSearch { get;set;}

            /// <summary>
            /// Desc:是否显示侧边栏 "看看这些"组件
            /// Default:b'0'
            /// Nullable:false
            /// </summary>     
            public bool IsShowLoveLook { get;set;}

           /// <summary>
           /// Desc:是否显示个性签名
           /// Default:b'1'
           /// Nullable:false
           /// </summary>           
           public bool IsShowQutoes {get;set;}

           /// <summary>
           /// Desc:备案号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string Cban {get;set;}

           /// <summary>
           /// Desc:尾部文字内容
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string TailContent {get;set;}

            /// <summary>
            /// Desc:主页列表展示文章的条数
            /// Default:5
            /// Nullable:false
            /// </summary>      
            public int OnePageCount { get;set;}
            /// <summary>
            /// 后台登录入口Value参数设置的值，若设置了值，必须验证正确的值显示登录界面
            /// </summary>
            public string LoginUriValue { get; set; }
            /// <summary>
            /// 显示链接("侧边栏看一看")自定义标题文本
            /// </summary>
            public string? LoveLookTitle { get; set; }
            /// <summary>
            /// 侧边栏"心愿图片"展示路径 在[文件图片]板块中设置心愿图片
            /// </summary>
            public string? WishPictureRelPath { get; set; }
            /// <summary>
            /// 是否展示"心愿图片"
            /// </summary>
            public bool IsShowWishPicture { get; set; }
            /// <summary>
            /// "心愿图片"标题祝福语
            /// </summary>
            public string? WishPictureName { get; set; }
            /// <summary>
            /// Desc:是否开启文章点赞表态功能
            /// Default:b'0'
            /// Nullable:false
            /// </summary>      
            public bool IsOpenDetailThumb { get; set; }
        
            /// <summary>
            /// Desc:ip一日内允许的留言次数(包括回复、评论)，0为无限制
            /// Default:b'0'
            /// Nullable:true
            /// </summary>      
            public int? LeaveLimitCount { get; set; }

    }
}
