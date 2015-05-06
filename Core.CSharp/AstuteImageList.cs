using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using Core.CSharp.Interface;

namespace Core.CSharp
{
    public sealed class AstuteImageList : IAmAnAstuteImageCollection
    {
        readonly Astute<BitmapImage> imageStore; 

        AstuteImageList() { }

        public AstuteImageList(string filename)
        {
            imageStore = new Astute<BitmapImage>(filename);
        }

        /// <summary>
        /// Asynchronously get list of things from either disk or memory
        /// </summary>
        /// <returns></returns>
        public IAsyncOperation<IEnumerable<BitmapImage>> GetValueAsync()
            => imageStore.GetValue().AsAsyncOperation();

        /// <summary>
        /// Asynchronously adds a list of things to memory
        /// </summary>
        /// <param name="listOfImages"></param>
        /// <returns></returns>
        public IAsyncAction PutValueAsync(IEnumerable<BitmapImage> listOfImages)
            => imageStore.PutValue(listOfImages).AsAsyncAction();
    }

    class Astute<T>
    {
        readonly string file;
        List<T> cache;
        internal Astute(string filename)
        {
            file = filename;
            cache = new List<T>();
            MemoryWatcher.ApplicationMemoryCriticalLimitReached += DoWhenMemoryIsCritical;
        }

        /// <summary>
        /// Gets list of things stored
        /// First, checks disk for stored items, and, if not there, retrieves them from memory
        /// </summary>
        /// <returns>Enumerable list of things</returns>
        internal async Task<IEnumerable<T>> GetValue()
        {
            var buffer = await FileIO.ReadBufferAsync(await KnownFolders.PicturesLibrary.GetFileAsync(file));
            await (await KnownFolders.PicturesLibrary.GetFileAsync(file)).DeleteAsync();
            if (buffer.Length == 0) return cache ?? new List<T>();
            using (var stream = buffer.AsStream())
            {
                var formatter = new DataContractSerializer(typeof(List<T>));
                return (List<T>)formatter.ReadObject(stream);
            }    
        }

        /// <summary>
        /// Adds list of things to the internal cache
        /// </summary>
        /// <param name="value">list of things to store</param>
        /// <returns></returns>
        internal async Task PutValue(IEnumerable<T> value)
        {
            await Task.Run(() => cache.AddRange(value));
        }

        /// <summary>
        /// When memory warning is given, serializes a list of things to disk and removes them from memory as soon as possible
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void DoWhenMemoryIsCritical(object sender, object e)
        {
            var formatter = new DataContractSerializer(typeof(List<T>));
            var stream = new MemoryStream();
            formatter.WriteObject(stream, cache);
            cache = null;
            await FileIO.WriteBufferAsync(await KnownFolders.PicturesLibrary.GetFileAsync(file), stream.GetWindowsRuntimeBuffer());
        }
    }
}
