using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace CoolNetBlog.Models
{
    ///<summary>
    /// "闲言碎语"组件表
    ///</summary>
    [SugarTable("Gossip")]
    public partial class Gossip
    {
        public Gossip(){


        }
        /// <summary>
        /// Desc:自增主键
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
        public int Id {get;set;}

        /// <summary>
        /// Desc:内容
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string Content {get;set;}

        public DateTime AddTime {get;set;}

        public int StarNumber { get; set; }

    }
}
