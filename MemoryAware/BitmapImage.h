#pragma once

namespace MemoryAware
{
	enum class StorageMode;
}

namespace MemoryAware
{
	static public ref class BitmapImage sealed
	{
		static Platform::Collections::Map<String^, Windows::UI::Xaml::Media::Imaging::BitmapImage^>^ inMemoryImages;
	public:
		BitmapImage();
		static bool Create(StorageMode, Platform::String^*);
		static bool Create(StorageMode, Windows::Foundation::Uri^, Platform::String^*);
		static bool SetSource(Platform::String^, Windows::Foundation::Uri^);
		static Windows::Foundation::IAsyncOperation<bool>^ SetSourceAsync(Platform::String^, Windows::Foundation::Uri^);
	};
}
