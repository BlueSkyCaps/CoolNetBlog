using CommonObject.Classes;
using CommonObject.Enums;
using ComponentsServices.Base;
using CoolNetBlog.Models;

namespace CoolNetBlog.Bll
{
    public class ThumbsUpBll
    {
        private readonly SugarDataBaseStorage<Article, int> _articleSet;
        public ThumbsUpBll()
        {
            _articleSet = new SugarDataBaseStorage<Article,int>();
        }

        public async Task<ValueResult> DealThumbsUpArticleAsync(int articleId, HttpContext httpContext) 
        {
            ValueResult result = new ValueResult();
            result.Code = ValueCodes.UnKnow;
            if (articleId<=0)
            {
                result.HideMessage = "文章Id小于等于0";
                result.TipMessage = "该文章或许已不存在了，请返回首页或者再试一次吧?!";
                return result;
            }
            var articleAble = await _articleSet.FindOneByIdAsync(articleId);
            if (articleAble is null)
            {
                result.HideMessage = "没有此文章Id";
                result.TipMessage = "该文章或许已不存在了，请返回首页或者再试一次吧?!";
                return result;
            }
            var cip = httpContext.Request.HttpContext.Connection.RemoteIpAddress;
            if (cip is null )
            {
                result.HideMessage = "获取不到客户端ip";
                result.TipMessage = "点赞失败了呢，你的好意我心领啦";
                return result;
            }
            result.Code = ValueCodes.Success;
        }
    }
}
