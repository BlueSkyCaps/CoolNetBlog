using SqlSugar;

namespace CoolNetBlog.Models
{
    [SugarTable("Menu")]
    public class Menu
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Tips { get; set; }
        public int PId { get; set; }

        public bool IsHome { get; set; }
        public bool IsShow { get; set; }
        public int OrderNumber { get; set; }


    }
}