using ComponentsServices.Base;
using CoolNetBlog.Base;
using CoolNetBlog.Models;
using CoolNetBlog.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

namespace CoolNetBlog.Controllers.AdminAccess
{
    /// <summary>
    /// 管理员后台 上传图片操作
    /// </summary>
    public class AdminFileController : BaseAdminController
    {
        private readonly IWebHostEnvironment _environment;
        private static FilePathViewModel sfvm = new();
        private readonly SugarDataBaseStorage<FilePath, int> _fileSet;

        public AdminFileController(IWebHostEnvironment environment) :base()
        {
            this._environment = environment;
            _fileSet = new SugarDataBaseStorage<FilePath, int>();

        }

        /// <summary>
        /// 上传文件 
        /// 判断文件是否存在：
        /// 图片格式是根据助记名称判断是否唯一，因为图片名称就是输入的助记名称+后缀格式
        /// 其余链接是根据文件名直接判断是否存在，不存储助记名称
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="helpName">图片助记名称</param>
        /// <param name="type">文件格式 图片img 其余other</param>
        /// <param name="file"></param>
        /// <returns></returns>
        /// ！！！使用RedirectToAction重定向刷新页面以及TempData一次性提示信息(有效解决办法而不需要js额外的处理)，而不是返回View视图，
        /// ！！！因为这样会导致input元素File上传会多次重复上传之前的文件导致错误(删除接口有时也不会被率先执行)
        [HttpPost]
        public async Task<IActionResult> UpFile(string? pt, string? helpName,string? type, IFormFile file)
        {
            ModelState.Clear();
            RemoveSomeValid();    
            sfvm = (FilePathViewModel)WrapMustNeedPassFields(sfvm);
            if (file is null || file.FileName is null)
            {
                TempData["Tips"] = "上传失败 没有选择文件。";
                //ModelState.AddModelError("", "上传失败 没有选择文件。");
                return RedirectToAction("FileAmManagement", "AdminFile", new { pt = sfvm.PassToken });


            }
            var format = file.FileName?.Split('.')?.LastOrDefault();
            var fileRoot = "";
            var saveDbfileRelPath = "";
            _fileSet.TransBegin();
            try
            {
                var pathList = new List<string>();
                if (type=="img")
                {
                    if (!file.ContentType.ToLower().Contains("image"))
                    {
                        TempData["Tips"] = "上传失败 请上传图片哦。";
                        return RedirectToAction("FileAmManagement", "AdminFile", new { pt = sfvm.PassToken });
                    }
                    // 若是图片文件
                    if (String.IsNullOrWhiteSpace(helpName))
                    {
                        TempData["Tips"] = "上传失败 图片助记名称要填哦！";
                        return RedirectToAction("FileAmManagement", "AdminFile", new { pt = sfvm.PassToken });


                    }
                    helpName = helpName.Trim();
                    if (_fileSet.Any(f=>f.HelpName== helpName))
                    {
                        TempData["Tips"] = "上传失败 此图片助记名称已经被其他图片使用了！";
                        return RedirectToAction("FileAmManagement", "AdminFile", new { pt = sfvm.PassToken });


                    }
                    if ((file.Length / 1024.0) > 200)
                    {
                        TempData["Tips"] = "上传失败 图片大小大于200kb！";
                        return RedirectToAction("FileAmManagement", "AdminFile", new { pt = sfvm.PassToken });


                    }
                    // 实际保存物理路径
                    fileRoot = Path.Combine(_environment.WebRootPath, "articleImgs", helpName+'.'+format);
                    // 保存到数据库中的对应相对物理文件路径的名称
                    saveDbfileRelPath = helpName + '.' + format;
                }
                else
                {
                    // html等其他链接
                    var allowLinkType = new List<string>() { "htm", "pdf", "powerpoint", "excel", "word" };
                    var allowed = false;
                    allowLinkType.ForEach(a => 
                        {
                            // 若是列表中允许的连接格式
                            if (file.ContentType.ToLower().Contains(a))
                            {
                                allowed = true;
                                return;
                            }
                        }
                    );
                    if (!allowed)
                    {
                        TempData["Tips"] = "上传失败 不是允许上传的链接格式";
                        return RedirectToAction("FileAmManagement", "AdminFile", new { pt = sfvm.PassToken });

                    }
                    // 实际保存物理路径
                    fileRoot = Path.Combine(_environment.WebRootPath, "epLinks", file.FileName);
                    // 保存到数据库中的对应相对物理文件路径的名称
                    saveDbfileRelPath = file.FileName;

                    if (_fileSet.Any(f=>f.FileRelPath== saveDbfileRelPath))
                    {
                        TempData["Tips"] = "上传失败 该文件名已经存在 你可以先删除再尝试重新上传，或者重命名文件名后再上传哦";
                        return RedirectToAction("FileAmManagement", "AdminFile", new { pt = sfvm.PassToken });

                    }
                }

                using (var stream = file?.OpenReadStream())
                {
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    // 存在则覆盖文件
                    System.IO.File.WriteAllBytes(fileRoot, bytes);
                }
                FilePath saveModel = new FilePath
                {
                    FileRelPath = saveDbfileRelPath,
                    HelpName = helpName,
                    Type = type,
                    UploadTime = DateTime.Now,
                };
                _fileSet.Insert(saveModel);
                _fileSet.TransCommit();
                return RedirectToAction("FileAmManagement", "AdminFile", new { pt = sfvm.PassToken });
            }
            catch (Exception ex)
            {
                _fileSet.TransRoll();
                TempData["Tips"] = "上传失败 请重试！";
                return RedirectToAction("FileAmManagement", "AdminFile", new { pt = sfvm.PassToken });
            }
        }



