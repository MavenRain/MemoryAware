using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.System.Threading;
using Core.CSharp.Interface;

namespace Core.CSharp
{
    public sealed class DiskStream : IDiskStreamable
    {
        IMemoryWatchable memWatcher;
        //Underlying byte array
        readonly IDictionary<string, IEnumerable<byte>> backingStore = new Dictionary<string, IEnumerable<byte>>();

        public void SubscribeToMemoryWatcher(IMemoryWatchable memoryWatcher)
        {
            memWatcher = memoryWatcher;
            memWatcher.ApplicationMemoryCriticalLimitReached += TakeCriticalAction;
        }

        public void TakeCriticalAction(object sender, object e)
        {
            WriteAsync(backingStore.Last().Key, backingStore.Last().Value, true);
        }

        public IAsyncAction WriteAsync(string identifier, IEnumerable<byte> content, bool isOnlyDiskIoPreferred)
        {
            return ThreadPool.RunAsync(workItem =>
            {
                if (isOnlyDiskIoPreferred)
                {
                    var storageFolder = ApplicationData.Current.LocalFolder.CreateFolderAsync(
                        "DiskAndMemoryReaderAndWriter", CreationCollisionOption.OpenIfExists).AsTask();
                    ForceWriteToDisk(storageFolder, identifier, content).ConfigureAwait(true);
                }
                else backingStore.Add(new KeyValuePair<string, IEnumerable<byte>>(identifier, content));
            });
        }

        static async Task ForceWriteToDisk(Task<StorageFolder> storageFolder, string identifier, IEnumerable<byte> content)
        {
            await FileIO.WriteBytesAsync(await (await storageFolder).CreateFileAsync(identifier, CreationCollisionOption.ReplaceExisting), content.ToArray());
        }
    }
}
