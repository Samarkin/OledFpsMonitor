using System;
using System.Windows.Input;

namespace Samarkin.Oled.FpsMonitor
{
	public class RelayCommand : ICommand
	{
		private readonly Action<object?> _execute;
		private readonly Func<object?, bool> _canExecute;

		public event EventHandler? CanExecuteChanged;

		public RelayCommand(Action execute, Func<bool>? canExecute = null)
		{
			_execute = _ => execute();
			_canExecute = canExecute != null
				? _ => canExecute()
				: _ => true;
		}

		public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
		{
			_execute = execute;
			_canExecute = canExecute ?? (_ => true);
		}

		public void NotifyCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}

		public bool CanExecute(object? parameter)
		{
			return _canExecute.Invoke(parameter);
		}

		public void Execute(object? parameter)
		{
			_execute.Invoke(parameter);
		}
	}
}
