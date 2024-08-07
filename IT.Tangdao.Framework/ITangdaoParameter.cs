using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework
{
    public interface ITangdaoParameter
    {
        void Add<T>(string key, T value);
        T Get<T>(string key);
        void AddCommand(string key, Action command);
        void ExecuteCommand(string key);
    }
}
