#include <collection.h>
#include <ppltasks.h>
#include <BitmapImage.h>
#include <stdio.h>
#include <StorageMode.h>
#include <objbase.h>

#pragma comment(lib,"ole32.lib")

using namespace Concurrency;
using namespace Platform;
using namespace Platform::Collections;
using namespace std;
using namespace Windows::Foundation;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace Windows::Web::Http;


MemoryAware::BitmapImage::BitmapImage()
{
	inMemoryImages = ref new Map<String^, Windows::UI::Xaml::Media::Imaging::BitmapImage^>();
}

bool MemoryAware::BitmapImage::CreateHandle(String^* handle)
{
	GUID nativeGuid;
	if (S_OK != CoCreateGuid(&nativeGuid)) return false;
	Guid uwpGuid(nativeGuid);
	*handle = uwpGuid.ToString() + ".handle";
	return true;
}

IAsyncOperation<bool>^ MemoryAware::BitmapImage::CreateAsync(StorageMode storageMode, String^* handle)
{
	if (!CreateHandle(handle)) return create_async([] { return false; });
	switch (storageMode)
	{
	case StorageMode::Memory:
		inMemoryImages->Insert(*handle, ref new Windows::UI::Xaml::Media::Imaging::BitmapImage());
		break;
	case StorageMode::Disk:
		ApplicationData::Current->TemporaryFolder->CreateFileAsync(*handle);
		break;
	}
	return create_async([] { return true; });
}

IAsyncOperation<bool>^ MemoryAware::BitmapImage::CreateAsync(StorageMode storageMode, Windows::Foundation::Uri^ uri, Platform::String^* handle)
{
	if (!CreateHandle(handle)) return create_async([] { return false; });
	switch (storageMode)
	{
	case StorageMode::Memory:
		inMemoryImages->Insert(*handle, ref new Windows::UI::Xaml::Media::Imaging::BitmapImage(uri));
		break;
	case StorageMode::Disk:
		create_task(ApplicationData::Current->TemporaryFolder->CreateFileAsync(*handle)).then([handle, uri](IStorageFile^ storageFile) 
		{
			return create_task(storageFile->OpenAsync(FileAccessMode::ReadWrite));
		}).then([uri](IRandomAccessStream^ randomAccessStream) 
		{
			return create_task((ref new HttpClient())->GetAsync(uri)).then([&randomAccessStream](HttpResponseMessage^ response) 
			{
				return create_task(response->Content->WriteToStreamAsync(randomAccessStream));
			});
		});
		break;
	}
	return create_async([] { return true; });
}

IAsyncOperation<bool>^ MemoryAware::BitmapImage::SetSourceAsync(StorageMode storageMode, Platform::String^ handle, Windows::Foundation::Uri^ uri)
{
	switch (storageMode)
	{
	case StorageMode::Memory:
		return create_async([handle, uri] 
		{
			inMemoryImages->Lookup(handle)->UriSource = uri;
			return true;
		});
		break;
	case StorageMode::Disk:
		return create_async([handle, uri] 
		{
			create_task(ApplicationData::Current->TemporaryFolder->CreateFileAsync(handle)).then([handle, uri](IStorageFile^ storageFile)
			{
				return create_task(storageFile->OpenAsync(FileAccessMode::ReadWrite));
			}).then([uri](IRandomAccessStream^ randomAccessStream)
			{
				return create_task((ref new HttpClient())->GetAsync(uri)).then([&randomAccessStream](HttpResponseMessage^ response)
				{
					return create_task(response->Content->WriteToStreamAsync(randomAccessStream));
				});
			});
			return true;
		});
		break;
	default:
		return create_async([] {return false; });
		break;
	}
}

IAsyncAction^ MemoryAware::BitmapImage::RemoveAsync(String^ handle)
{
	return create_async([handle] 
	{
		inMemoryImages->Remove(handle);
		create_task(ApplicationData::Current->TemporaryFolder->GetFileAsync(handle))
			.then([](StorageFile^ file)
		{
			file->DeleteAsync();
		});
	});
	
}