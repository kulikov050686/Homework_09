using System;
using System.Windows.Input;

namespace BaseClasse
{
    /// <summary>
    /// Класс комманд
    /// </summary>
    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        /// <summary>
        /// Событие извещающее об измении состояния команды
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Конструктор команды
        /// </summary>
        /// <param name="execute"> Выполняемый метод команды </param>
        /// <param name="canExecute"> Метод разрешающий выполнение команды </param>
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Вызов разрешающего метода команды
        /// </summary>
        /// <param name="parameter"> Параметр команды </param>        
        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        /// <summary>
        /// Вызов выполняющего метода команды
        /// </summary>
        /// <param name="parameter"> Параметр команды </param>
        public void Execute(object parameter)
        {
            if (execute != null)
            {
                this.execute(parameter);
            }
        }
    }
}
