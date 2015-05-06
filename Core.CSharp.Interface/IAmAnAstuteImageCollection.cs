using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Imaging;

namespace Core.CSharp.Interface
{
    public interface IAmAnAstuteImageCollection
    {
        IAsyncOperation<IEnumerable<BitmapImage>> GetValueAsync();
        IAsyncAction PutValueAsync(IEnumerable<BitmapImage> listOfImages);
    }
}
