// unset

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Models
{
	public class FileData : LocalFile
	{
		public string? Data { get; }
		public ReadOnlyMemory<byte>? Payload { get; }


		public FileData( string path, string data ) : base(path) => Data = data;
		public FileData( string path, string name, string data ) : base(path, name) => Data = data;
		public FileData( FileInfo path, string data ) : base(path) => Data = data;
		public FileData( FileInfo path, string name, string data ) : base(path, name) => Data = data;


		public FileData( string path, byte[] data ) : base(path) => Payload = data.AsMemory();
		public FileData( string path, string name, byte[] data ) : base(path, name) => Payload = data.AsMemory();
		public FileData( FileInfo path, byte[] data ) : base(path) => Payload = data.AsMemory();
		public FileData( FileInfo path, string name, byte[] data ) : base(path, name) => Payload = data.AsMemory();
	}
}