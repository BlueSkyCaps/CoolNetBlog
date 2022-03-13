using CoolNetBlog.Models;

namespace CoolNetBlog.ViewModels.Admin
{
    /// <summary>
    /// 文章操作模型视图类
    /// </summary>
    public class ArticleViewModel : PassBaseViewModel
    {   //----⬇⬇⬇⬇⬇⬇⬇⬇⬇⬇⬇ POST表单属性
        public int Id { get; set; }
        public int MenuId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsShowTitle { get; set; }
        public string Abstract { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public bool IsLock { get; set; }
        public string LockPassword { get; set; }
        public bool IsDraft { get; set; }
        public string? Labels { get; set; }
        public string? CustUri { get; set; }
        public int CommentType { get; set; }

        // -----⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆

        /// <summary>
        /// 是否有至少一个菜单 没有避免点击新增文章按钮
        /// </summary>
        public bool HasAnyOneMenu { get; set; }

        /// <summary>
        /// 列表迭代显示的文章属性原值
        /// </summary>
        public IList<Article> ArticlesOrg { get; set; } = new List<Article>();
        /// <summary>
        /// 用于设置文章的存放菜单
        /// </summary>
        public IList<SelectList> MenuSelectList { get; set; } = new List<SelectList>();
        /// <summary>
        /// 当前文章关联的菜单实体
        /// </summary>
        public Menu RelatedMenu { get; set; } = new Menu();

        /// <summary>
        /// 图片实际名 图片名称地址列表 供文章编辑时选择插入的图片
        /// </summary>
        public List<FilePath> ImgRelPaths { get; set; } = new List<FilePath>();   
       

    }
}
