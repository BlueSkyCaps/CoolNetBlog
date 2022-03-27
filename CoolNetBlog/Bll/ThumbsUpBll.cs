using CommonObject.Constructs;
using CommonObject.Enums;
using ComponentsServices.Base;
using CoolNetBlog.Models;

namespace CoolNetBlog.Bll
{
    public class ThumbsUpBll
    {
        private readonly BaseSugar _baseSugar;
        private readonly SugarDataBaseStorage<Article, int> _articleSet;
        private readonly SugarDataBaseStorage<ArticleThumbUp, int> _thumbUpSet;
        public ThumbsUpBll()
        {
            _baseSugar = new BaseSugar();
            _articleSet = new SugarDataBaseStorage<Article,int>(_baseSugar._dbHandler);
            _thumbUpSet = new SugarDataBaseStorage<ArticleThumbUp, int>(_baseSugar._dbHandler);
        }

        public async Task<ValueResult> DealThumbsUpArticleAsync(int articleId, int type, HttpContext httpContext) 
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
                result.HideMessage = "文章点赞，获取不到客户端ip";
                result.TipMessage = "表态失败了呢，你的好意我心领啦。";
                return result;
            }
            var exd = await _thumbUpSet.AnyAsync(u => u.ArticleId == articleId && u.ClientIp == cip);
            if (exd)
            {
                result.TipMessage = "该文章你已经表过态啦！";
                return result;
            }
            try
            {
                _articleSet.TransBegin();
                await _thumbUpSet.InsertAsync(new ArticleThumbUp { ArticleId = articleId,
                    ClientIp = cip,Type=type, UpTime = DateTime.Now});
                _articleSet.TransCommit();
            }
            catch (Exception e)
            {
                _articleSet.TransRoll();
                result.Code = ValueCodes.Error;
                result.HideMessage = "点赞文章，执行插入数据报错:"+e.Message;
                result.TipMessage = "表态失败了呢，你的好意我心领啦。";
                return result;
            }
            result.Code = ValueCodes.Success;
            if (type==1)
            {
                result.TipMessage = "~谢谢你的点赞！";
            }else if (type == 2)
            {
                result.TipMessage = "~欢乐也是一时的享受。";
            }
            else
            {
                //type == 3 不敢苟同
                result.TipMessage = "~有容乃大,谢谢你的表态。";
            }
            var theAllArticleThumb = await _thumbUpSet.GetListByExpAsync(x => x.ArticleId == articleId);
            //文章表态类型数量，文章点赞数ThumbUpStart；文章"有被笑到"数ThumbUpFun；文章"不敢苟同"数ThumbUpSilence
            int thumbUpStart, thumbUpFun, thumbUpSilence = 0;
            thumbUpStart = theAllArticleThumb.Where(x => x.Type == 1).Count();
            thumbUpFun = theAllArticleThumb.Where(x => x.Type == 2).Count();
            thumbUpSilence = theAllArticleThumb.Where(x => x.Type == 3).Count();
            // 封装返回给aiax重显当前最新点赞数据
            result.Data = new { ThumbUpStart=thumbUpStart, ThumbUpFun=thumbUpFun, ThumbUpSilence=thumbUpSilence };
            return result;
        }
    }
}
