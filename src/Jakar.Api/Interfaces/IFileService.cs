// unset

using System;
using System.IO;
using System.Threading.Tasks;

namespace Jakar.Api.Interfaces
{
	public interface IFileService
	{
		public Task<FileInfo> DownloadFile( Uri link, string fileName );
	}
}