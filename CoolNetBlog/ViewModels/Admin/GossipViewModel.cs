namespace CoolNetBlog.ViewModels.Admin
{
    public class GossipViewModel:PassBaseViewModel
    {
        public int Id { get; set; }

        public int Type { get; set; }

        public string? ImgUrl { get; set; }
        public string Content { get; set; }

        public DateTime AddTime { get; set; }

        public int StarNumber { get; set; }
    }
}
