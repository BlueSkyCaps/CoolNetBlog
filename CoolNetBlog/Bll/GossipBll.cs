using CommonObject.Constructs;
using CommonObject.Enums;
using ComponentsServices.Base;
using CoolNetBlog.Models;

namespace CoolNetBlog.Bll
{
    public class GossipBll
    {
        private readonly BaseSugar _baseSugar;
        private readonly SugarDataBaseStorage<Gossip, int> _gossipSet;
        private ValueResult _result;
        
        public GossipBll()
        {
            _result = new()
            {
                Code = ValueCodes.UnKnow
            };
            _baseSugar = new BaseSugar();
            _gossipSet = new SugarDataBaseStorage<Gossip, int>(_baseSugar._dbHandler);

        }

        public async Task<ValueResult> GetGossipsAsync(int index, int pageCount)
        {

            try
            {
                // 最新记录在前 一次加载取pageCount
                var gossips = await _gossipSet.GetListBuilder()
                    .OrderBy(g=>g.AddTime, SqlSugar.OrderByType.Desc)
                    .Skip((index - 1)* pageCount).Take(pageCount).ToListAsync();
                _result.Data = gossips;
                _result.Code = ValueCodes.Success;
            }
            catch (Exception e)
            {
                _result.Code = ValueCodes.Error;
                _result.HideMessage = $"获取“闲言碎语”Gossip表数据失败，引发异常:{e.Message} {e.StackTrace}";
                _result.TipMessage = "加载失败了，下滑在试试吧?!";
            }
            return _result;
        }

    }
}
