﻿using ComponentsServices.Base;
using CoolNetBlog.BlogException;
using CoolNetBlog.Models;
using CoolNetBlog.ViewModels.Detail;

namespace CoolNetBlog.Bll
{
    /// <summary>
    /// 文章详细 单独业务处理类
    /// </summary>
    public class DetailLogicBll
    {
        private BaseSugar _baseSugar;
        private SugarDataBaseStorage<DetailArticleViewModel, int> _articleVmSet;
        private SugarDataBaseStorage<Menu, int> _menuSet;
        private SugarDataBaseStorage<ArticleThumbUp, int> _thumbUpSet;

        public DetailLogicBll()
        {
            _baseSugar = new BaseSugar();
            _articleVmSet = new SugarDataBaseStorage<DetailArticleViewModel, int>(_baseSugar._dbHandler);
            _menuSet = new SugarDataBaseStorage<Menu, int>(_baseSugar._dbHandler);
            _thumbUpSet = new SugarDataBaseStorage<ArticleThumbUp, int>(_baseSugar._dbHandler);
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

            // 没有此文章 或文章当前是草稿
            if (_homeGlobalView.DetailArticleData is null || _homeGlobalView.DetailArticleData.IsDraft)
            {
                // 找不到显示的文章 主动抛出异常捕获，前端显示提示文本
                _homeGlobalView.NotTips = "找不到文章(帖子)！返回重试一下，或者关键字重新搜搜吧？！";
                throw new DetailNotExistException();
            }
            if(!_homeGlobalView.DetailArticleData.IsSpecial)
            {
                _homeGlobalView.DetailArticleData.Ig_MenuName = (await _menuSet.FindOneByIdAsync(_homeGlobalView.DetailArticleData.MenuId)).Name;
            }else
            {
                _homeGlobalView.DetailArticleData.Ig_MenuName = "特殊内容";
            }
            _homeGlobalView.Location = "Detail";
            _homeGlobalView.LocationTip = "文章(帖子)";
            // 将标签组成字符串列表
            if (!string.IsNullOrWhiteSpace(_homeGlobalView.DetailArticleData.Labels))
            {
                var tmpLabelV = _homeGlobalView.DetailArticleData.Labels.Split(',', '，', ' ').ToList();
                tmpLabelV.RemoveAll(a => string.IsNullOrWhiteSpace(a));
                _homeGlobalView.DetailArticleData.LabelsList = tmpLabelV;
            }
            // 是加锁文章 隐藏内容主体
            _homeGlobalView.DetailArticleData.Content = _homeGlobalView.DetailArticleData.IsLock ? 
                "" : _homeGlobalView.DetailArticleData.Content;
            // 处理文章点赞数
            // 获取文章Id，因为有可能是用文章自定义uri访问文章，此时方法参数articleId是null，而需要用id去文章点赞表找当前文章的点赞数
            articleId = _homeGlobalView.DetailArticleData.Id;
            var theAllArticleThumb = await _thumbUpSet.GetListByExpAsync(x => x.ArticleId == articleId);
            // 文章表态类型数量，文章点赞数ThumbUpStart；文章"有被笑到"数ThumbUpFun；文章"不敢苟同"数ThumbUpSilence
            int thumbUpStart,thumbUpFun, thumbUpSilence = 0;
            thumbUpStart = theAllArticleThumb.Where(x => x.Type == 1).Count();
            thumbUpFun = theAllArticleThumb.Where(x => x.Type == 2).Count();
            thumbUpSilence = theAllArticleThumb.Where(x => x.Type == 3).Count();
            var thumbData = new Dictionary<string, int> { 
                { "ThumbUpStart", thumbUpStart }, 
                { "ThumbUpFun", thumbUpFun }, 
                { "ThumbUpSilence", thumbUpSilence } 
            };
            _homeGlobalView.DetailArticleData.ThumbUpNumbers = thumbData;
            _homeGlobalView.CurrentTitle = _homeGlobalView.DetailArticleData.IsShowTitle&& 
                !string.IsNullOrWhiteSpace(_homeGlobalView.DetailArticleData.Title) ? 
                _homeGlobalView.DetailArticleData.Title: "正阅读无题文章"+_homeGlobalView.DetailArticleData.Id;
        }



        /// <summary>
        /// 加锁文章解锁
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public DetailArticleUnLockViewModel DealArticleUnLock(DetailArticleUnLockViewModel data)
        {
            try
            {
                data.Code = 0;
                data.Content = "";
                //SugarDataBaseStorage<Article, int> articleSet = new SugarDataBaseStorage<Article, int>();
                var article = _articleVmSet.FindOneById(data.ArticleId);
                if (article is null)
                {
                    return data;
                }
                // 若已经不是加锁文章或密码正确
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
