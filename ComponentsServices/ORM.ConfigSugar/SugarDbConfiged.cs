using CommonObject.Enums;
using SqlSugar;
using System.Text.Json;

namespace ComponentsServices.ORM.ConfigSugar
{
    public class SugarDbConfiged
    {
        private DataBaseTypes DataBaseType;
        public SqlSugarScope DbHandler { get; set; }
        public SugarDbConfiged(DataBaseTypes dataBaseType = DataBaseTypes.MySql)
        {
            this.DataBaseType = dataBaseType;
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
            switch (this.DataBaseType)
            {
                case DataBaseTypes.MySql:
                    return DbType.MySql;
                case DataBaseTypes.SqlServer:
                    return DbType.SqlServer;
                default:
                    throw new ArgumentNullException("DataBaseTypes not matched.");
            }
        }
        private string FindConnStr()
        {
            var constr = "";
            var jsonStr = File.ReadAllText($"configs.json");
            var appSettings = JsonDocument.Parse(jsonStr, new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip });
            var needResult = appSettings.RootElement.GetProperty("DbConnStr");
            switch (this.DataBaseType)
            {
                case DataBaseTypes.MySql:
                    constr = needResult.GetProperty("Default").GetString();
                    break;
                case DataBaseTypes.SqlServer:
                    constr = needResult.GetProperty("SqlServer").GetString();
                    break;
            }
            return constr==""?throw new ArgumentException("connection string is empty."):constr?? 
                throw new ArgumentNullException("connection string is null.");
        }
    }
}
