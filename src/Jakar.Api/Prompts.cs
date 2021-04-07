using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Jakar.Api.Exceptions.General;
using Jakar.Api.Exceptions.Networking;
using Xamarin.Forms;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api
{
	public class Prompts
	{
		protected IUserDialogs _Prompts { get; } = UserDialogs.Instance;

		private ApiServices? _services;

		protected ApiServices _Services
		{
			get => _services ?? throw new ApiDisabledException($"Must call {nameof(Start)} first.", new NullReferenceException(nameof(_services)));
			private set => _services = value;
		}
		private Debug? _debug;

		protected Debug _Debug
		{
			get => _debug ?? throw new ApiDisabledException($"Must call {nameof(Start)} first.", new NullReferenceException(nameof(_services)));
			private set => _debug = value;
		}

		public void Start( ApiServices services ) => _Services = services;
		public void Start( Debug services ) => _Debug = services;


		internal void ShowLoading( string title ) => ShowLoading(title, MaskType.Black);
		internal void ShowLoading( string title, MaskType mask ) => _Prompts.ShowLoading(title, mask);
		internal void HideLoading() => _Prompts.HideLoading();


		public ICommand LoadingCommand( Func<CancellationToken, Task> func, Page page, string cancel ) => LoadingCommand(func, MaskType.Black, cancel, page);

		public ICommand LoadingCommand( Func<CancellationToken, Task> func, MaskType mask, string cancel, Page page ) => LoadingCommand(func,
																																		null,
																																		cancel,
																																		mask,
																																		page);

		public ICommand LoadingCommand( Func<CancellationToken, Task> func,
										string? title,
										string cancel,
										MaskType mask,
										Page page
		) => new Command(async () => await LoadingAsyncTask(func,
															title,
															cancel,
															mask,
															page).ConfigureAwait(true));

		public async Task LoadingAsyncTask( Func<CancellationToken, Task> func, Page page, string cancel ) => await LoadingAsyncTask(func, page, cancel, MaskType.Black).ConfigureAwait(true);

		public async Task LoadingAsyncTask( Func<CancellationToken, Task> func, Page page, string cancel, MaskType mask ) => await LoadingAsyncTask(func,
																																					null,
																																					cancel,
																																					mask,
																																					page).ConfigureAwait(true);

		public async Task LoadingAsyncTask( Func<CancellationToken, Task> func,
											string? title,
											string cancel,
											MaskType mask,
											Page page
		)
		{
			if ( func is null ) throw new ArgumentNullException(nameof(func));

			using var cancelSrc = new CancellationTokenSource();
			ProgressDialogConfig config = new ProgressDialogConfig().SetTitle(title).SetIsDeterministic(false).SetMaskType(mask).SetCancel(cancel, cancelSrc.Cancel);

			using ( _Prompts.Progress(config) )
			{
				try { await func(cancelSrc.Token).ConfigureAwait(true); }
				catch ( OperationCanceledException ) { }
				catch ( Exception e ) { await HandleExceptionAsync(e, page, cancelSrc.Token).ConfigureAwait(true); }
			}
		}


		public ICommand LoadingCommand( Func<Task> func, Page page, string cancel ) => LoadingCommand(func, MaskType.Black, cancel, page);

		public ICommand LoadingCommand( Func<Task> func, MaskType mask, string cancel, Page page ) => LoadingCommand(func,
																													 null,
																													 cancel,
																													 mask,
																													 page);

		public ICommand LoadingCommand( Func<Task> func,
										string? title,
										string cancel,
										MaskType mask,
										Page page
		) => new Command(async () => await LoadingAsyncTask(func,
															title,
															cancel,
															mask,
															page).ConfigureAwait(true));

		public async Task LoadingAsyncTask( Func<Task> func, Page page, string cancel ) => await LoadingAsyncTask(func, page, cancel, MaskType.Black).ConfigureAwait(true);

		public async Task LoadingAsyncTask( Func<Task> func, Page page, string cancel, MaskType mask ) => await LoadingAsyncTask(func,
																																 null,
																																 cancel,
																																 mask,
																																 page).ConfigureAwait(true);

		public async Task LoadingAsyncTask( Func<Task> func,
											string? title,
											string cancel,
											MaskType mask,
											Page page
		)
		{
			if ( func is null ) throw new ArgumentNullException(nameof(func));

			using var cancelSrc = new CancellationTokenSource();
			ProgressDialogConfig config = new ProgressDialogConfig().SetTitle(title).SetIsDeterministic(false).SetMaskType(mask).SetCancel(cancel, cancelSrc.Cancel);

			using ( _Prompts.Progress(config) )
			{
				try { await func().ConfigureAwait(true); }
				catch ( OperationCanceledException ) { }
				catch ( Exception e ) { await HandleExceptionAsync(e, page, cancelSrc.Token).ConfigureAwait(true); }
			}
		}


		public ICommand LoadingCommand( Action func, Page page, string cancel ) => LoadingCommand(func, page, cancel, MaskType.Black);

		public ICommand LoadingCommand( Action func, Page page, string cancel, MaskType mask ) => LoadingCommand(func,
																												 page,
																												 null,
																												 cancel,
																												 mask);

		public ICommand LoadingCommand( Action func,
										Page page,
										string? title,
										string cancel,
										MaskType mask
		) => new Command(async () => await LoadingAction(func,
														 title,
														 cancel,
														 mask,
														 page).ConfigureAwait(true));

		public async Task LoadingAction( Action func,
										 string? title,
										 string cancel,
										 MaskType mask,
										 Page page
		)
		{
			if ( func is null ) throw new ArgumentNullException(nameof(func));

			using var cancelSrc = new CancellationTokenSource();
			ProgressDialogConfig config = new ProgressDialogConfig().SetTitle(title).SetIsDeterministic(false).SetMaskType(mask).SetCancel(cancel, cancelSrc.Cancel);

			using ( _Prompts.Progress(config) )
			{
				try { func(); }
				catch ( OperationCanceledException ) { }
				catch ( Exception e ) { await HandleExceptionAsync(e, page, cancelSrc.Token).ConfigureAwait(true); }
			}
		}


		public virtual Task HandleExceptionAsync( Exception e, Page page, CancellationToken token ) => throw new NotImplementedException();
		public virtual Task HandleExceptionAsync<TFeedBackPage>( Exception e, Page page, CancellationToken token ) where TFeedBackPage : Page, new() => throw new NotImplementedException();

		public async Task<bool> HandleExceptionAsync( Exception e, CancellationToken token )
		{
			if ( token.IsCancellationRequested ) return false;
			return await HandleExceptionAsync(e).ConfigureAwait(true);
		}

		public async Task<bool> HandleExceptionAsync( Exception e )
		{
			if ( e is null ) throw new ArgumentNullException(nameof(e));

			switch ( e )
			{
				case OperationCanceledException:
				case NameResolutionException:
				case RequestAbortedException:
					break;

				case TimeoutException: break;

				default:
					await _Debug.HandleExceptionAsync(e).ConfigureAwait(true);
					break;
			}

			return !InternalHandleException(e);
		}

		public bool HandleException( Exception e )
		{
			if ( !( e is OperationCanceledException ) && !( e is NameResolutionException ) && !( e is RequestAbortedException ) && !( e is TimeoutException ) ) { _Debug.HandleException(e); }

			return InternalHandleException(e);
		}


		protected virtual bool InternalHandleException( Exception e ) => true; // switch the type of exception to show what ever prompt you want


		public async Task SendFeedBack<TFeedBackPage>( string title,
													   string message,
													   string yes,
													   string no,
													   string sendFeedBackPrompt,
													   Page page,
													   Exception e
		) where TFeedBackPage : Page, new()
		{
			if ( page is null ) throw new ArgumentNullException(nameof(page));
			if ( e is null ) throw new ArgumentNullException(nameof(e));

			await _Debug.HandleExceptionAsync(e).ConfigureAwait(true);

			if ( await Check(title, $"{message}\n\n{sendFeedBackPrompt}", yes, no).ConfigureAwait(true) )
			{
				_Services.ScreenShotAddress = await Share.GetScreenShot().ConfigureAwait(true);

				await page.Navigation.PushAsync(new TFeedBackPage()).ConfigureAwait(true);
			}
			else { _Services.ScreenShotAddress = null; }
		}


		public async Task<bool> Check( string title, string message, string yes, string no ) => await _Prompts.ConfirmAsync(message, title, yes, no).ConfigureAwait(true);
		protected void Alert( string title, string message, string ok ) => _Prompts.Alert(message, title, ok);


		internal void DebugMessage( Exception e, string ok, [CallerMemberName] string caller = "" )
		{
			if ( !_Debug.CanDebug ) return;

			if ( !string.IsNullOrWhiteSpace(caller) ) { caller = $"DEBUG: {caller}"; }

			//if ( !Debug.CanDebug )
			//{
			//	Alert(e.Message, caller);
			//	return;
			//}

			Alert(caller, e.ToString(), ok);
		}
	}
}
