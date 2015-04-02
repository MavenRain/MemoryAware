using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.System.Threading;

namespace Core.CSharp
{
    public static class DiskAndMemoryReaderAndWriter
    {
        static void TakeMemoryCriticalAction(object sender, object e)
        {
            var ejectionCount = BackingStore.Count / 2;
            for (var removalIndex = 0; removalIndex < ejectionCount; ejectionCount++)
            {
                BlockToWrite();
                BackingStore.Remove(BackingStore.Last().Key);
            }
        }

        static async void BlockToWrite()
        {
            await WriteAsync(BackingStore.Last().Key, BackingStore.Last().Value);
        }

        static DiskAndMemoryReaderAndWriter()
        {
            MemoryWatcher.ApplicationMemoryCriticalLimitReached += TakeMemoryCriticalAction;
        }

        static readonly IDictionary<string, IEnumerable<byte>> BackingStore = new Dictionary<string, IEnumerable<byte>>();

        public static bool IsOnlyDiskReadAndWritePreferred { get; set; }

	    public static IAsyncAction WriteAsync(string identifier,  IEnumerable<byte> content)
	    {
		    return ThreadPool.RunAsync(workItem =>
		    {
                if (IsOnlyDiskReadAndWritePreferred)
                {
                    var storageFolder = ApplicationData.Current.LocalFolder.CreateFolderAsync(
                        "DiskAndMemoryReaderAndWriter", CreationCollisionOption.OpenIfExists).AsTask();
                    ForceWriteToDisk(storageFolder, identifier, content).ConfigureAwait(true);
                }
                else BackingStore.Add(new KeyValuePair<string, IEnumerable<byte>>(identifier, content));
		    });
	    }

        static async Task ForceWriteToDisk(Task<StorageFolder> storageFolder, string identifier, IEnumerable<byte> content)
        {
            await FileIO.WriteBytesAsync(await (await storageFolder).CreateFileAsync(identifier, CreationCollisionOption.ReplaceExisting), content.ToArray());
        }

	    public static IAsyncOperation<IEnumerable<byte>> ReadAsync(string identifier)
	    {
	        return BackingStore.ContainsKey(identifier) ? Task.Run(() => BackingStore[identifier]).AsAsyncOperation() : GetByteArray(GetStorageFile(identifier)).AsAsyncOperation();
	    }

        static async Task<IEnumerable<byte>> GetByteArray(Task<StorageFile> storageFileTask)
        {
            return (IEnumerable<byte>)(await FileIO.ReadBufferAsync(await storageFileTask)).ToArray().ToList();
        }

	    static async Task<StorageFile> GetStorageFile(string identifier)
	    {
		    return await (await ApplicationData.Current.LocalFolder.CreateFolderAsync(
				"DiskAndMemoryReaderAndWriter", CreationCollisionOption.OpenIfExists)).GetFileAsync(identifier);
	    }

        public static IAsyncAction DeleteAsync(string identifier)
        {
            if (BackingStore.ContainsKey(identifier))
            {
	            return Task.Run(() => BackingStore.Remove(identifier)).AsAsyncAction();
            }
            try
            {
                return AttemptToDeleteFromDisk(identifier).AsAsyncAction();
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                HandleError();
                throw new FileNotFoundException("An attempt to delete a file that did not exists was made.  Most likely, a delete occured before a creation, or two deletions happens in succession.", "DiskAndMemoryReaderAndWriterErrorLog.txt", fileNotFoundException);
            }
        }

        static async void HandleError()
        {
            await FileIO.WriteTextAsync(await (await ApplicationData.Current.LocalFolder.CreateFolderAsync("DiskAndMemoryReaderAndWriter", CreationCollisionOption.OpenIfExists)).CreateFileAsync("DiskAndMemoryReaderAndWriterErrorLog.txt", CreationCollisionOption.ReplaceExisting), "An attempt to delete a file that did not exists was made.");
        }

        static async Task AttemptToDeleteFromDisk(string identifier)
        {
            await (await (await ApplicationData.Current.LocalFolder.GetFolderAsync("DiskAndMemoryReaderAndWriter")).GetFileAsync(identifier)).DeleteAsync();
        }

        public static IAsyncAction ReplaceAsync(string oldIdentifier, string newIdentifier, IEnumerable<byte> content)
        {
			DeleteAsync(oldIdentifier).GetResults();
	        return WriteAsync(newIdentifier, content);
        }
    }
}
