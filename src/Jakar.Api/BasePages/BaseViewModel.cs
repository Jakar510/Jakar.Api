using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Jakar.Api.Statics;
using Jakar.Extensions.Http;
using Jakar.Extensions.Interfaces;
using Jakar.Extensions.Models;
using Xamarin.Essentials;
using Xamarin.Forms;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.BasePages
{
	public abstract class BaseViewModel : BaseNotifyPropertyModel
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
	}



	public abstract class BaseViewModel<TPage> : BaseViewModel where TPage : Page
	{
		public ICommand? GoHomeCommand { get; protected set; }

		public ICommand LaunchWebsiteCommand { get; protected set; }

		public ICommand? RefreshCommand { get; protected set; }

		public ICommand? AboutCommand { get; protected set; }


		protected abstract IAppSettings _AppSettings { get; }
		protected          TPage?       _SourcePage  { get; set; }


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

		protected virtual async Task ShareScreenShot( FileSystemApi api, string shareTitle )
		{
			_AppSettings.ScreenShotAddress = await api.GetScreenShot().ConfigureAwait(true);
			await _AppSettings.ScreenShotAddress.ShareFile(shareTitle, MimeTypeNames.Image.JPEG).ConfigureAwait(false);
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

		protected BaseViewModel() { LoadItemsCommand                              = new Command(async () => await ExecuteLoadItemsCommand().ConfigureAwait(true)); }
		protected BaseViewModel( TPage source ) : base(source) { LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand().ConfigureAwait(true)); }


		public             void LoadItems()      => LoadItemsCommand.Execute(null);
		public async       Task LoadItemsAsync() => await MainThread.InvokeOnMainThreadAsync(ExecuteLoadItemsCommand).ConfigureAwait(true);
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
