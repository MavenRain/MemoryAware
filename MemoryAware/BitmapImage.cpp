#include <collection.h>
#include <ppltasks.h>
#include <BitmapImage.h>
#include <stdio.h>
#include <StorageMode.h>

using namespace std;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Storage;

MemoryAware::BitmapImage::BitmapImage()
{
	inMemoryImages = ref new Map<String^, Windows::UI::Xaml::Media::Imaging::BitmapImage^>();
}

bool MemoryAware::BitmapImage::Create(StorageMode storageMode, String^* handle)
{
	switch (storageMode)
	{
	case StorageMode::Memory:
		break;
	case StorageMode::Disk:
		break;
	}
}