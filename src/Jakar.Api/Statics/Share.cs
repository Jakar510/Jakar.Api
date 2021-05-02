﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Jakar.Api.Extensions;
using Jakar.Extensions.Http;
using Jakar.Extensions.Interfaces;
using Jakar.Extensions.Models;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Screenshot;
using Xamarin.Essentials;
using Xamarin.Forms;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Statics
{
	public static class Share
	{
		// TODO: add MimType extensions and overloads for ShareFile

		private static ShareTextRequest GetTextRequest( string title, string text, string uri ) =>
			new(text, title)
			{
				Uri = uri,
			};

		private static IFileService _FileService { get; } = DependencyService.Get<IFileService>();

		public static async Task ShareRequest( this string title, string text, Uri uri ) => await ShareRequest(title, text, uri.ToString()).ConfigureAwait(true);

		public static async Task ShareRequest( this string title, string text, string uri ) { await Xamarin.Essentials.Share.RequestAsync(GetTextRequest(title, text, uri)).ConfigureAwait(true); }


		public static async Task ShareFile( this Uri uri, string shareTitle ) => await ShareFile(uri.ToString() ?? throw new ArgumentNullException(nameof(uri)), shareTitle).ConfigureAwait(true);

		public static async Task ShareFile( this FileInfo info, string shareTitle ) =>
			await ShareFile(info.FullName ?? throw new ArgumentNullException(nameof(info)), shareTitle).ConfigureAwait(true);

		public static async Task ShareFile( string filePath, string shareTitle ) => await ShareFile(new ShareFile(filePath), shareTitle).ConfigureAwait(true);

		public static async Task ShareFile( this Uri uri, string shareTitle, string mime ) =>
			await ShareFile(uri.ToString() ?? throw new ArgumentNullException(nameof(uri)), shareTitle, mime).ConfigureAwait(true);

		public static async Task ShareFile( this FileInfo info, string shareTitle, string mime ) =>
			await ShareFile(info.FullName ?? throw new ArgumentNullException(nameof(info)), shareTitle, mime).ConfigureAwait(true);

		public static async Task ShareFile( this string filePath, string shareTitle, string mime ) => await new ShareFile(filePath, mime).ShareFile(shareTitle).ConfigureAwait(true);

		public static async Task ShareFile( this ShareFile shareFile, string shareTitle )
		{
			var request = new ShareFileRequest(shareTitle, shareFile);
			await Xamarin.Essentials.Share.RequestAsync(request).ConfigureAwait(true);
		}


		public static async Task<LocalFile?> OpenOfficeDoc( this Uri link, string shareTitle, MimeType mime, IAppSettings settings ) =>
			await link.OpenOfficeDoc(shareTitle, mime, settings.AppName ?? throw new NullReferenceException(nameof(IAppSettings.AppName))).ConfigureAwait(true);

		public static async Task<LocalFile?> OpenOfficeDoc( this Uri link, string shareTitle, MimeType mime, string name )
		{
			LocalFile info = await _FileService.DownloadFile(link, mime.ToFileName(name)).ConfigureAwait(true);

			var url = info.ToUri(mime);

			if ( Device.RuntimePlatform == Device.Android ) { await Launcher.OpenAsync(url).ConfigureAwait(true); }
			else { await info.FileName.ShareFile(shareTitle, mime.ToString()).ConfigureAwait(true); }

			return info;
		}

		public static async Task<bool> Open( this string url ) => await Open(new Uri(url)).ConfigureAwait(true);

		public static async Task<bool> Open( this Uri url )
		{
			if ( await Launcher.CanOpenAsync(url).ConfigureAwait(true) ) { return await Launcher.TryOpenAsync(url).ConfigureAwait(true); }

			return false;
		}

		public static async Task OpenBrowser( this Uri uri, BrowserLaunchMode launchMode = BrowserLaunchMode.SystemPreferred ) { await Browser.OpenAsync(uri, launchMode).ConfigureAwait(true); }


		public static void SetupCrossMedia( this Page page, string title, string message, string ok )
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
							SaveMetaData  = true,
							SaveToAlbum   = true,
							DefaultCamera = CameraDevice.Rear,
							PhotoSize     = PhotoSize.Full,
							RotateImage   = false,
							AllowCropping = false,
							Location      = ( await LocationManager.GetLocation().ConfigureAwait(true) )?.ToPluginLocation(),
						};

			MediaFile photo = await CrossMedia.Current.TakePhotoAsync(options, token).ConfigureAwait(true);
			return photo;
		}

		public static async Task<MediaFile> GetPhoto( PickMediaOptions? options = null, CancellationToken token = default )
		{
			options ??= new PickMediaOptions()
						{
							PhotoSize              = PhotoSize.Full,
							SaveMetaData           = true,
							RotateImage            = false,
							ModalPresentationStyle = MediaPickerModalPresentationStyle.FullScreen,
						};

			MediaFile photo = await CrossMedia.Current.PickPhotoAsync(options, token).ConfigureAwait(true);
			return photo;
		}

		public static async Task<MediaFile> TakeVideo( StoreVideoOptions? options = null, CancellationToken token = default )
		{
			options ??= new StoreVideoOptions()
						{
							Quality       = VideoQuality.High,
							SaveMetaData  = true,
							SaveToAlbum   = true,
							DefaultCamera = CameraDevice.Rear,
							PhotoSize     = PhotoSize.Full,
							RotateImage   = false,
							AllowCropping = false,
							Location      = ( await LocationManager.GetLocation().ConfigureAwait(true) )?.ToPluginLocation(),
						};

			MediaFile photo = await CrossMedia.Current.TakeVideoAsync(options, token).ConfigureAwait(true);
			return photo;
		}

		public static async Task<MediaFile> GetVideo( CancellationToken token = default ) => await CrossMedia.Current.PickVideoAsync(token).ConfigureAwait(true);


		private static ReadOnlyMemory<byte> _ScreenShotBuffer   { get; set; }
		public static  bool                 ScreenShotAvailable => _ScreenShotBuffer.IsEmpty;

		public static async Task BufferScreenShot()
		{
			byte[] bytes = await TakeScreenShot().ConfigureAwait(true);

			_ScreenShotBuffer = new ReadOnlyMemory<byte>(bytes);
		}

		public static async Task<string> GetScreenShot( this FileSystemApi api )
		{
			byte[] screenShot = await TakeScreenShot().ConfigureAwait(true);
			return await api.WriteScreenShot(screenShot).ConfigureAwait(true);
		}

		public static async Task<byte[]> TakeScreenShot() => await MainThread.InvokeOnMainThreadAsync(CrossScreenshot.Current.CaptureAsync).ConfigureAwait(true);

		public static async Task<string> WriteScreenShot( this FileSystemApi api ) => await api.WriteScreenShot(_ScreenShotBuffer.ToArray()).ConfigureAwait(true);

		public static async Task<string> WriteScreenShot( this FileSystemApi api, byte[] screenShot )
		{
			string          path = api.ScreenShot;
			await using var file = new LocalFile(path);
			await file.WriteToFileAsync(screenShot).ConfigureAwait(true);

			return path;
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


		public static ImageSource    GetImageSource( this MediaFile file )          => ImageSource.FromStream(file.GetStream);
		public static UriImageSource GetImageSource( this string    url )           => GetImageSource(new Uri(url), 5);
		public static UriImageSource GetImageSource( this string    url, int days ) => GetImageSource(new Uri(url), days);
		public static UriImageSource GetImageSource( this Uri       url, int days ) => GetImageSource(url,          new TimeSpan(days, 0, 0, 0));

		public static UriImageSource GetImageSource( this Uri url, TimeSpan time ) =>
			new()
			{
				Uri            = url,
				CachingEnabled = true,
				CacheValidity  = time
			};
	}
}
