using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Core.CSharp;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace StorageRetrievalSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private readonly AstuteImageList cachedPictures;
        private readonly ObservableCollection<Pictures> pictureCollection = new ObservableCollection<Pictures>();
        private readonly List<StorageFile> pictureFileStorage = new List<StorageFile>();
        public MainPage()
        {
            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Required;

            cachedPictures = new AstuteImageList(storedFilename.Text);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

		private void storageSample_Click(object sender, RoutedEventArgs e)
		{
		    (new FileOpenPicker()
		    {
		        ViewMode = PickerViewMode.Thumbnail,
		        SuggestedStartLocation = PickerLocationId.PicturesLibrary
		    }).PickMultipleFilesAndContinue();

		}

        public void ContinueFromPicker(FileOpenPickerContinuationEventArgs args)
        {
            foreach (var file in args.Files)
            {
                pictureFileStorage.Add(file);
                pictureCollection.Add(new Pictures {PictureValue = file.Path});
            }
        }

        private async void putButton_Click(object sender, RoutedEventArgs e)
        {
            var enumerableCollection = new List<BitmapImage>();
            foreach (var pictureReference in pictureFileStorage)
            {
                var bitmap = new BitmapImage();
                await bitmap.SetSourceAsync(await pictureReference.OpenAsync(FileAccessMode.Read));
                enumerableCollection.Add(bitmap);
            }
            await cachedPictures.PutValueAsync(enumerableCollection);
        }

        private async void getButton_Click(object sender, RoutedEventArgs e)
        {
            var enumerableImages = await cachedPictures.GetValueAsync();
            storedFilename.Text = "FilesSuccessfullyObtained.txt";
        }
    }

    public sealed class Pictures
    {
        public string PictureValue;
    }
}
