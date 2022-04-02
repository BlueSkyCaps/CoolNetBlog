using CoolNetBlog.Bll;
using CoolNetBlog.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CoolNetBlog.Controllers
{
    /// <summary>
    /// "闲言碎语"获取接口
    /// </summary>
    public class GossipController : Controller
    {
        private readonly GossipBll _gossipBll;

        public GossipController()
        {
            _gossipBll = new GossipBll();
        }

        
        /// <summary>
        /// 获取当前加载的"闲言碎语"数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetGossips(int index=1, int pageCount = 10)
        {
            var res = await _gossipBll.GetGossipsAsync(index, pageCount);
            return new JsonResult(res);
        }
    }

}
