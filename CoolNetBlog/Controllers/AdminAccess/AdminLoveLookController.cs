using ComponentsServices.Base;
using CoolNetBlog.Base;
using CoolNetBlog.Models;
using CoolNetBlog.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

namespace CoolNetBlog.Controllers.AdminAccess
{
    /// <summary>
    /// 管理员后台 链接显示"看看这些" 管理
    /// </summary>
    public class AdminLoveLookController : BaseAdminController
    {
        private readonly IWebHostEnvironment _environment;
        private static LoveLookViewModel slvm = new();
        private readonly SugarDataBaseStorage<LoveLook, int> _loveLookSet;

        public AdminLoveLookController(IWebHostEnvironment environment) :base()
        {
            this._environment = environment;
            _loveLookSet = new SugarDataBaseStorage<LoveLook, int>();

        }

       
        [HttpPost]
        public async Task<IActionResult> Add(string? pt, LoveLookViewModel vm)
        {
            ModelState.Clear();
            RemoveSomeValid();
            slvm = (LoveLookViewModel)WrapMustNeedPassFields(slvm);
            if (vm is null || vm.Type<=0)
            {
                ModelState.AddModelError("", "添加失败，请返回重试一下吧！");
                return View("LoveLookAmManagement", slvm);
            }
            if (string.IsNullOrWhiteSpace(vm.RelHref))
            {
                ModelState.AddModelError("", "添加失败，要输入链接哦！");
                return View("LoveLookAmManagement", slvm);
            }
            if (string.IsNullOrWhiteSpace(vm.LinkName))
            {
                ModelState.AddModelError("", "添加失败，要输入链接名哦，在主页侧边栏中显示！");
                return View("LoveLookAmManagement", slvm);
            }
            _loveLookSet.TransBegin();
            try
            {
                LoveLook saveModel = new LoveLook
                {
                    RelHref = vm.RelHref,
                    AddedTime = DateTime.Now,
                    LinkName = vm.LinkName,
                    Type = vm.Type,
                };
                if (saveModel.Type==1)
                {
                    if (!saveModel.RelHref.ToLower().StartsWith("detail"))
                    {
                        ModelState.AddModelError("", "添加失败，内部文章链接格式错误，文章详细链接应该是 'Detail' 开头的值(忽略单引号)，请重新查看一下你的文章详情uri。");
                        return View("LoveLookAmManagement", slvm);
                    };
                    // 组成 供点击访问
                    saveModel.RelHref = Path.Combine("/", saveModel.RelHref);
                }

                if (saveModel.Type == 2)
                {
                    // 检测链接文件是否存在
                    var exLinkName = Path.Combine(_environment.WebRootPath, "epLinks", saveModel.RelHref);
                    if (!System.IO.File.Exists(exLinkName))
                    {
                        ModelState.AddModelError("", "添加失败，没有此上传过的链接文件，请先上传文件链接。");
                        return View("LoveLookAmManagement", slvm);
                    }
                    // 组成 供点击访问 epLinks链接文件夹+具体上传的链接文件 名
                    saveModel.RelHref =  "/epLinks/"+saveModel.RelHref.TrimStart('/');
                }
                _loveLookSet.Insert(saveModel);
                _loveLookSet.TransCommit();
                return RedirectToAction("LoveLookAmManagement", "AdminLoveLook", new { pt = slvm.PassToken });
            }
            catch (Exception ex)
            {
                _loveLookSet.TransRoll();
                ModelState.AddModelError("", "添加失败，请返回重试一下吧！");
                return View("LoveLookAmManagement", slvm);
            }
        }



        
        [HttpPost("DeleteLoveLook")]
        public IActionResult DeleteLoveLook(string? pt,int id)
        {
            RemoveSomeValid();
            try
            {
                _loveLookSet.Delete(_loveLookSet.FindOneById(id));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "删除失败 请重试！");
                return RedirectToAction("LoveLookAmManagement", "AdminLoveLook", new { pt = slvm.PassToken });
            }
            // 删除成功 重定向刷新页面
            return RedirectToAction("LoveLookAmManagement", "AdminLoveLook", new { pt = slvm.PassToken });
        }

        [HttpGet]
        public async Task<IActionResult> LoveLookAmManagement(string? pt)
        {

            // 获取所有"看一看"显示的链接
            var data = (await _loveLookSet.GetAllListAsync()).OrderByDescending(a=>a.AddedTime).ToList();

            slvm = new LoveLookViewModel { LoveLooksOrg = data };
            // 自动封装已有的数据
            slvm = (LoveLookViewModel)WrapMustNeedPassFields(slvm);
            return View(slvm);
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
