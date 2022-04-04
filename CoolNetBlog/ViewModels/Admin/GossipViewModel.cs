using CoolNetBlog.Models;

namespace CoolNetBlog.ViewModels.Admin
{
    public class GossipViewModel:PassBaseViewModel
    {
        //----⬇⬇⬇⬇⬇⬇⬇⬇⬇⬇⬇ POST表单属性
        public int Id { get; set; }

        public int Type { get; set; }

        public string? ImgUrl { get; set; }
        public string Content { get; set; }

        public DateTime AddTime { get; set; }

        public int StarNumber { get; set; }
        // -----⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆⬆

        public IList<Gossip> GossipesOrg { get; set; } = new List<Gossip>();
        public string? Keyword { get; set; }

        public int Index { get; set; } = 1;
        public int NextIndex { get; set; } = 2;
        public int PreIndex { get; set; } = 1;

    }
}
