using System.Linq.Expressions;

namespace ComponentsServices.Base.Inter
{
    public interface IDateBaseStorage<TEntity, TId> where TEntity : class,new()
    {
        IList<TEntity> GetAllList();
        Task<IList<TEntity>> GetAllListAsync();
        Task<IList<TEntity>> GetTopCountListAsync(int tc, Expression<Func<TEntity, object>> orderCol, SqlSugar.OrderByType type = SqlSugar.OrderByType.Asc);
        IList<TEntity> GetListByExp(Expression<Func<TEntity, bool>> p);
        Task<IList<TEntity>> GetListByExpAsync(Expression<Func<TEntity, bool>> p);
        SqlSugar.ISugarQueryable<TEntity> GetListBuilder();
        TEntity FindOneById(object id);
        Task<TEntity> FindOneByIdAsync(object id);
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> p);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> p);
        int Insert(TEntity entity);
        Task<int> InsertAsync(TEntity entity);
        int Update(TEntity entity);
        Task<int> UpdateAsync(TEntity entity);
        Task<int> UpdateByExpAsync(TEntity entity,Expression<Func<TEntity, bool>> p);
        Task<int> UpdateBySomeColExpAsync(Expression<Func<TEntity, bool>> cols, Expression<Func<TEntity, bool>> p);
        Task<int> UpdateByIgColsAsync(TEntity entity, params string[] igCols);
        int Delete(TEntity entity);
        Task<int> DeleteAsync(TEntity entity);
        Task<int> DeleteEntitiesAsync(Expression<Func<TEntity, bool>> p);
        Task<long> CountAsync();
        Task<long> CountAsync(Expression<Func<TEntity, bool>> p);
        bool Any(Expression<Func<TEntity, bool>> p);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> p);

    }
}
