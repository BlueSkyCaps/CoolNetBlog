using CommonObject.Enums;
using SqlSugar;
using System.Text.Json;

namespace ComponentsServices.ORM.ConfigSugar
{
    public class SugarDbConfiged
    {
        /// <summary>
        /// 静态数据库连接字符串变量 注意：项目只使用单数据库模式。
        /// 多数据库切换，DbConnStr和DataBaseType不能是static，因为多个客户端连接都会使用最先定义的全局静态，导致数据连接错误
        /// </summary>
        private static string? DbConnStr;
        /// <summary>
        /// 静态数据库连接类型 注意：项目只使用单数据库模式。
        /// 多数据库切换，DbConnStr和DataBaseType不能是static，因为多个客户端连接都会使用最先定义的全局静态，导致数据连接错误
        /// </summary>
        private static DataBaseTypes DataBaseType;
        public SqlSugarScope DbHandler { get; set; }
        public SugarDbConfiged(DataBaseTypes dataBaseType = DataBaseTypes.MySql)
        {
            DataBaseType = dataBaseType;
            //创建数据库对象
            SqlSugarScope db = new SqlSugarScope(new ConnectionConfig()
            {
                ConnectionString = FindConnStr(),
                DbType = FindType(),
                IsAutoCloseConnection = true
            });

            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                //Console.WriteLine(sql);
            };
            this.DbHandler = db;
            //db.DbFirst.IsCreateAttribute().CreateClassFile(@"C:\Users\Public\M", "Models");
        }

        private DbType FindType()
        {
            switch (DataBaseType)
            {
                case DataBaseTypes.MySql:
                    return DbType.MySql;
                case DataBaseTypes.SqlServer:
                    return DbType.SqlServer;
                default:
                    throw new ArgumentNullException("DataBaseTypes not matched.");
            }
        }
        private static string FindConnStr()
        {
            // DbConnStr已在最初的连接时被设置 后续任何操作、任何客户端连接都不必再读
            if (string.IsNullOrWhiteSpace(DbConnStr))
            {
                var jsonStr = File.ReadAllText($"configs.json");
                var appSettings = JsonDocument.Parse(jsonStr, new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip });
                var needResult = appSettings.RootElement.GetProperty("DbConnStr");
                bool isFinded;
                switch (DataBaseType)
                {
                    case DataBaseTypes.MySql:
                        isFinded = needResult.TryGetProperty("Default", out _);
                        if (isFinded)
                        {
                            DbConnStr = needResult.GetProperty("Default").ToString();
                        }
                        break;
                    case DataBaseTypes.SqlServer:
                        isFinded = needResult.TryGetProperty("SqlServer", out _);
                        if (isFinded)
                        {
                            DbConnStr = needResult.GetProperty("Default").ToString();
                        }
                        break;
                }
            }
            return DbConnStr == ""?throw new ArgumentException("connection string is empty."): DbConnStr ?? 
                throw new ArgumentNullException("connection string is null.");
        }
    }
}
