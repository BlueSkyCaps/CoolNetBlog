using ComponentsServices.Base;
using CoolNetBlog.Base;
using CoolNetBlog.Models;
using CoolNetBlog.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

namespace CoolNetBlog.Controllers.AdminAccess
{
    /// <summary>
    /// 后台基本配置操作
    /// </summary>
    public class AdminSiteSettingController : BaseAdminController
    {
        private SugarDataBaseStorage<SiteSetting, int> _siteSettingSet;

        public AdminSiteSettingController() : base()
        {
            _siteSettingSet = new SugarDataBaseStorage<SiteSetting, int>();
        }

        /// <summary>
        /// 移除有些场景下表单不需要验证的属性
        /// </summary>
        private void RemoveSomeValid()
        {
            ModelState.Remove("AccountName");

        }

        /// <summary>
        /// 更新站点的基本配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditSiteSetting(SiteSettingViewModel vm)
        {
            RemoveSomeValid();

            if (vm is null)
            {
                ModelState.AddModelError("", "更新失败:传递获取不到模型，重启浏览器再试吧？。");
                return View(vm);
            }
            vm = (SiteSettingViewModel)WrapMustNeedPassFields(vm);
            if (vm.OnePageCount<=0)
            {
                ModelState.AddModelError("", "更新失败:主页列表展示文章的条数不能是0。");
                return View(vm);
            }

            SiteSetting saveEntity = new SiteSetting
            {
                SiteName = vm.SiteName,
                IsShowSiteName = vm.IsShowSiteName,
                IsShowQutoes = vm.IsShowQutoes,
                Cban = vm.Cban,
                Domain = vm.Domain,
                FashionQuotes = vm.FashionQuotes,
                Host = vm.Host,
                TailContent = vm.TailContent,
                OnePageCount = vm.OnePageCount,
                LoginUriValue = vm.LoginUriValue,
                LoveLookTitle = vm.LoveLookTitle,
                IsShowEdgeSearch = vm.IsShowEdgeSearch,
                IsShowLoveLook = vm.IsShowLoveLook,
                WishPictureRelPath = vm.WishPictureRelPath,
                IsShowWishPicture = vm.IsShowWishPicture,
                WishPictureName = vm.WishPictureName,
                IsOpenDetailThumb = vm.IsOpenDetailThumb,
            };

            _siteSettingSet.TransBegin();
            try
            {
                // 虽然是全删除 但也只有一个站点数据
                await _siteSettingSet._dbHandler.Deleteable<SiteSetting>().Where(s=>1==1).ExecuteCommandAsync();
                await _siteSettingSet.InsertAsync(saveEntity);
                _siteSettingSet.TransCommit();
            }
            catch (Exception)
            {
                _siteSettingSet.TransRoll();
                ModelState.AddModelError("", "更新失败回滚，重启浏览器再试吧？。");
                return View(vm);
            }
            return RedirectToAction("AdminHome", "Admin", new { pt = vm.PassToken });

        }

        /// <summary>
        /// 站点的基本配置修改页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> EditSiteSetting(string? pt)
        {
           var eny =  await _siteSettingSet._dbHandler.Queryable<SiteSetting>().FirstAsync();
            SiteSettingViewModel org = new SiteSettingViewModel
            {
                SiteName = eny.SiteName,
                IsShowSiteName = eny.IsShowSiteName,
                IsShowQutoes = eny.IsShowQutoes,
                Cban = eny.Cban,
                Domain = eny.Domain,
                FashionQuotes = eny.FashionQuotes,
                Host = eny.Host,
                OnePageCount = eny.OnePageCount,
                TailContent = eny.TailContent,
                LoginUriValue = eny.LoginUriValue,
                LoveLookTitle = eny.LoveLookTitle,
                IsShowEdgeSearch = eny.IsShowEdgeSearch,
                IsShowLoveLook = eny.IsShowLoveLook,
                WishPictureRelPath = eny.WishPictureRelPath,
                IsShowWishPicture = eny.IsShowWishPicture,
                WishPictureName = eny.WishPictureName,
                IsOpenDetailThumb = eny.IsOpenDetailThumb,
            };
            // 自动封装已有的数据
            org = (SiteSettingViewModel)WrapMustNeedPassFields(org);
            return View(org);
        }
    }
}
