using System;
using System.Collections.Generic;

namespace JanusChess.NET
{
	internal class Log
	{
		public delegate void LogHandler(object sender, LogEventArgs args);

		public event LogHandler OnNewEntry;

		public enum Level
		{
			Info
		}

		public Level DefaultLogLevel { get; set; } = Level.Info;

		private readonly LinkedList<Entry> _logEntries = new LinkedList<Entry>();

		public void Write(string message) => Write(DefaultLogLevel, message);

		public void Write(Level level, string message)
		{
			if (string.IsNullOrWhiteSpace(message)) return;
			var e = new Entry(level, message);
			_logEntries.AddLast(e);
			OnNewEntry?.Invoke(this, new LogEventArgs(e));
		}

		public struct Entry
		{
			public Entry(Level level, string message)
			{
				Level = level;
				Message = message;
			}

			public Level Level { get; }
			public string Message { get; }
		}

		public class LogEventArgs : EventArgs
		{
			public LogEventArgs(Entry entry)
			{
				LogEntry = entry;
			}

			public Entry LogEntry { get; }
		}
	}
}