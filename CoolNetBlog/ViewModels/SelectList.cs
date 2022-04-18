namespace CoolNetBlog.ViewModels
{
    /// <summary>
    /// 下拉框视图模型
    /// </summary>
    public class SelectList
    {
        public int Value { get; set; }
        public string Text { get; set; }
        public dynamic CarryData { get; set; }
        public List<SelectList> Subs { get; set; } = new List<SelectList>();

    }
}
