using CoolNetBlog.Base;
using CoolNetBlog.Bll;
using CoolNetBlog.ViewModels.Detail;
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

        /// <summary>
        /// 隐私文章解锁接口
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// 指定路由为{Controller}/UnLock/ArticleUnLock，否则{Controller}/ArticleUnLock直接走Index方法自定义文章uri了
        [HttpPost]
        [Route("{Controller}/UnLock/ArticleUnLock")]
        public JsonResult ArticleUnLock([FromBody]ArticleUnLockViewModel data)
        {
            var res = _detailBll.DealArticleUnLock(data);
            return new JsonResult(res);
        }
    }
}
