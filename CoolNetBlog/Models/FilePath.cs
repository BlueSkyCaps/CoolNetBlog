using SqlSugar;

namespace CoolNetBlog.Models
{
    [SugarTable("FilePath")]

    public class FilePath
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string FileRelPath { get; set; }
        public DateTime UploadTime { get; set; }
        public string? HelpName { get; set; }
        public string Type { get; set; }
    }
}
