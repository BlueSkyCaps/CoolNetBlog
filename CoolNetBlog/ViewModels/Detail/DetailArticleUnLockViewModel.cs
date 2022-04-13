namespace CoolNetBlog.ViewModels.Detail
{
    /// <summary>
    /// 博客站点 加锁文章需要解锁 传递得到的验证数据模型
    /// </summary>
    public class DetailArticleUnLockViewModel
    {
        public string Password { get; set; }
        public int ArticleId { get; set; }
        public string Content { get; set; }
        public int Code { get; set; } = -1;
    }
}
