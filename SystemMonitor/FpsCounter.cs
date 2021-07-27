using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Session;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Samarkin.SystemMonitor
{
	public sealed class FpsCounter : IDisposable
	{
		//event codes [https://github.com/GameTechDev/PresentMon/blob/40ee99f437bc1061a27a2fc16a8993ee8ce4ebb5/PresentData/PresentMonTraceConsumer.cpp]
		public const int EventID_D3D9PresentStart = 1;
		public const int EventID_DxgiPresentStart = 42;

		//ETW provider codes
		public static readonly Guid DXGI_provider = Guid.Parse("{CA11C036-0102-4A2D-A6AD-F03CFED5D3C9}");
		public static readonly Guid D3D9_provider = Guid.Parse("{783ACA0A-790E-4D7F-8451-AA850511C6B9}");

		private record FpsInfo(string ProcessName, Queue<double> FrameTimestamps);

		private static void TrimOldTimestamps(FpsInfo fpsInfo, double oldestTimestampToKeep)
		{
			Queue<double> frameTimestamps = fpsInfo.FrameTimestamps;
			while (frameTimestamps.Count > 0 && frameTimestamps.Peek() < oldestTimestampToKeep) {
				frameTimestamps.Dequeue();
			}
		}

		private static FpsInfo CreateEmptyFpsInfo(int processId)
		{
			using Process proc = Process.GetProcessById(processId);
			return new FpsInfo(proc.ProcessName, new Queue<double>());
		}

		private ConcurrentDictionary<int, FpsInfo> _fpsInfoPerProcess;
		private TraceEventSession _etwSession;

		public FpsCounter()
		{
			_fpsInfoPerProcess = new ConcurrentDictionary<int, FpsInfo>();
			_etwSession = new TraceEventSession("mysess")
			{
				StopOnDispose = true
			};
			_etwSession.Source.AllEvents += HandleEvent;
			_etwSession.EnableProvider("Microsoft-Windows-D3D9");
			_etwSession.EnableProvider("Microsoft-Windows-D3GI");
		}

		public void Start()
		{
			// TODO: Schedule cleanup?
			new Thread(() => _etwSession.Source.Process()).Start();
		}

		public string GetProcessName(int processId) => _fpsInfoPerProcess.GetOrAdd(processId, CreateEmptyFpsInfo).ProcessName;

		public int? GetFpsForProcess(int processId) => _fpsInfoPerProcess.TryGetValue(processId, out var fpsInfo) ? fpsInfo.FrameTimestamps.Count : null;

		private void HandleEvent(TraceEvent data)
		{
			//filter out frame presentation events
			if (((int)data.ID == EventID_D3D9PresentStart && data.ProviderGuid == D3D9_provider) ||
				((int)data.ID == EventID_DxgiPresentStart && data.ProviderGuid == DXGI_provider))
			{
				int processId = data.ProcessID;
				double t = data.TimeStampRelativeMSec;

				FpsInfo fpsInfo = _fpsInfoPerProcess.AddOrUpdate(processId, CreateEmptyFpsInfo,
					(_, info) =>
					{
						TrimOldTimestamps(info, t - 1000);
						info.FrameTimestamps.Enqueue(t);
						return new FpsInfo(info.ProcessName, info.FrameTimestamps);
					});
			}
		}

		public void Dispose()
		{
			_etwSession.Dispose();
			_fpsInfoPerProcess.Clear();
		}
	}
}
