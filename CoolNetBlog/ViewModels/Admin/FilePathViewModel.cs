using CoolNetBlog.Models;

namespace CoolNetBlog.ViewModels.Admin
{
    public class FilePathViewModel : PassBaseViewModel
    {
        //----⬇⬇⬇⬇⬇⬇⬇⬇⬇⬇⬇ POST表单属性
        public int Id { get; set; }
        public string FileRelPath { get; set; }
        public string HelpName { get; set; }
        public string Type { get; set; }
        public DateTime UploadTime { get; set; }
        //-----⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆
        /// <summary>
        /// 迭代显示FilePath列表原值
        /// </summary>
        public IList<FilePath> FileImgPathsOrg { get; set; } = new List<FilePath>();
        public IList<FilePath> FileOtherPathsOrg { get; set; } = new List<FilePath>();
    }
}
