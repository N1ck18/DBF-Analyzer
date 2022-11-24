using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DBF_Analyzer_WPF.Infrastructure.Commands.Base
{
    // ВАЩЕ ХЗ ЧТО И КАК
    internal abstract class Command : ICommand // Важен интерфейс!
    {
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public abstract bool CanExecute(object? parameter);

        public abstract void Execute(object? parameter);
        
    }
}
