using ComponentsServices.Base;
using CoolNetBlog.Base;
using CoolNetBlog.Models;
using CoolNetBlog.ViewModels;
using CoolNetBlog.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

namespace CoolNetBlog.Controllers.Admin
{

    /// <summary>
    /// 后台菜单操作
    /// </summary>

    public class AdminMenuController : BaseAdminController
    {
        private static MenuViewModel smvm = new();
        private SugarDataBaseStorage<Menu, int> _menuSet;
        private SugarDataBaseStorage<Article, int> _articleSet;

        public AdminMenuController():base()
        {
            _menuSet = new SugarDataBaseStorage<Menu, int>();
            _articleSet = new SugarDataBaseStorage<Article, int>();
        }

        /// <summary>
        /// 根据Id删除一个菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Delete([FromForm]int id) {
            if (id<1)
            {
                ModelState.AddModelError("", "删除失败:菜单Id无效，重启浏览器再试吧？。");
                return View("MenuAmManagement", smvm);
            }
            Menu delable;
            try
            {
                delable = await _menuSet.FindOneByIdAsync(id);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "删除失败:没有此菜单Id，刷新再试吧？。");
                return View("MenuAmManagement", smvm);
            }
            bool exd = await _articleSet.AnyAsync(a=>a.MenuId==id);
            if (exd)
            {
                ModelState.AddModelError("", "删除失败:还有文章属于此菜单，你可以将文章归类为其他菜单再尝试删除;" +
                    "也可以重新命名为合适的菜单名哦！");
                return View("MenuAmManagement", smvm);
            }
            exd = await _menuSet.AnyAsync(m => m.PId == id);
            if (exd)
            {
                ModelState.AddModelError("", "删除失败:还有下级菜单属于此菜单，你可以先删除此菜单的下级菜单。");
                return View("MenuAmManagement", smvm);
            }
            var ef = await _menuSet.DeleteAsync(delable);
            return RedirectToAction("MenuAmManagement", "AdminMenu", new { pt = smvm.PassToken });

        }

        /// <summary>
        /// 更新菜单实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Update([FromForm] int id, MenuViewModel vm)
        {
            RemoveSomeValid();
            if (vm is null)
            {
                ModelState.AddModelError("", "更新失败:传递获取不到菜单模型，重启浏览器再试吧？。");
                return View("MenuAmManagement", smvm);
            }
            if (string.IsNullOrWhiteSpace(vm.Name))
            {
                ModelState.AddModelError("", "新增失败:菜单名是需要的。");
                return View("MenuAmManagement", smvm);
            }
            Menu orgEntity;
            try
            {
                orgEntity = await _menuSet.FindOneByIdAsync(id);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "更新失败:根据菜单Id找不到对应的原菜单实体，刷新再试吧？。");
                return View("MenuAmManagement", smvm);
            }
            var selectedPMenu = await _menuSet.FindOneByIdAsync(vm.MenuSelectedValue);
            // 开始验证父菜单逻辑 若当前菜单设置的父菜单已是二级菜单(也本就有父菜单)
            if (vm.MenuSelectedValue!=0)
            {
                if (selectedPMenu != null&& selectedPMenu.PId!=0)
                {
                    ModelState.AddModelError("", $"更新失败:此菜单选定的父菜单({selectedPMenu.Name})已是二级菜单，无法设置。菜单的父菜单必须是顶级菜单。");
                    return View("MenuAmManagement", smvm);
                }
            }

            // 若所选的父菜单是主页菜单，主页菜单不能有二级菜单
            if (selectedPMenu != null && selectedPMenu.IsHome)
            {
                ModelState.AddModelError("", $"更新失败:此菜单选定的父菜单({selectedPMenu.Name})是主页菜单，无法设置。主页菜单不能有下级菜单。");
                return View("MenuAmManagement", smvm);
            }

            // 若是主页菜单想隐藏
            if (vm.IsHome && !vm.IsShow)
            {
                ModelState.AddModelError("", $"更新失败:主页菜单，无法设置隐藏。");
                return View("MenuAmManagement", smvm);
            }

            orgEntity.PId = vm.MenuSelectedValue;
            orgEntity.IsHome = vm.IsHome;
            orgEntity.OrderNumber = vm.OrderNumber;
            orgEntity.Name = vm.Name;
            orgEntity.IsShow = vm.IsShow;
            orgEntity.Tips = vm.Tips;
            _menuSet.TransBegin();
            try
            {
                // 设置为主页菜单。将之前的主页菜单设为false
                if (vm.IsHome)
                {
                    await _menuSet.UpdateBySomeColExpAsync(c=>c.IsHome== false, p=>p.IsHome==true);
                }
                await _menuSet.UpdateAsync(orgEntity);
                _menuSet.TransCommit();
            }
            catch (Exception)
            {
                _menuSet.TransRoll();
                ModelState.AddModelError("", "更新失败回滚，重启浏览器再试吧？。");
                return View("MenuAmManagement", smvm);
            }
            return RedirectToAction("MenuAmManagement", "AdminMenu", new { pt = smvm.PassToken });

        }


        /// <summary>
        /// 新增菜单实体
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Add(MenuViewModel vm)
        {
            RemoveSomeValid();
            if (vm is null)
            {
                ModelState.AddModelError("", "新增失败:传递获取不到菜单模型，重启浏览器再试吧？。");
                return View("MenuAmManagement", smvm);
            }
            if (string.IsNullOrWhiteSpace(vm.Name))
            {
                ModelState.AddModelError("", "新增失败:菜单名是需要的。");
                return View("MenuAmManagement", smvm);
            }
            Menu addEntity = new Menu();
           
            var selectedPMenu = await _menuSet.FindOneByIdAsync(vm.MenuSelectedValue);
            // 开始验证父菜单逻辑 若当前菜单设置的父菜单已是二级菜单(已经本就有父菜单)
            if (vm.MenuSelectedValue != 0)
            {
                if (selectedPMenu != null && selectedPMenu.PId != 0)
                {
                    ModelState.AddModelError("", $"更新失败:此菜单选定的父菜单({selectedPMenu.Name})已是二级菜单，无法设置。菜单的父菜单必须是顶级菜单。");
                    return View("MenuAmManagement", smvm);
                }
            }
            // 若所选的父菜单是主页菜单，主页菜单不能有二级菜单
            if (selectedPMenu!=null&&selectedPMenu.IsHome)
            {
                ModelState.AddModelError("", $"更新失败:此菜单选定的父菜单({selectedPMenu.Name})是主页菜单，无法设置。主页菜单不能有下级菜单。");
                return View("MenuAmManagement", smvm);
            }

            // 若是主页菜单想隐藏
            if (vm.IsHome && !vm.IsShow)
            {
                ModelState.AddModelError("", $"更新失败:主页菜单，无法设置隐藏。");
                return View("MenuAmManagement", smvm);
            }

            addEntity.PId = vm.MenuSelectedValue;
            addEntity.IsHome = vm.IsHome;
            addEntity.OrderNumber = vm.OrderNumber;
            addEntity.Name = vm.Name;
            addEntity.IsShow = vm.IsShow;
            addEntity.Tips = vm.Tips;
            _menuSet.TransBegin();
            try
            {
                // 设置为主页菜单。将之前的主页菜单设为false
                if (vm.IsHome)
                {
                    await _menuSet.UpdateBySomeColExpAsync(c => c.IsHome == false, p => p.IsHome == true);
                }
                await _menuSet.InsertAsync(addEntity);
                _menuSet.TransCommit();
            }
            catch (Exception)
            {
                _menuSet.TransRoll();
                ModelState.AddModelError("", "新增失败回滚，重启浏览器再试吧？。");
                return View("MenuAmManagement", smvm);
            }
            return RedirectToAction("MenuAmManagement", "AdminMenu", new { pt = smvm.PassToken });

        }


        /// <summary>
        /// 菜单管理页面入口
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> MenuAmManagement(string? pt)
        {
            // 获取菜单列表
            var _menuSet = new SugarDataBaseStorage<Menu, int>();
            var menus = await _menuSet.GetAllListAsync();
            smvm = new MenuViewModel { MenusOrg = menus };
            // 封装菜单下拉框选择列表，用于在设置归属菜单时显示
            foreach (var item in menus)
            {
                smvm.MenuSelectList.Add(new SelectList { Value = item.Id, Text = item.Name });
            }
            // 自动封装已有的数据
            smvm = (MenuViewModel)WrapMustNeedPassFields(smvm);
            return View(smvm);
        }


        /// <summary>
        /// 移除有些场景下表单不需要验证的属性
        /// </summary>
        private void RemoveSomeValid()
        {
            ModelState.Remove("AccountName");
            ModelState.Remove("Name");
        }
    }
}
