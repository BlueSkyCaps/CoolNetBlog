using SqlSugar;

namespace CoolNetBlog.Models
{
    [SugarTable("AdminUser")]

    public class AdminUser
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }
        /// <summary>
        /// Token=PassToken (from page passd)
        /// </summary>
        public string? Email { get; set; }
        public string Token { get; set; }
        public string? SmtpHost { get; set; }
        public string? EmailPassword { get; set; }
        public int? Port { get; set; }
        public bool? UseSsl { get; set; }


    }
}
