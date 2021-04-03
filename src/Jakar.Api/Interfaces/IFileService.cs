// unset

using System;
using System.IO;
using System.Threading.Tasks;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Interfaces
{
	public interface IFileService
	{
		public Task<FileInfo> DownloadFile( Uri link, string fileName );
	}
}
