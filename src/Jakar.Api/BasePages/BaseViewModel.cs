using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Jakar.Api.Interfaces;
using Xamarin.Essentials;
using Xamarin.Forms;
using Share = Jakar.Api.Statics.Share;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.BasePages
{
	public abstract class BaseViewModel : INotifyPropertyChanged
	{
		private bool _isBusy;

		public bool IsBusy
		{
			get => _isBusy;
			set => SetProperty(ref _isBusy, value);
		}


		private string? _title = string.Empty;

		public string? Title
		{
			get => _title;
			set => SetProperty(ref _title, value);
		}


		public event PropertyChangedEventHandler? PropertyChanged;

		protected void SetProperty<T>( ref T backingStore, T value, [CallerMemberName] string propertyName = "" )
		{
			if ( EqualityComparer<T>.Default.Equals(backingStore, value) ) { return; }

			backingStore = value;
			OnPropertyChanged(propertyName);
		}

		protected void SetProperty<T>( ref T backingStore, T value, Action onChanged, [CallerMemberName] string propertyName = "" )
		{
			if ( EqualityComparer<T>.Default.Equals(backingStore, value) ) { return; }

			backingStore = value;
			onChanged();
			OnPropertyChanged(propertyName);
		}

		protected void SetProperty<T>( ref T backingStore, T value, Action<T> onChanged, [CallerMemberName] string propertyName = "" )
		{
			if ( EqualityComparer<T>.Default.Equals(backingStore, value) ) { return; }

			backingStore = value;
			onChanged(value);
			OnPropertyChanged(propertyName);
		}

		protected void OnPropertyChanged( string propertyName )
		{
			PropertyChangedEventHandler? changed = PropertyChanged;

			changed?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}



	public abstract class BaseViewModel<TPage> : BaseViewModel where TPage : Page
	{
		public ICommand? GoHomeCommand { get; protected set; }

		public ICommand LaunchWebsiteCommand { get; protected set; }

		public ICommand? RefreshCommand { get; protected set; }

		public ICommand? AboutCommand { get; protected set; }


		protected abstract IAppSettings _AppSettings { get; }
		protected TPage? _SourcePage { get; set; }


		protected BaseViewModel()
		{
			LaunchWebsiteCommand = new Command<string>(async ( address ) => await LaunchWebsite(address).ConfigureAwait(true));

			// GoHomeCommand = new Command(async () => await _SourcePage.Navigation.PopToRootAsync(true).ConfigureAwait(true));
		}

		protected BaseViewModel( TPage source ) : this() { SetPage(source); }


		public void SetPage( TPage source ) { _SourcePage = source ?? throw new ArgumentNullException(nameof(source)); }

		protected abstract Task StartFeedBack();
		protected abstract Task LaunchWebsite( object? address = null );


		protected abstract Task ShareScreenShot();

		protected virtual async Task ShareScreenShot( string shareTitle )
		{
			_AppSettings.ScreenShotAddress = await Share.GetScreenShot().ConfigureAwait(true);
			await Share.ShareFile(_AppSettings.ScreenShotAddress, shareTitle, System.Net.Mime.MediaTypeNames.Image.Jpeg).ConfigureAwait(false);
		}
	}



	public abstract class BaseViewModel<TPage, TItem> : BaseViewModel<TPage> where TPage : Page
																			 where TItem : class
	{
		public ICommand LoadItemsCommand { get; protected set; }

		public ObservableCollection<TItem> Items { get; set; } = new();

		private TItem? _selectedItem;

		public TItem? SelectedItem
		{
			get => _selectedItem;
			set
			{
				_selectedItem = value;
				SetProperty(ref _selectedItem, value);
			}
		}

		protected BaseViewModel() { LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand().ConfigureAwait(true)); }
		protected BaseViewModel( TPage source ) : base(source) { LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand().ConfigureAwait(true)); }


		public void LoadItems() => LoadItemsCommand.Execute(null);
		public async Task LoadItemsAsync() => await MainThread.InvokeOnMainThreadAsync(ExecuteLoadItemsCommand).ConfigureAwait(true);
		protected abstract Task ExecuteLoadItemsCommand();
	}



	public abstract class BaseViewModel<TPage, TItem, TSource> : BaseViewModel<TPage, TItem> where TPage : Page
																							 where TItem : class
																							 where TSource : class
	{
		private TSource? _sourceItem;

		public TSource? SourceItem
		{
			get => _sourceItem;
			set
			{
				_sourceItem = value;
				SetProperty(ref _sourceItem, value);
			}
		}

		protected BaseViewModel() { }
		protected BaseViewModel( TPage source ) : base(source) { }
	}
}
