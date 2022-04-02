namespace CoolNetBlog.ViewModels.Admin
{
    public class SiteSettingViewModel:PassBaseViewModel
    {
        public string SiteName { get; set; }
        public string Host { get; set; }
        public string Domain { get; set; }
        public string FashionQuotes { get; set; }
        public bool IsShowSiteName { get; set; }
        public bool IsShowQutoes { get; set; }
        public string Cban { get; set; }
        public string TailContent { get; set; }
        public int OnePageCount { get; set; }
        public string? LoveLookTitle { get; set; }
        public string LoginUriValue { get; set; }
        public bool IsShowEdgeSearch { get; set; }
        public bool IsShowLoveLook { get; set; }
        public bool IsShowWishPicture { get; set; }
        public string? WishPictureName { get; set; }
        public string? WishPictureRelPath { get; set; }
        public bool IsOpenDetailThumb { get; set; }
        public int? LeaveLimitCount { get; set; }
        public bool IsShowLeaveHeadImg { get; set; }
        public bool IsShowGossip { get;  set; }
    }
}
