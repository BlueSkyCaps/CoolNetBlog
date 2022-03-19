using CommonObject.Enums;
using ComponentsServices.Base.Inter;
using ComponentsServices.ORM.ConfigSugar;
using SqlSugar;
using System.Linq.Expressions;

namespace ComponentsServices.Base
{
    public class BaseSugar
    {
        public readonly SqlSugarScope _dbHandler;
        public BaseSugar(DataBaseTypes dataBaseType = DataBaseTypes.MySql)
        {
            _dbHandler = new SugarDbConfiged(dataBaseType).DbHandler;
        }
    }
    public class SugarDataBaseStorage<TEntity, TKey> : IDateBaseStorage<TEntity, TKey> where TEntity : class, new()
    {
        public readonly SqlSugar.SqlSugarScope _dbHandler;
        /// <summary>
        /// 一个实体类实例化一个数据库句柄 </br>
        /// 后续每个实体Set都要连接一次数据库 开销大
        /// </summary>
        /// <param name="dataBaseType"></param>
        public SugarDataBaseStorage(DataBaseTypes dataBaseType = DataBaseTypes.MySql)
        {
            _dbHandler = new SugarDbConfiged(dataBaseType).DbHandler;
        }

        /// <summary>
        /// 只用事先实例化的单独数据库句柄 后续每个泛型实体Set会共享这一个句柄 不会重复连接数据库 </br>
        /// 建议搭配BaseSugar类使用此构造函数初始化
        /// </summary>
        /// <param name="dbHandler"></param>
        public SugarDataBaseStorage(SqlSugarScope dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public Task<long> CountAsync()
        {
            throw new NotImplementedException();
        }

        public Task<long> CountAsync(Expression<Func<TEntity, bool>> p)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 判断实体表是否存在至少一条数据，若p不是null则按条件判断
        /// </summary>
        /// <returns></returns>
        public bool Any(Expression<Func<TEntity, bool>> p)
        {
            if (p is null)
            {
                return  _dbHandler.Queryable<TEntity>().Any();
            }
            return _dbHandler.Queryable<TEntity>().Any(p);

        }

        /// <summary>
        /// 异步 判断实体表是否存在至少一条数据，若p不是null则按条件判断
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> p)
        {
            if (p is null)
            {
                return await _dbHandler.Queryable<TEntity>().AnyAsync();
            }
            return await _dbHandler.Queryable<TEntity>().AnyAsync(p);

        }

        public int Delete(TEntity entity) 
        {
            return _dbHandler.Deleteable(entity).ExecuteCommand();
        }

        public async Task<int> DeleteAsync(TEntity entity)
        {
            return await _dbHandler.Deleteable(entity).ExecuteCommandAsync();

        }

        public async Task<int> DeleteEntitiesAsync(Expression<Func<TEntity, bool>> p)
        {
            return await _dbHandler.Deleteable(p).ExecuteCommandAsync();
        }

        /// <summary>
        /// 通过唯一主键值查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TEntity FindOneById(object id)
        {
            return _dbHandler.Queryable<TEntity>().InSingle(id);

        }

        /// <summary>
        /// 异步 通过唯一主键值查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TEntity> FindOneByIdAsync(object id)
        {
            return await _dbHandler.Queryable<TEntity>().InSingleAsync(id);
        }

        /// <summary>
        /// 按条件查找第一个,没有返回null
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> p)
        {
            return _dbHandler.Queryable<TEntity>().First(p);
        }

        /// <summary>
        /// 异步 按条件查找第一个,没有返回null
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> p)
        {
            return await _dbHandler.Queryable<TEntity>().FirstAsync(p);
        }

        public IList<TEntity> GetAllList()
        {
            return _dbHandler.Queryable<TEntity>().ToList();
        }

        public async Task<IList<TEntity>> GetAllListAsync()
        {
            return await _dbHandler.Queryable<TEntity>().ToListAsync();

        }

        /// <summary>
        /// 获取指定数量的头部数据，按某字段排序
        /// </summary>
        /// <param name="tc"></param>
        /// <param name="orderCol"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<IList<TEntity>> GetTopCountListAsync(int tc, Expression<Func<TEntity, object>> orderCol, OrderByType type = OrderByType.Asc)
        {
            return await _dbHandler.Queryable<TEntity>().OrderBy(orderCol, type).Take(tc).ToListAsync();

        }

        /// <summary>
        /// 按条件返回列表数据
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public IList<TEntity> GetListByExp(Expression<Func<TEntity, bool>> p)
        {
            return _dbHandler.Queryable<TEntity>().Where(p).ToList();

        }

        /// <summary>
        /// 异步 按条件返回列表数据
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public async Task<IList<TEntity>> GetListByExpAsync(Expression<Func<TEntity, bool>> p)
        {
            return await _dbHandler.Queryable<TEntity>().Where(p).ToListAsync();

        }

        /// <summary>
        /// ISugarQueryable查询断句 返回Queryable后的ISugarQueryable句柄以此来继续之后的操作
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public ISugarQueryable<TEntity> GetListBuilder()
        {
            return _dbHandler.Queryable<TEntity>();

        }

        public int Insert(TEntity entity)
        {
            return _dbHandler.Insertable(entity).ExecuteCommand();
        }

        public async Task<int> InsertAsync(TEntity entity)
        {
            return await _dbHandler.Insertable(entity).ExecuteCommandAsync();

        }

        public int Update(TEntity entity)
        {
            return _dbHandler.Updateable(entity).ExecuteCommand();

        }

        public async Task<int> UpdateAsync(TEntity entity)
        {
            return await _dbHandler.Updateable(entity).ExecuteCommandAsync();

        }

        public async Task<int> UpdateByIgColsAsync(TEntity entity, params string[] igCols)
        {
            return await _dbHandler.Updateable(entity).IgnoreColumns(igCols).ExecuteCommandAsync();

        }

        /// <summary>
        /// 异步 按条件更新匹配的数据，改为entity实体的值
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public async Task<int> UpdateByExpAsync(TEntity entity, Expression<Func<TEntity, bool>> p)
        {
            return await _dbHandler.Updateable(entity).Where(p).ExecuteCommandAsync();
        }

        /// <summary>
        /// 异步 按条件更新实体，只更新想要更新的列
        /// </summary>
        /// <param name="cols">要更新的列</param>
        /// <param name="p">where表达式</param>
        /// <returns></returns>
        public async Task<int> UpdateBySomeColExpAsync(Expression<Func<TEntity, bool>> cols, Expression<Func<TEntity, bool>> p)
        {

            return await _dbHandler.Updateable<TEntity>()
            .SetColumns(cols)
            .Where(p)
            .ExecuteCommandAsync();
        }

        public void TransBegin()
        {
            _dbHandler.Ado.BeginTran();
        }

        public void TransCommit()
        {
            _dbHandler.Ado.CommitTran();
        }
        public void TransRoll()
        {
            _dbHandler.Ado.RollbackTran();
        }
    }
}
