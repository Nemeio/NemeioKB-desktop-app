﻿using System;
using System.Windows.Input;

namespace Nemeio.Wpf.Models
{
    public class CommandHandler : ICommand
    {
        private Action _action;
        private Func<bool> _canExecute;

        public CommandHandler(Action action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute.Invoke();

        public void Execute(object parameter) => _action();
    }
}
