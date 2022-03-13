using ComponentsServices.Base;
using CoolNetBlog.Models;

namespace CoolNetBlog.Bll
{
    public class CommentBll
    {
        private readonly SugarDataBaseStorage<Article, int> _articleSet;
        private readonly SugarDataBaseStorage<Comment, int> _commentSet;
        public CommentBll()
        {
            _articleSet = new SugarDataBaseStorage<Article, int>();
            _commentSet = new SugarDataBaseStorage<Comment, int>();
        }
    }
}
