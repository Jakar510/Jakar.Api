using System.Threading;
using System.Threading.Tasks;
using Plugin.LatestVersion;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Statics
{
	public static class AppStoreVersion
	{
		// https://github.com/edsnider/latestversionplugin

		public static async Task<bool> IsLatest() => await CrossLatestVersion.Current.IsUsingLatestVersion().ConfigureAwait(true);
		public static async Task<string> LatestVersionNumber() => await CrossLatestVersion.Current.GetLatestVersionNumber().ConfigureAwait(true);
		public static string InstalledVersionNumber => CrossLatestVersion.Current.InstalledVersionNumber;
		public static async Task OpenAppInStore() => await CrossLatestVersion.Current.OpenAppInStore().ConfigureAwait(true);

		public static async Task<bool> VerifyAsync( Prompts prompts,
													string newVersionAvailable,
													string newVersionUpdateNowOrLater,
													CancellationToken token = default
		)
		{
			bool isLatest = await IsLatest().ConfigureAwait(true);

			if ( isLatest ) { return false; }

			bool update = await prompts.ConfirmAsync(newVersionAvailable,
													 newVersionUpdateNowOrLater,
													 token).ConfigureAwait(true);

			if ( !update ) { return false; }

			await OpenAppInStore().ConfigureAwait(true);
			return true;
		}
		public static async Task<bool> VerifyAsync( Prompts prompts,
													string newVersionAvailable,
													string newVersionUpdateNowOrLater,
													string yes,
													string no,
													CancellationToken token = default
		)
		{
			bool isLatest = await IsLatest().ConfigureAwait(true);

			if ( isLatest ) { return false; }

			bool update = await prompts.ConfirmAsync(newVersionAvailable,
													 newVersionUpdateNowOrLater,
													 yes,
													 no,
													 token).ConfigureAwait(true);

			if ( !update ) { return false; }

			await OpenAppInStore().ConfigureAwait(true);
			return true;
		}
	}
}
