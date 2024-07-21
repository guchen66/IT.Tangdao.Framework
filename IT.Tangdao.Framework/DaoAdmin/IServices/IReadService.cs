using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin.IServices
{
    /// <summary>
    /// 定义读取服务层
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IReadService<TEntity> where TEntity : class, new()
    {
        #region 增删改查

      //  Task<bool> AddAsync(TEntity entity);
     //   Task<bool> UpdateAsync(TEntity entity);
     //   Task<bool> DeleteAsync(int id);
     //   Task<TEntity> QueryAsync(int id);
     //   Task<TEntity> QueryAsync(Expression<Func<TEntity, bool>> func);
        #endregion

        /// <summary>
        /// 查询全部数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
       // Task<List<TEntity>> QueryListAsync();

        /// <summary>
        /// 自定义条件查询
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
       // Task<List<TEntity>> QueryListAsync(Expression<Func<TEntity, bool>> func);

    }
}
