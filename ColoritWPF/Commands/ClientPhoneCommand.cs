using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ColoritWPF.ViewModel;

namespace ColoritWPF.Commands
{
    internal class ClientPhoneCommand : ICommand
    {
        public ClientPhoneCommand(ColorsMainViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private ColorsMainViewModel _viewModel;

        #region ICommand members

        public bool CanExecute(object parameter)
        {
            //return _viewModel.CanUpdate;
            throw new NotImplementedException();
        }

        public event System.EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            //_viewModel.SaveChanges();
        }

        #endregion
    }
}
