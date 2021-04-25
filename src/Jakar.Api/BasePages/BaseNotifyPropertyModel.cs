using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Jakar.Extensions.Annotations;
using Xamarin.Essentials;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.BasePages
{
	public class BaseNotifyPropertyModel : INotifyPropertyChanged, INotifyPropertyChanging
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		public event PropertyChangingEventHandler? PropertyChanging;


		/// <summary>
		/// Calls <see cref="OnPropertyChanging"/>, sets the value, then calls <see cref="OnPropertyChanged"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="backingStore"></param>
		/// <param name="value"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		[NotifyPropertyChangedInvocator]
		protected bool SetProperty<T>( ref T backingStore, T value, [CallerMemberName] string propertyName = "" )
		{
			if ( EqualityComparer<T>.Default.Equals(backingStore, value) ) { return false; }

			OnPropertyChanging(propertyName);
			backingStore = value;
			OnPropertyChanged(propertyName);

			return true;
		}


		/// <summary>
		/// "onChanged" only called if the backingStore value has changed, and onChanged.CanExecute(value) is true. 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="backingStore"></param>
		/// <param name="value"></param>
		/// <param name="onChanged"></param>
		/// <param name="propertyName"></param>
		[NotifyPropertyChangedInvocator]
		protected void SetProperty<T>( ref T backingStore, T value, ICommand onChanged, [CallerMemberName] string propertyName = "" )
		{
			if ( !SetProperty(ref backingStore, value, propertyName) ) { return; }

			if ( onChanged.CanExecute(value) ) { onChanged.Execute(value); }
		}


		/// <summary>
		/// "onChanged" only called if the backingStore value has changed.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="backingStore"></param>
		/// <param name="value"></param>
		/// <param name="onChanged"></param>
		/// <param name="propertyName"></param>
		[NotifyPropertyChangedInvocator]
		protected void SetProperty<T>( ref T backingStore, T value, Action onChanged, [CallerMemberName] string propertyName = "" )
		{
			if ( !SetProperty(ref backingStore, value, propertyName) ) { return; }

			onChanged();
		}


		/// <summary>
		/// "onChanged" only called if the backingStore value has changed.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="backingStore"></param>
		/// <param name="value"></param>
		/// <param name="onChanged"></param>
		/// <param name="propertyName"></param>
		[NotifyPropertyChangedInvocator]
		protected void SetProperty<T>( ref T backingStore, T value, Action<T> onChanged, [CallerMemberName] string propertyName = "" )
		{
			if ( !SetProperty(ref backingStore, value, propertyName) ) { return; }

			onChanged(value);
		}


		/// <summary>
		/// "onChanged" only called if the backingStore value has changed.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="backingStore"></param>
		/// <param name="value"></param>
		/// <param name="onChanged"></param>
		/// <param name="background"></param>
		/// <param name="propertyName"></param>
		[NotifyPropertyChangedInvocator]
		protected void SetProperty<T>( ref T backingStore,
									   T value,
									   Func<Task> onChanged,
									   bool background = true,
									   [CallerMemberName] string propertyName = ""
		)
		{
			if ( !SetProperty(ref backingStore, value, propertyName) ) { return; }

			if ( background ) { Task.Run(onChanged); }
			else { MainThread.BeginInvokeOnMainThread(async () => await onChanged().ConfigureAwait(true)); }
		}


		/// <summary>
		/// "onChanged" only called if the backingStore value has changed.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="backingStore"></param>
		/// <param name="value"></param>
		/// <param name="onChanged"></param>
		/// <param name="background"></param>
		/// <param name="propertyName"></param>
		[NotifyPropertyChangedInvocator]
		protected void SetProperty<T>( ref T backingStore,
									   T value,
									   Func<T, Task> onChanged,
									   bool background = true,
									   [CallerMemberName] string propertyName = ""
		)
		{
			if ( !SetProperty(ref backingStore, value, propertyName) ) { return; }

			if ( background ) { Task.Run(async () => await onChanged(value).ConfigureAwait(true)); }
			else { MainThread.BeginInvokeOnMainThread(async () => await onChanged(value).ConfigureAwait(true)); }
		}


		[NotifyPropertyChangedInvocator]
		protected void OnPropertyChanged( [CallerMemberName] string propertyName = "" ) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }


		// [NotifyPropertyChangingInvocator]
		protected void OnPropertyChanging( [CallerMemberName] string propertyName = "" ) { PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName)); }
	}
}
