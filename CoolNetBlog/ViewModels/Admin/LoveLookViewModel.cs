namespace CoolNetBlog.ViewModels.Admin
{
    public class LoveLookViewModel:PassBaseViewModel
    {
        public int Id { get; set; }
        public DateTime AddedTime { get; set; }
        public string RelHref { get; set; }
        public string LinkName { get; set; }
        public int Type { get; set; }
        //↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑post表单属性

        public IList<Models.LoveLook> LoveLooksOrg { get; set; } = new List<Models.LoveLook>();
    }
}
