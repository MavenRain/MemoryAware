#pragma once
#include <StorageMode.h>

namespace MemoryAware
{
	static public ref class BitmapImage sealed
	{
		static property Windows::Foundation::Collections::IMap<Platform::String^, Windows::UI::Xaml::Media::Imaging::BitmapImage^>^ inMemoryImages;
		static bool CreateHandle(Platform::String^*);
	public:
		BitmapImage();
		static Windows::Foundation::IAsyncOperation<bool>^ CreateAsync(StorageMode, Platform::String^*);
		static Windows::Foundation::IAsyncOperation<bool>^ CreateAsync(StorageMode, Windows::Foundation::Uri^, Platform::String^*);
		static Windows::Foundation::IAsyncOperation<bool>^ SetSourceAsync(StorageMode, Platform::String^, Windows::Foundation::Uri^);
		static Windows::Foundation::IAsyncAction^ RemoveAsync(Platform::String^);
	};
}
