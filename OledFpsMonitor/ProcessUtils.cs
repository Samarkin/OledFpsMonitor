using System;
using System.Runtime.InteropServices;

namespace Samarkin.Oled.FpsMonitor
{
	public static class ProcessUtils
	{
		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", SetLastError = true)]
		private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

		public static int GetForegroundProcessId()
		{
			IntPtr hWnd = GetForegroundWindow();
			_ = GetWindowThreadProcessId(hWnd, out int processId);
			return processId;
		}
	}
}
