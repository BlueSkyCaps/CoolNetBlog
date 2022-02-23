using CoolNetBlog.Base;
using CoolNetBlog.Bll;
using Microsoft.AspNetCore.Mvc;

namespace CoolNetBlog.Controllers.Home
{
    /// <summary>
    /// 文章帖子详细页面控制器
    /// </summary>
    public class DetailController : BaseController
    {
        private readonly ILogger<DetailController> _logger;
        private readonly DetailLogicBll _detailBll;

        public DetailController(ILogger<DetailController> logger) : base()
        {
            _logger = logger;
            _detailBll = new DetailLogicBll();
            WrapsGlobalHomeData();
        }
        [Route("{Controller}/")]
        [Route("{Controller}/{custUri}")]
        public async Task<IActionResult> Index(int? articleId, string? custUri)
        {
            await _detailBll.DealArticleEntityAsync(_homeGlobalView, articleId, custUri);
            return View(_homeGlobalView);
        }
    }
}
