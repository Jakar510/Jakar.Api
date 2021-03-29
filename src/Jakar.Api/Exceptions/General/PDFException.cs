using System;
using System.Runtime.Serialization;

namespace Jakar.Api.Exceptions.General
{
	[Serializable]
	public class ScreenShotException : Exception
	{
		public ScreenShotErrorArgs? Args { get; set; }

		public ScreenShotException() { }
		public ScreenShotException( ScreenShotErrorArgs e ) => Args = e;
		public ScreenShotException( string message, ScreenShotErrorArgs e ) : base(message) => Args = e;
		public ScreenShotException( string message, Exception innerException, ScreenShotErrorArgs e ) : base(message, innerException) => Args = e;
		protected ScreenShotException( SerializationInfo info, StreamingContext context, ScreenShotErrorArgs e ) : base(info, context) => Args = e;

		public ScreenShotException( string message ) : base(message) { }
		public ScreenShotException( string message, Exception innerException ) : base(message, innerException) { }
	}



	public class ScreenShotErrorArgs : EventArgs
	{
		public string ScreenShotFilePath { get; }
		public ScreenShotErrorArgs( string path ) => ScreenShotFilePath = path;
	}
}