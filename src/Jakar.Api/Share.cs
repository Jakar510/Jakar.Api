using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Jakar.Api.Enumerations;
using Jakar.Api.Extensions;
using Jakar.Api.Interfaces;
using Jakar.Api.Models;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Screenshot;
using Xamarin.Essentials;
using Xamarin.Forms;

#pragma warning disable 1591

#nullable enable
namespace Jakar.Api
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public static class Share
	{
		private static ShareTextRequest GetTextRequest( string title, string text, string uri ) =>
			new(text, title)
			{
				Uri = uri,
			};
		private static IFileService _FileService { get; } = DependencyService.Get<IFileService>();

		public static async Task ShareRequest( string title, string text, Uri uri ) => await ShareRequest(title, text, uri.ToString()).ConfigureAwait(true);
		public static async Task ShareRequest( string title, string text, string uri )
		{
			try { await Xamarin.Essentials.Share.RequestAsync(GetTextRequest(title, text, uri)).ConfigureAwait(true); }
			catch ( Exception e ) { await Debug.Current.HandleExceptionAsync(e).ConfigureAwait(true); }
		}


		public static async Task<bool> ShareFile( Uri uri, string shareTitle ) => await ShareFile(uri.ToString() ?? throw new ArgumentNullException(nameof(uri)), shareTitle).ConfigureAwait(true);
		public static async Task<bool> ShareFile( FileInfo info, string shareTitle ) => await ShareFile(info.FullName ?? throw new ArgumentNullException(nameof(info)), shareTitle).ConfigureAwait(true);
		public static async Task<bool> ShareFile( string filePath, string shareTitle ) => await ShareFile(new ShareFile(filePath), shareTitle).ConfigureAwait(true);
		public static async Task<bool> ShareFile( Uri uri, string shareTitle, string mime ) => await ShareFile(uri.ToString() ?? throw new ArgumentNullException(nameof(uri)), shareTitle, mime).ConfigureAwait(true);
		public static async Task<bool> ShareFile( FileInfo info, string shareTitle, string mime ) => await ShareFile(info.FullName ?? throw new ArgumentNullException(nameof(info)), shareTitle, mime).ConfigureAwait(true);
		public static async Task<bool> ShareFile( string filePath, string shareTitle, string mime ) => await new ShareFile(filePath, mime).ShareFile(shareTitle).ConfigureAwait(true);
		public static async Task<bool> ShareFile( this ShareFile shareFile, string shareTitle )
		{
			try
			{
				var request = new ShareFileRequest(shareTitle, shareFile);
				await Xamarin.Essentials.Share.RequestAsync(request).ConfigureAwait(true);

				return true;
			}
			catch ( Exception e )
			{
				await Debug.Current.HandleExceptionAsync(e).ConfigureAwait(true);
				return false;
			}
		}


		public static async Task<LocalFile?> OpenOfficeDoc( Uri link, MimeType mime )
		{
			try
			{
				LocalFile info = await _FileService.DownloadFile(link, mime.ToFileName() ?? throw new NullReferenceException(nameof(mime))).ConfigureAwait(true);

				var url = info.ToUri(mime);

				if ( Device.RuntimePlatform == Device.Android )
					await Launcher.OpenAsync(url).ConfigureAwait(true);
				else
					await ShareFile(info.Info, "", mime.ToString()).ConfigureAwait(true);

				return info;
			}
			catch ( Exception e ) { await Debug.Current.HandleExceptionAsync(e).ConfigureAwait(true); }

			return default;
		}
		public static async Task<bool> Open( string url ) => await Open(new Uri(url)).ConfigureAwait(true);
		public static async Task<bool> Open( Uri url )
		{
			if ( await Launcher.CanOpenAsync(url).ConfigureAwait(true) ) { return await Launcher.TryOpenAsync(url).ConfigureAwait(true); }

			return false;
		}
		public static async Task<bool> OpenBrowser( Uri uri )
		{
			try
			{
				await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred).ConfigureAwait(true);
				return true;
			}
			catch ( Exception e )
			{
				await Debug.Current.HandleExceptionAsync(e).ConfigureAwait(true);
				return false;
			}
		}


		public static void SetupCrossMedia( Page page, string title, string message, string ok )
		{
			if ( page is null )
				throw new ArgumentNullException(nameof(page));

			MainThread.BeginInvokeOnMainThread(async () =>
											   {
												   await CrossMedia.Current.Initialize().ConfigureAwait(true);

												   if ( CrossMedia.Current.IsCameraAvailable ) { return; }

												   await page.DisplayAlert(title, message, ok).ConfigureAwait(true);
												   await page.Navigation.PopAsync().ConfigureAwait(true);
											   });
		}


		public static async Task<MediaFile> TakePhoto( StoreCameraMediaOptions? options = null, CancellationToken token = default )
		{
			options ??= new StoreCameraMediaOptions()
						{
							SaveMetaData = true,
							SaveToAlbum = true,
							DefaultCamera = CameraDevice.Rear,
							PhotoSize = PhotoSize.Full,
							RotateImage = false,
							AllowCropping = false,
							Location = await LocationManager.GetLocation().ConfigureAwait(true),
						};
			MediaFile photo = await CrossMedia.Current.TakePhotoAsync(options, token).ConfigureAwait(true);
			return photo;
		}
		public static async Task<MediaFile> GetPhoto( PickMediaOptions? options = null, CancellationToken token = default )
		{
			options ??= new PickMediaOptions()
						{
							PhotoSize = PhotoSize.Full,
							SaveMetaData = true,
							RotateImage = false,
							ModalPresentationStyle = MediaPickerModalPresentationStyle.FullScreen,
						};
			MediaFile photo = await CrossMedia.Current.PickPhotoAsync(options, token).ConfigureAwait(true);
			return photo;
		}

		public static async Task<MediaFile> TakeVideo( StoreVideoOptions? options = null, CancellationToken token = default )
		{
			options ??= new StoreVideoOptions()
						{
							Quality = VideoQuality.High,
							SaveMetaData = true,
							SaveToAlbum = true,
							DefaultCamera = CameraDevice.Rear,
							PhotoSize = PhotoSize.Full,
							RotateImage = false,
							AllowCropping = false,
							Location = await LocationManager.GetLocation().ConfigureAwait(true),
						};
			MediaFile photo = await CrossMedia.Current.TakeVideoAsync(options, token).ConfigureAwait(true);
			return photo;
		}
		public static async Task<MediaFile> GetVideo( CancellationToken token = default ) => await CrossMedia.Current.PickVideoAsync(token).ConfigureAwait(true);


		private static Memory<byte> _ScreenShotBuffer { get; set; }
		public static bool ScreenShotAvailable => _ScreenShotBuffer.IsEmpty;
		public static async Task BufferScreenShot()
		{
			byte[] bytes = await CrossScreenshot.Current.CaptureAsync().ConfigureAwait(true);

			_ScreenShotBuffer = new Memory<byte>(bytes);
		}
		public static async Task<string> GetScreenShot()
		{
			byte[] screenShot = await TakeScreenShot().ConfigureAwait(true);
			return await WriteScreenShot(screenShot).ConfigureAwait(true);
		}
		public static async Task<byte[]> TakeScreenShot() => await MainThread.InvokeOnMainThreadAsync(CrossScreenshot.Current.CaptureAsync).ConfigureAwait(true);

		public static async Task<string> WriteScreenShot() => await WriteScreenShot(_ScreenShotBuffer.ToArray()).ConfigureAwait(true);
		public static async Task<string> WriteScreenShot( byte[] screenShot )
		{
			await FileSystem.Current.WriteToFileAsync(FileSystem.ScreenShot, screenShot).ConfigureAwait(true);

			return FileSystem.ScreenShot;
		}


		// public static Image GetScreenShotImage() =>
		// 	new()
		// 	{
		// 		Source = GetScreenShotImageSource()
		// 	};
		// public static ImageSource GetScreenShotImageSource() => ImageSource.FromStream(GetScreenShotStream);
		// public static async Task<ScreenshotResult> TakeScreenShotResult() => await MainThread.InvokeOnMainThreadAsync(Screenshot.CaptureAsync).ConfigureAwait(true);
		// public static async Task<Stream> GetScreenShotStream( CancellationToken token )
		// {
		// 	ScreenshotResult result = await Screenshot.CaptureAsync().ConfigureAwait(true);
		// 	token.ThrowIfCancellationRequested();
		// 	return await result.OpenReadAsync().ConfigureAwait(true);
		// }


		public static ImageSource GetImageSource( MediaFile file ) => ImageSource.FromStream(file.GetStream);
		public static UriImageSource GetImageSource( string url ) => GetImageSource(new Uri(url), 5);
		public static UriImageSource GetImageSource( string url, int days ) => GetImageSource(new Uri(url), days);
		public static UriImageSource GetImageSource( Uri url, int days ) => GetImageSource(url, new TimeSpan(days, 0, 0, 0));
		public static UriImageSource GetImageSource( Uri url, TimeSpan time ) =>
			new()
			{
				Uri = url,
				CachingEnabled = true,
				CacheValidity = time
			};
	}
}