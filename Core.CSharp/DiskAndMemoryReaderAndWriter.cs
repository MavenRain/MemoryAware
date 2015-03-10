using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
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
                WriteAsync(BackingStore.Last().Key, BackingStore.Last().Value).GetResults();
                BackingStore.Remove(BackingStore.Last().Key);
            }
        }

        static DiskAndMemoryReaderAndWriter()
        {
            MemoryWatcher.ApplicationMemoryCriticalLimitReached += TakeMemoryCriticalAction;
        }

        private static readonly Dictionary<string, IList<byte>> BackingStore = new Dictionary<string, IList<byte>>();

        public static bool IsOnlyDiskReadAndWritePreferred { get; set; }

	    public static IAsyncAction WriteAsync(string identifier,  IList<byte> content)
	    {
		    if (IsOnlyDiskReadAndWritePreferred)
		    {
			    var storageFolder = Package.Current.InstalledLocation.CreateFolderAsync(
				    "DiskAndMemoryReaderAndWriter", CreationCollisionOption.OpenIfExists);
			    return FileIO.WriteBytesAsync(
				    storageFolder.GetResults().CreateFileAsync(identifier, CreationCollisionOption.ReplaceExisting).GetResults(),
				    content.ToArray());
		    }
		    BackingStore.Add(identifier, content);
		    return ThreadPool.RunAsync((workItem) => { });
	    }

	    public static IAsyncOperation<IList<byte>> ReadAsync(string identifier)
	    {
		    if (BackingStore.ContainsKey(identifier))
		    {
			    return Task.Run(() => BackingStore[identifier]).AsAsyncOperation();
		    }

			var file = Package.Current.InstalledLocation.CreateFolderAsync(
                        "DiskAndMemoryReaderAndWriter",CreationCollisionOption.OpenIfExists).GetResults().GetFileAsync(identifier);
			return file.GetResults() == null ? Task.Run(() => (IList<byte>)new List<byte>()).AsAsyncOperation() : Task.Run(() => (IList<byte>)FileIO.ReadBufferAsync(file.GetResults()).GetResults().ToArray().ToList()).AsAsyncOperation();
	    }

        public static IAsyncAction DeleteAsync(string identifier)
        {
            if (BackingStore.ContainsKey(identifier))
            {
	            return Task.Run(() => BackingStore.Remove(identifier)).AsAsyncAction();
            }
            try
            {
                return Package.Current.InstalledLocation.GetFolderAsync(
                    "DiskAndMemoryReaderAndWriter").GetResults().GetFileAsync(identifier).GetResults().DeleteAsync();
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                FileIO.WriteTextAsync(Package.Current.InstalledLocation.CreateFolderAsync(
                    "DiskAndMemoryReaderAndWriter", CreationCollisionOption.OpenIfExists)
                    .GetResults()
                    .CreateFileAsync("DiskAndMemoryReaderAndWriterErrorLog.txt", CreationCollisionOption.ReplaceExisting).GetResults(), "An attempt to delete a file that did not exists was made.").GetResults();
                throw new FileNotFoundException("An attempt to delete a file that did not exists was made.  Most likely, a delete occured before a creation, or two deletions happens in succession.", "DiskAndMemoryReaderAndWriterErrorLog.txt", fileNotFoundException);
            }
        }

        public static IAsyncAction ReplaceAsync(string oldIdentifier, string newIdentifier, IList<byte> content)
        {
			DeleteAsync(oldIdentifier).GetResults();
	        return WriteAsync(newIdentifier, content);
        }
    }
}
