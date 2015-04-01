using System;
using System.Collections.Generic;
using Windows.Foundation;

namespace Core.CSharp.Interface
{
	public interface IDiskStreamable
	{
		void SubscribeToMemoryWatcher(IMemoryWatchable memoryWatcher);
		IAsyncAction WriteAsync(string identifier, IEnumerable<byte> content);
		IAsyncOperation<IEnumerable<byte>> ReadAsync(string identifier);
		IAsyncAction DeleteAsync(string identifier);
		IAsyncAction ReplaceAsync(string oldIdentifier, string newIdentifier, IEnumerable<byte> content);
	}
}
