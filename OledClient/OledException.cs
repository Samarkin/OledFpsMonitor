using System;

namespace Samarkin.Oled
{
	public class OledException : Exception
	{
		public OledException(string message, Exception? innerException = null)
			: base(message, innerException)
		{
		}
	}
}
