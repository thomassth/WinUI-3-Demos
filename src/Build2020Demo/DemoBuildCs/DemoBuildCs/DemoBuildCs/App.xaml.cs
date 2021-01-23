﻿
using System;
using System.Runtime.InteropServices;

using WinRT;

using Microsoft.UI.Xaml;
using Windows.ApplicationModel;

namespace DemoBuildCs
{
    public partial class App : Application
    {

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            //This is never called in WInUi 3 in Desktop Preview 3
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            //Get the Window's HWND
            var windowNative = m_window.As<IWindowNative>();
            m_windowHandle = windowNative.WindowHandle;
            m_window.Title = "Folder Inspector (.NET 5 Desktop WinUI 3)";
            m_window.Activate();

            // The Window object doesn't have Width and Height properties in WInUI 3 Desktop yet.
            // To set the Width and Height, you can use the Win32 API SetWindowPos.
            // Note, you should apply the DPI scale factor if you are thinking of dpi instead of pixels.
            //SetWindowSize(m_windowHandle, 800, 600);

            CenterWindow(m_windowHandle, 800, 600);
        }

        private void SetWindowSize(IntPtr hwnd, int width, int height)
        {
            var dpi = PInvoke.User32.GetDpiForWindow(hwnd);
            float scalingFactor = (float)dpi / 96;
            width = (int)(width * scalingFactor);
            height = (int)(height * scalingFactor);

            PInvoke.User32.SetWindowPos(hwnd, PInvoke.User32.SpecialWindowHandles.HWND_TOP,
                                        0, 0, width, height,
                                        PInvoke.User32.SetWindowPosFlags.SWP_NOMOVE);
        }

        private void CenterWindow(IntPtr hnwd, int width, int height)
        {
            IntPtr hwndDesktop = PInvoke.User32.GetDesktopWindow();

            PInvoke.RECT rectParent;
            PInvoke.User32.GetClientRect(hwndDesktop, out rectParent);

            var dpi = PInvoke.User32.GetDpiForWindow(hwndDesktop);
            float scalingFactor = (float)dpi / 96;

            width = (int)(width * scalingFactor);
            height = (int)(height * scalingFactor);

            rectParent.left = (rectParent.right / 2) - (width / 2);
            rectParent.top = (rectParent.bottom / 2) - (height / 2);

            PInvoke.User32.SetWindowPos(hnwd, PInvoke.User32.SpecialWindowHandles.HWND_TOP,
                                        rectParent.left, rectParent.top, width, height,
                                        PInvoke.User32.SetWindowPosFlags.SWP_SHOWWINDOW);

        }


        private Window m_window;
        private IntPtr m_windowHandle;
        public IntPtr WindowHandle { get { return m_windowHandle; } }

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
        internal interface IWindowNative
        {
            IntPtr WindowHandle { get; }
        }
    }
}
