#pragma once

namespace Platform
{
	ref class Object;
	ref class String;
}

namespace Windows
{
	namespace Foundation
	{
		ref class Uri;
	}

	namespace UI
	{
		namespace Xaml
		{
			namespace Media
			{
				namespace Imaging
				{
					ref class DownloadProgressEventArgs;
					delegate void DownloadProgressEventHandler(Platform::Object^, DownloadProgressEventArgs^);
				}
			}
			ref class RoutedEventArgs;
			delegate void RoutedEventHandler(Platform::Object^, RoutedEventArgs^);
			ref class ExceptionRoutedEventArgs;
			delegate void ExceptionRoutedEventHandler(Platform::Object^, ExceptionRoutedEventArgs^);
		}
	}
}

namespace MemoryAware
{
	static public ref class BitmapImage sealed
	{
	public:
		static bool Create(Platform::String^*);
		static bool Create(Windows::Foundation::Uri^, Platform::String^*);
		static event Windows::UI::Xaml::Media::Imaging::DownloadProgressEventHandler^ DownloadProgress;
		static event ExceptionRoutedEventHandler^ ImageFailed;
		static event RoutedEventHandler^ ImageOpened;
	};
}
