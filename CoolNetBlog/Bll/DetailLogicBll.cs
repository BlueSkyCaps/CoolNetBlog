using ComponentsServices.Base;
using CoolNetBlog.Models;
using CoolNetBlog.ViewModels.Api;
using CoolNetBlog.ViewModels.Detail;

namespace CoolNetBlog.Bll
{
    /// <summary>
    /// 文章详细 单独业务处理类
    /// </summary>
    public class DetailLogicBll
    {
        private SugarDataBaseStorage<DetailArticleViewModel, int> _articleVmSet;
        private SugarDataBaseStorage<Menu, int> _menuSet;

        public DetailLogicBll()
        {
            _articleVmSet = new SugarDataBaseStorage<DetailArticleViewModel, int>();
            _menuSet = new SugarDataBaseStorage<Menu, int>();
        }

        /// <summary>
        /// 获取当前的文章
        /// </summary>
        /// <param name="_homeGlobalView"></param>
        /// <param name="articleId"></param>
        /// <param name="custUri"></param>
        /// <returns></returns>
        public async Task DealArticleEntityAsync(ViewModels.Home.HomeViewModel _homeGlobalView, int? articleId, string? custUri)
        {
            _homeGlobalView.DetailArticleData = null;
            // 要么传递文章Id，要么传递自定义文章uri，两个都是唯一的
            if (!String.IsNullOrWhiteSpace(custUri))
            {
                _homeGlobalView.DetailArticleData = await _articleVmSet.FirstOrDefaultAsync(a => a.CustUri == custUri);
            }
            else if (articleId!=null)
            {
                _homeGlobalView.DetailArticleData = await _articleVmSet.FindOneByIdAsync(articleId);
            }

              
            if (_homeGlobalView.DetailArticleData is null)
            {
                _homeGlobalView.NotTips = "找不到文章！返回重试一下，或者关键字重新搜搜吧？！";
            }
            else
            {
                _homeGlobalView.DetailArticleData.Ig_MenuName = (await _menuSet.FindOneByIdAsync(_homeGlobalView.DetailArticleData.MenuId)).Name;
                _homeGlobalView.Location = "Detail";
                _homeGlobalView.LocationTip = "文章(帖子)";
                // 将标签组成字符串列表
                if (!string.IsNullOrWhiteSpace(_homeGlobalView.DetailArticleData.Labels))
                {
                    var tmpLabelV = _homeGlobalView.DetailArticleData.Labels.Split(',', '，', ' ').ToList();
                    tmpLabelV.RemoveAll(a => string.IsNullOrWhiteSpace(a));
                    _homeGlobalView.DetailArticleData.LabelsList = tmpLabelV;
                }
                // 是隐私文章 隐藏内容主体
                _homeGlobalView.DetailArticleData.Content = _homeGlobalView.DetailArticleData.IsLock ? 
                    "" : _homeGlobalView.DetailArticleData.Content;
            }
        }

        /// <summary>
        /// 隐私文章解锁
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ArticleUnLockViewModel DealArticleUnLock(ArticleUnLockViewModel data)
        {
            try
            {
                data.Code = 0;
                data.Content = "";
                SugarDataBaseStorage<Article, int> articleSet = new SugarDataBaseStorage<Article, int>();
                var article = articleSet.FindOneById(data.ArticleId);
                if (article is null)
                {
                    return data;
                }
                // 若已经不是隐私文章或密码正确
                if ((!article.IsLock) || data.Password == article.LockPassword)
                {
                    data.Content = article.Content;
                    data.Code = 1;
                    return data;
                }

                return data;

            }
            catch (Exception)
            {
                return data;
            }

        }
    }
}
