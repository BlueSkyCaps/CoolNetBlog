namespace CoolNetBlog.ViewModels.Api
{
    /// <summary>
    /// 博客站点 隐私文章需要解锁 传递得到的验证数据模型
    /// </summary>
    public class ArticleUnLockViewModel
    {
        public string Password { get; set; }
        public int ArticleId { get; set; }
        public string Content { get; set; }
        public int Code { get; set; } = -1;
    }
}
