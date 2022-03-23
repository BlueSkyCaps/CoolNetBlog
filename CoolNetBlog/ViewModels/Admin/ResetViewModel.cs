using System.ComponentModel.DataAnnotations;

namespace CoolNetBlog.ViewModels.Admin
{
    public class ResetViewModel : PassBaseViewModel
    {
        public string OrgPassword { get; set; } = "";
        public string OrgAccountName { get; set; } = "";
        public string NewAccountName { get; set; } = "";
        public string NewPassword { get; set; } = "";
        public string NewPasswordRep { get; set; } = "";
    }
}
