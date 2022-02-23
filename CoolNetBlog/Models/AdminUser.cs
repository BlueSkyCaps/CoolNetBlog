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
        public string Token { get; set; }


    }
}
