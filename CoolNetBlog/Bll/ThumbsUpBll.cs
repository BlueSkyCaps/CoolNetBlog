using CommonObject.Classes;
using CommonObject.Enums;
using ComponentsServices.Base;
using CoolNetBlog.Models;

namespace CoolNetBlog.Bll
{
    public class ThumbsUpBll
    {
        private readonly SugarDataBaseStorage<Article, int> _articleSet;
        private readonly SugarDataBaseStorage<ArticleThumbUp, int> _thumbUpSet;
        public ThumbsUpBll()
        {
            _articleSet = new SugarDataBaseStorage<Article,int>();
            _thumbUpSet = new SugarDataBaseStorage<ArticleThumbUp, int>();
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
            var cip = httpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrWhiteSpace(cip))
            {
                result.HideMessage = "获取不到客户端ip";
                result.TipMessage = "点赞失败了呢，你的好意我心领啦。";
                return result;
            }
            var exd = await _thumbUpSet.AnyAsync(u => u.ArticleId == articleId && u.ClientIp == cip);
            if (exd)
            {
                result.TipMessage = "你已经点过赞啦！";
                return result;
            }
            try
            {
                _articleSet.TransBegin();
                articleAble.ThumbUpStart = articleAble.ThumbUpStart+1;
                await _articleSet.UpdateAsync(articleAble);
                await _thumbUpSet.InsertAsync(new ArticleThumbUp { ArticleId = articleId,ClientIp = cip});
                _articleSet.TransCommit();
            }
            catch (Exception e)
            {
                _articleSet.TransRoll();
                result.Code = ValueCodes.Error;
                result.HideMessage = "点赞文章，执行插入数据报错:"+e.Message;
                result.TipMessage = "点赞失败了呢，你的好意我心领啦。";
                return result;
            }
            result.Code = ValueCodes.Success;
            result.TipMessage = "~谢谢你的赞！";
            result.Data = articleAble.ThumbUpStart;
            return result;
        }
    }
}
