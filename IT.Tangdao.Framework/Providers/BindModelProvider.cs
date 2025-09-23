using IT.Tangdao.Framework.Abstractions.IServices;
using IT.Tangdao.Framework.DaoMvvm;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Providers
{
    public class BindModelProvider<T> : DaoViewModelBase, IBindModelProvider<T>
    {
        public T Default { get; set; }
        private readonly IReadService _readService;
        private string _objectPath;

        public string ObjectPath
        {
            get => _objectPath;
            set => _objectPath = value;
        }

        public BindModelProvider()
        {
            _readService = TangdaoApplication.Provider.GetService<IReadService>();
            var xmlData = _readService.Read(ObjectPath);
            Default = XmlFolderHelper.Deserialize<T>(xmlData);
        }
    }
}