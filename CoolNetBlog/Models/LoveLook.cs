using SqlSugar;

namespace CoolNetBlog.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("LoveLook")]
    public partial class LoveLook
    {
           public LoveLook(){


           }
         
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public int Id {get;set;}

          
           public DateTime AddedTime { get;set;}

         
           public string RelHref { get;set;}
           public string LinkName { get;set;}
           public int Type { get;set;}



    }
}
