using IT.Tangdao.Framework.DaoAdmin.IServices;
using IT.Tangdao.Framework.DaoCommands;
using IT.Tangdao.Framework.DaoCommon;
using IT.Tangdao.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IT.Tangdao.Framework.DaoMvvm
{
    public class DaoViewModelBase<T> : DaoViewModelBase
    {
        private T _model;

        public T Model
        {
            get => _model;
            set => SetProperty(ref _model, value);
        }

        private IWriteService _writeService;

        public DaoViewModelBase(T model)
        {
            Model = model;
            SaveCommand = MinidaoCommand.Create(ExecuteSave);
            DeleteCommand = MinidaoCommand.Create(ExecuteDelete);
        }

        private void ExecuteSave()
        {
            var info = XmlFolderHelper.SerializeXML<T>(Model);
            _writeService.WriteString(DaoLocation.RecipePath, info);
        }

        private void ExecuteDelete()
        {
        }

        public ICommand SaveCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
    }
}