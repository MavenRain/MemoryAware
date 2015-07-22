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
using namespace Windows::Web::Http;


MemoryAware::BitmapImage::BitmapImage()
{
	inMemoryImages = ref new Map<String^, Windows::UI::Xaml::Media::Imaging::BitmapImage^>();
}

bool MemoryAware::BitmapImage::CreateHandle(String^& handle)
{
	GUID nativeGuid;
	if (S_OK != CoCreateGuid(&nativeGuid)) return false;
	Guid uwpGuid(nativeGuid);
	handle = uwpGuid.ToString() + ".handle";
	return true;
}

bool MemoryAware::BitmapImage::Create(StorageMode storageMode, String^& handle)
{
	if (!CreateHandle(handle)) return false;
	switch (storageMode)
	{
	case StorageMode::Memory:
		inMemoryImages->Insert(handle, ref new Windows::UI::Xaml::Media::Imaging::BitmapImage());
		break;
	case StorageMode::Disk:
		ApplicationData::Current->TemporaryFolder->CreateFileAsync(handle);
		break;
	}
	return true;
}

bool MemoryAware::BitmapImage::Create(StorageMode storageMode, Windows::Foundation::Uri^ uri, Platform::String^& handle)
{
	if (!CreateHandle(handle)) return false;
	switch (storageMode)
	{
	case StorageMode::Memory:
		inMemoryImages->Insert(handle, ref new Windows::UI::Xaml::Media::Imaging::BitmapImage(uri));
		break;
	case StorageMode::Disk:
		ApplicationData::Current->TemporaryFolder->CreateFileAsync(handle);
		auto httpResourceResponse = 
		break;
	}
	return true;
}

bool MemoryAware::BitmapImage::SetSource(StorageMode storageMode, Platform::String^, Windows::Foundation::Uri^)
{
	switch (storageMode)
	{
	case StorageMode::Memory:
		break;
	case StorageMode::Disk:
		break;
	}
	return true;
}

IAsyncOperation<bool>^ MemoryAware::BitmapImage::SetSourceAsync(StorageMode storageMode, Platform::String^, Windows::Foundation::Uri^)
{
	switch (storageMode)
	{
	case StorageMode::Memory:
		break;
	case StorageMode::Disk:
		break;
	}
	return create_async([] {return true; });
}