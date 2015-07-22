#pragma once
#include <StorageMode.h>

namespace MemoryAware
{
	static public ref class BitmapImage sealed
	{
		static property Windows::Foundation::Collections::IMap<Platform::String^, Windows::UI::Xaml::Media::Imaging::BitmapImage^>^ inMemoryImages;
		static bool CreateHandle(Platform::String^&);
	public:
		BitmapImage();
		static bool Create(StorageMode, Platform::String^&);
		static bool Create(StorageMode, Windows::Foundation::Uri^, Platform::String^&);
		static bool SetSource(StorageMode, Platform::String^, Windows::Foundation::Uri^);
		static Windows::Foundation::IAsyncOperation<bool>^ SetSourceAsync(StorageMode, Platform::String^, Windows::Foundation::Uri^);
	};
}
