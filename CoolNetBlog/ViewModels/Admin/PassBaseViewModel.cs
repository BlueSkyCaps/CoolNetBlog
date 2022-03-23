using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoolNetBlog.ViewModels.Admin
{
    public class PassBaseViewModel
    {
        [Required(ErrorMessage = "管理员昵称是必填项。")]
        [Display(Name = "管理员昵称:")]
        public string AccountName { get; set; }

        /// <summary>
        /// 其值等于当前AdminUser.Token，用于在页面中传递授权
        /// </summary>
        [HiddenInput(DisplayValue = false)]
        public string? PassToken { get; set; }
        [HiddenInput(DisplayValue = false)]
        public string? Email { get; set; }
        /// <summary>
        /// 当前某页面所需展示的公共封装数据
        /// </summary>
        public dynamic? CurrentData { get; set; } 

    }
}
