#nullable enable

using System;
using System.IO;
using SkiaSharp;
using Uno.Extensions;
using Uno.UI.Xaml.Core;
using Windows.UI.Xaml.Input;
using WUX = Windows.UI.Xaml;
using Uno.Foundation.Logging;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Uno.UI.Runtime.Skia.Helpers.Windows;
using Uno.UI.Runtime.Skia.Helpers.Dpi;
using Windows.Graphics.Display;
using Gdk;
using System.Reflection;
using Gtk;
using Silk.NET.OpenGLES;
using Silk.NET.Core.Loader;

namespace Uno.UI.Runtime.Skia
{

	internal class OpenGLESRenderSurface : GLRenderSurfaceBase
	{
		private GL _gl;

		public OpenGLESRenderSurface()
		{
			_gl = new GL(new Silk.NET.Core.Contexts.DefaultNativeContext(new GLCoreLibraryNameContainer().GetLibraryName()));
		}

		public static bool IsSupported
		{
			get
			{
				// OpenGL support on macOS is currently broken
				var isMacOS = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

				// WSL2 is not supported because of a low version for GLSL (https://github.com/unoplatform/uno/issues/8643#issuecomment-1114392827)
				var isWSL2 = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
					// https://github.com/microsoft/WSL/issues/423#issuecomment-844418910
					&& File.Exists("/proc/sys/kernel/osrelease")
					&& File.ReadAllText("/proc/sys/kernel/osrelease").Trim().EndsWith("WSL2");

				return !isMacOS && !isWSL2;
			}
		}

		protected override (int, int, int) GetGLBuffers()
		{
			_gl.GetInteger(GLEnum.FramebufferBinding, out var framebuffer);
			_gl.GetInteger(GLEnum.Stencil, out var stencil);
			_gl.GetInteger(GLEnum.Samples, out var samples);

			return (framebuffer, stencil, samples);
		}

		protected override void GLClear()
		{
			_gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.StencilBufferBit | ClearBufferMask.DepthBufferBit);
			_gl.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
		}

		protected override void GLFlush()
			=> _gl.Flush();

		protected override GRContext TryBuildGRContext()
		{
			var glInterface = GRGlInterface.CreateGles(proc =>
			{
				if (_gl.Context.TryGetProcAddress(proc, out var addr))
				{
					return addr;
				}

				return IntPtr.Zero;
			});

			return GRContext.CreateGl(glInterface);
		}

		// Extracted from https://github.com/dotnet/Silk.NET/blob/23f9bd4d67ad21c69fbd69cc38a62fb2c0ec3927/src/OpenGL/Silk.NET.OpenGL/GLCoreLibraryNameContainer.cs
		internal class GLCoreLibraryNameContainer : SearchPathContainer
		{
			/// <inheritdoc />
			public override string Linux => "libGL.so.1";

			/// <inheritdoc />
			public override string MacOS => "/System/Library/Frameworks/OpenGL.framework/OpenGL";

			/// <inheritdoc />
			public override string Android => "libGL.so.1";

			/// <inheritdoc />
			public override string IOS => "/System/Library/Frameworks/OpenGL.framework/OpenGL";

			/// <inheritdoc />
			public override string Windows64 => "opengl32.dll";

			/// <inheritdoc />
			public override string Windows86 => "opengl32.dll";
		}

		// https://github.com/dotnet/Silk.NET/blob/23f9bd4d67ad21c69fbd69cc38a62fb2c0ec3927/src/OpenGL/Silk.NET.OpenGLES/OpenGLESLibraryNameContainer.cs
		internal class OpenGLESLibraryNameContainer : SearchPathContainer
		{
			public override string Linux => "libGLESv2.so";

			public override string MacOS => "/System/Library/Frameworks/OpenGLES.framework/OpenGLES";

			public override string Android => "libGLESv2.so";

			public override string IOS => "/System/Library/Frameworks/OpenGLES.framework/OpenGLES";

			public override string Windows64 => "libGLESv2.dll";

			public override string Windows86 => "libGLESv2.dll";
		}
	}
}
