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
	public abstract class Prompts
	{
		protected internal abstract string Cancel { get; }
		protected internal abstract string Ok { get; }
		protected internal abstract string Yes { get; }
		protected internal abstract string No { get; }

		protected internal IUserDialogs Dialogs { get; } = UserDialogs.Instance;

		private IAppSettings? _services;

		protected IAppSettings _Services
		{
			get => _services ?? throw new ApiDisabledException($"Must call {nameof(Init)} first.", new NullReferenceException(nameof(_services)));
			private set => _services = value;
		}

		private Debug? _debug;

		protected Debug _Debug
		{
			get => _debug ?? throw new ApiDisabledException($"Must call {nameof(Init)} first.", new NullReferenceException(nameof(_services)));
			private set => _debug = value;
		}

		public void Init( IAppSettings services ) => _Services = services;
		public void Init( Debug services ) => _Debug = services;


		public void ShowLoading( string title ) => ShowLoading(title, MaskType.Black);
		public void ShowLoading( string title, MaskType mask ) => Dialogs.ShowLoading(title, mask);
		public void HideLoading() => Dialogs.HideLoading();

		public abstract Task HandleExceptionAsync( Exception e, Page page, CancellationToken token );
		public abstract Task HandleExceptionAsync<TFeedBackPage>( Exception e, Page page, CancellationToken token ) where TFeedBackPage : Page, new();

		public async Task SendFeedBack<TFeedBackPage>( string title,
													   string message,
													   Page page,
													   Exception e
		) where TFeedBackPage : Page, new()
		{
			await SendFeedBack<TFeedBackPage>(title,
											  message,
											  Yes,
											  No,
											  page,
											  e).ConfigureAwait(true);
		}

		public async Task SendFeedBack<TFeedBackPage>( string title,
													   string message,
													   string yes,
													   string no,
													   Page page,
													   Exception e
		) where TFeedBackPage : Page, new()
		{
			if ( page is null ) throw new ArgumentNullException(nameof(page));
			if ( e is null ) throw new ArgumentNullException(nameof(e));

			await _Debug.HandleExceptionAsync(e).ConfigureAwait(true);

			if ( await Check(title, message, yes, no).ConfigureAwait(true) )
			{
				_Services.ScreenShotAddress = await Share.GetScreenShot().ConfigureAwait(true);

				await page.Navigation.PushAsync(new TFeedBackPage()).ConfigureAwait(true);
			}
			else { _Services.ScreenShotAddress = null; }
		}


		public async Task<bool> HandleExceptionAsync( Exception e, CancellationToken token )
		{
			if ( token.IsCancellationRequested ) { return false; }

			return await HandleExceptionAsync(e).ConfigureAwait(true);
		}

		public async Task<bool> HandleExceptionAsync( Exception e )
		{
			await InternalHandleExceptionAsync(e).ConfigureAwait(true);

			return InternalHandleException(e);
		}

		public bool HandleException( Exception e )
		{
			Task.Run(async () => await InternalHandleExceptionAsync(e).ConfigureAwait(true));

			return InternalHandleException(e);
		}


		protected virtual async Task InternalHandleExceptionAsync( Exception e )
		{
			switch ( e )
			{
				case null: throw new ArgumentNullException(nameof(e));

				case OperationCanceledException:
				case NameResolutionException:
				case RequestAbortedException:
				case TimeoutException:
					return;


				default:
					await _Debug.HandleExceptionAsync(e).ConfigureAwait(true);
					return;
			}
		}


		/// <summary>
		/// switch the type of exception to show what ever prompt you want
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		protected abstract bool InternalHandleException( Exception e );


		protected void Alert( string title, string message ) => Dialogs.Alert(message, title, Ok);
		protected void Alert( string title, string message, string ok ) => Dialogs.Alert(message, title, ok);
		protected async Task AlertAsync( string title, string message, string ok ) => await Dialogs.AlertAsync(message, title, ok);


		public async Task<bool> Check( string title, string message ) => await Dialogs.ConfirmAsync(message, title, Yes, No).ConfigureAwait(true);
		public async Task<bool> Check( string title, string message, string yes, string no ) => await Dialogs.ConfirmAsync(message, title, yes, no).ConfigureAwait(true);


		public void DebugMessage( Exception e, [CallerMemberName] string caller = "" )
		{
			if ( !_Debug.CanDebug ) return;

			if ( !string.IsNullOrWhiteSpace(caller) ) { caller = $"DEBUG: {caller}"; }

			//if ( !Debug.CanDebug )
			//{
			//	Alert(e.Message, caller);
			//	return;
			//}

			Alert(caller, e.ToString());
		}
	}
}
