using System;
using System.Runtime.CompilerServices;
using System.Threading;
using ns7;

namespace RevitAi.Core.AI;

public sealed class RevitDocumentChangeMonitor : IDisposable
{
	private object? object_0;

	private bool bool_0;

	private bool bool_1;

	[CompilerGenerated]
	private EventHandler? eventHandler_0;

	public bool IsMonitoring
	{
		get
		{
			if (bool_0)
			{
				return !bool_1;
			}
			return false;
		}
	}

	public object? CurrentDocument => object_0;

	public event EventHandler? DocumentChanged
	{
		[CompilerGenerated]
		add
		{
			EventHandler eventHandler = eventHandler_0;
			EventHandler eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler value2 = (EventHandler)Delegate.Combine(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref eventHandler_0, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
		[CompilerGenerated]
		remove
		{
			EventHandler eventHandler = eventHandler_0;
			EventHandler eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler value2 = (EventHandler)Delegate.Remove(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref eventHandler_0, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
	}

	public void StartMonitoring(object document)
	{
		if (bool_1)
		{
			throw new ObjectDisposedException("RevitDocumentChangeMonitor");
		}
		if (document == null)
		{
			throw new ArgumentNullException("document");
		}
		if (!bool_0 || object_0 != document)
		{
			if (bool_0)
			{
				StopMonitoring();
			}
			object_0 = document;
			bool_0 = true;
		}
	}

	public void StopMonitoring()
	{
		if (!bool_1)
		{
			bool_0 = false;
			object_0 = null;
		}
	}

	public void OnDocumentChanged()
	{
		if (!bool_1)
		{
			eventHandler_0?.Invoke(this, EventArgs.Empty);
		}
	}

	public void Dispose()
	{
		if (!bool_1)
		{
			StopMonitoring();
			bool_1 = true;
		}
	}
}
