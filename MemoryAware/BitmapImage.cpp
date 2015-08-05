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
	auto handleIsCreated = CreateHandle(handle);
	return create_async([storageMode, handleIsCreated, &handle]
	{
		if (!handleIsCreated) return false;
		switch (storageMode)
		{
		case StorageMode::Memory:
			inMemoryImages->Insert(*handle, ref new Windows::UI::Xaml::Media::Imaging::BitmapImage());
			break;
		case StorageMode::Disk:
			ApplicationData::Current->TemporaryFolder->CreateFileAsync(*handle);
			break;
		}
		return true;
	});
}

IAsyncOperation<bool>^ MemoryAware::BitmapImage::CreateAsync(StorageMode storageMode, Windows::Foundation::Uri^ uri, Platform::String^* handle)
{
	auto handleIsCreated = CreateHandle(handle);
	return create_async([storageMode, handleIsCreated, &handle, uri]
	{
		if (!handleIsCreated) return false;
		switch (storageMode)
		{
		case StorageMode::Memory:
			inMemoryImages->Insert(*handle, ref new Windows::UI::Xaml::Media::Imaging::BitmapImage(uri));
			break;
		case StorageMode::Disk:
			auto randomAccessStream = ApplicationData::Current->TemporaryFolder->CreateFileAsync(*handle)->GetResults()->OpenAsync(FileAccessMode::ReadWrite)->GetResults();
			(ref new HttpClient())->GetAsync(uri)->GetResults()->Content->WriteToStreamAsync(randomAccessStream)->GetResults();
			break;
		}
		return true;
	});
}

IAsyncAction^ MemoryAware::BitmapImage::SetSourceAsync(StorageMode storageMode, Platform::String^ handle, Windows::Foundation::Uri^ uri)
{
	return create_async([storageMode, handle, uri] 
	{
		switch (storageMode)
		{
		case StorageMode::Memory:
			inMemoryImages->Lookup(handle)->UriSource = uri;
			break;
		case StorageMode::Disk:
			auto randomAccessStream = ApplicationData::Current->TemporaryFolder->CreateFileAsync(handle)->GetResults()->OpenAsync(FileAccessMode::ReadWrite)->GetResults();
			(ref new HttpClient())->GetAsync(uri)->GetResults()->Content->WriteToStreamAsync(randomAccessStream)->GetResults();
			break;
		}
	});
}

IAsyncAction^ MemoryAware::BitmapImage::RemoveAsync(String^ handle)
{
	return create_async([handle] 
	{
		inMemoryImages->Remove(handle);
		ApplicationData::Current->TemporaryFolder->GetFileAsync(handle)->GetResults()->DeleteAsync();
	});
	
}