        /// <summary>
        /// 删除文件 
        /// </summary>
        /// <param name="id">FilePath表的Id</param>
        /// <param name="type">文件格式类型，img | other</param>
        /// <param name="type">文件实际相对路径</param>
        /// <returns></returns>
        /// ！！！使用RedirectToAction重定向刷新页面以及TempData一次性提示信息(有效解决办法而不需要js额外的处理)，而不是返回View视图，
        /// ！！！因为这样会导致input元素File上传会多次重复上传之前的文件导致错误(删除接口有时也不会被率先执行)
        [HttpPost("Delete")]
        public IActionResult Delete(string? pt,int id,string type, string fileRelPath)
        {
            RemoveSomeValid();
            if (string.IsNullOrWhiteSpace(fileRelPath)|| id<=0)
            {
                TempData["Tips"] = "删除失败 请重试！";
                return RedirectToAction("FileAmManagement", "AdminFile", new { pt = sfvm.PassToken });
            }
            string? root;
            if (type.ToLower()=="img")
            {
                root = Path.Combine(_environment.WebRootPath, "articleImgs", fileRelPath);
            }
            else
            {
                root = Path.Combine(_environment.WebRootPath, "epLinks", fileRelPath);

            }
            try
            {
                if (System.IO.File.Exists(root))
                    System.IO.File.Delete(root);
                var _fileSet = new SugarDataBaseStorage<FilePath, int>();
                _fileSet.Delete(_fileSet.FindOneById(id));
            }
            catch (Exception)
            {
                TempData["Tips"] = "删除失败 请重试！";
                return RedirectToAction("FileAmManagement", "AdminFile", new { pt = sfvm.PassToken });
            }
            // 删除成功 重定向刷新页面
            return RedirectToAction("FileAmManagement", "AdminFile", new { pt = sfvm.PassToken });
        }

        [HttpGet]
        public async Task<IActionResult> FileAmManagement(string? pt, string? kw)
        {

            IList<FilePath> fileImgPaths;
            IList<FilePath> fileOtherPaths;
            if (!string.IsNullOrWhiteSpace(kw))
            {
                // 有关键字 过滤非图片格式 再根据关键字过滤
                fileImgPaths = await _fileSet.GetListBuilder().Where(a => a.Type.ToLower() == "img")
                    .Where(a =>
                    (a.HelpName != null && a.HelpName.Contains(kw)) ||
                    (a.FileRelPath != null && a.FileRelPath.Contains(kw)))
                    .OrderBy(a => a.UploadTime, SqlSugar.OrderByType.Desc).Take(10).ToListAsync();
            }
            else
            {
                // 过滤非图片格式
                fileImgPaths = await _fileSet.GetListBuilder().Where(a => a.Type.ToLower()=="img")
                    .OrderBy(a => a.UploadTime, SqlSugar.OrderByType.Desc).Take(10).ToListAsync();
            }

            // 再获取所有链接路径
            fileOtherPaths = await _fileSet.GetListBuilder().Where(a => a.Type.ToLower()=="other")
                    .OrderBy(a => a.UploadTime, SqlSugar.OrderByType.Desc).ToListAsync();

            sfvm = new FilePathViewModel { FileImgPathsOrg = fileImgPaths, FileOtherPathsOrg= fileOtherPaths };
            // 自动封装已有的数据
            sfvm = (FilePathViewModel)WrapMustNeedPassFields(sfvm);
            return View(sfvm);
        }

        /// <summary>
        /// 设置侧边栏"心愿图片"展示的图片路径
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SetWishPicture(string? pt, string fileRelPath)
        {
            var orgSiteSetting = (await _bdb._dbHandler.Queryable<SiteSetting>().FirstAsync());
            if (orgSiteSetting == null || string.IsNullOrWhiteSpace(fileRelPath)) {
                TempData["Tips"] = "设置侧边栏'心愿图片'发生错误，请重试吧！？？";
                return RedirectToAction("FileAmManagement", "AdminFile", new { pt = sfvm.PassToken });
            }
            try
            {
                await _bdb._dbHandler.Ado.ExecuteCommandAsync($"update SiteSetting set WishPictureRelPath='{fileRelPath}'");
                return RedirectToAction("FileAmManagement", "AdminFile", new { pt = sfvm.PassToken });
            }
            catch (Exception e)
            {
                TempData["Tips"] = "设置侧边栏'心愿图片'发生错误，请重试吧！？？";
                return RedirectToAction("FileAmManagement", "AdminFile", new { pt = sfvm.PassToken });
            }
        }

        /// <summary>
        /// 移除有些场景下表单不需要验证的属性
        /// </summary>
        private void RemoveSomeValid()
        {
            ModelState.Remove("AccountName");
        }
    }
}
