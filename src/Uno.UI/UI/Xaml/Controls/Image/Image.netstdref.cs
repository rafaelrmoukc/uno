using System;
using System.IO;
using Windows.Foundation;
using Microsoft.UI.Xaml.Media;
using Uno.Extensions;
using Uno.Foundation;
using Uno.Foundation.Logging;
using Microsoft.UI.Xaml.Media.Imaging;
using Uno.Disposables;
using Windows.Storage.Streams;
using System.Runtime.InteropServices;

using Windows.UI;

namespace Microsoft.UI.Xaml.Controls
{
	partial class Image : FrameworkElement
	{
#pragma warning disable 67
		public event RoutedEventHandler ImageOpened;
		public event ExceptionRoutedEventHandler ImageFailed;
#pragma warning restore 67
	}
}
