using Microsoft.AspNetCore.Mvc;

namespace CoolNetBlog.Controllers
{
    public class EpLinkHelperController : Controller
    {
        private readonly IWebHostEnvironment _environment;

        public EpLinkHelperController(IWebHostEnvironment environment)
        {
            this._environment = environment;
        }

        /// <summary>
        /// 这是忽略文件扩展名，将其跳转到wwwroot\epLinks\下具体文件名的html控制操作方法
        /// </summary>
        /// <param name="htmlFileName"></param>
        /// <returns></returns>
        [Route("epLinks/{htmlFileName}")]
        public IActionResult Index(string htmlFileName)
        {
            if (String.IsNullOrWhiteSpace(htmlFileName))
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }
            var uri = $"/epLinks/{htmlFileName}.html";
            if (!System.IO.File.Exists(Path.Combine(_environment.WebRootPath, "epLinks", htmlFileName+".html"))){
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }
            return Redirect(uri);
        }
    }
}
