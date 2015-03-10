using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Core.CSharp;
using Microsoft.WindowsAzure.Storage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace StorageRetrievalSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Required;
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

		private async void storageSample_Click(object sender, RoutedEventArgs e)
		{
			var blobClient =
				(CloudStorageAccount.Parse(
					"DefaultEndpointsProtocol=https;AccountName=mavenrain;AccountKey=kPrWKjlH1Es1UQOWSHhZgDDiYuyC2EcrZkCulepl9uyCEUe7+gVNhmWqOQM6xT+GGcA79mENChT8eF2qrhTYOw==")).CreateCloudBlobClient();
			var resultSegment = await (blobClient.GetContainerReference("mavenrain").ListBlobsSegmentedAsync(null));
			var pictureCount = 0;
			foreach (var blob in resultSegment.Results)
			{
				await blobClient.GetContainerReference("mavenrain").GetBlockBlobReference(blob.Uri.OriginalString).DownloadToByteArrayAsync((await DiskAndMemoryReaderAndWriter.ReadAsync("picture"+ (pictureCount++).ToString())).ToArray(),0);
			}
		}
	}
}
