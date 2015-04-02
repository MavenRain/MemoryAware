using System.Collections.Generic;
using Windows.Foundation;

namespace Core.CSharp.Interface
{
	public interface IDiskStreamable
	{
		void SubscribeToMemoryWatcher(IMemoryWatchable memoryWatcher);
	    void TakeCriticalAction(object sender, object e);
		IAsyncAction WriteAsync(string identifier, IEnumerable<byte> content, bool isOnlyDiskIoPreferred);
		IAsyncOperation<IEnumerable<byte>> ReadAsync(string identifier);
		IAsyncAction DeleteAsync(string identifier);
		IAsyncAction ReplaceAsync(string oldIdentifier, string newIdentifier, IEnumerable<byte> content);
	}
}
