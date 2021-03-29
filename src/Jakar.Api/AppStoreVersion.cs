using System.Threading.Tasks;
using Plugin.LatestVersion;

namespace Jakar.Api
{
	internal static class AppStoreVersion
	{
		// https://github.com/edsnider/latestversionplugin

		public static async Task<bool> IsLatest() => await CrossLatestVersion.Current.IsUsingLatestVersion().ConfigureAwait(true);
		public static async Task<string> LatestVersionNumber() => await CrossLatestVersion.Current.GetLatestVersionNumber().ConfigureAwait(true);
		public static string InstalledVersionNumber => CrossLatestVersion.Current.InstalledVersionNumber;
		public static async Task OpenAppInStore() => await CrossLatestVersion.Current.OpenAppInStore().ConfigureAwait(true);

		public static async Task<bool> VerifyAsync(string NewVersionAvailable, string NewVersionUpdateNowOrLater, string yes, string no)
		{
			bool isLatest = await IsLatest().ConfigureAwait(true);

			if ( isLatest ) { return false; }

			bool update = await Prompts.Current.Check(NewVersionAvailable, NewVersionUpdateNowOrLater, yes, no).ConfigureAwait(true);

			if ( !update ) { return false; }

			await OpenAppInStore().ConfigureAwait(true);
			return true;
		}
	}
}