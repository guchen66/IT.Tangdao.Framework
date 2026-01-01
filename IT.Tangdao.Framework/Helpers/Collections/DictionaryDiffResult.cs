using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// 字典差异结果
    /// </summary>
    public class DictionaryDiffResult<TKey, TValue>
    {
        /// <summary>
        /// 新增的键
        /// </summary>
        public List<TKey> AddedKeys { get; set; } = new List<TKey>();

        /// <summary>
        /// 删除的键
        /// </summary>
        public List<TKey> RemovedKeys { get; set; } = new List<TKey>();

        /// <summary>
        /// 修改的键
        /// </summary>
        public List<TKey> ModifiedKeys { get; set; } = new List<TKey>();

        /// <summary>
        /// 未修改的键
        /// </summary>
        public List<TKey> UnchangedKeys { get; set; } = new List<TKey>();

        /// <summary>
        /// 是否有差异
        /// </summary>
        public bool HasChanges => AddedKeys.Count > 0 || RemovedKeys.Count > 0 || ModifiedKeys.Count > 0;
    }
}