﻿using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace AquaMai.Utils;

public class WindowState
{
    private const int GWL_STYLE = -16;
    private const int WS_WHATEVER  = 0x14CF0000;

    private static IntPtr hwnd = IntPtr.Zero;

    public static void Execute()
    {
        if (AquaMai.AppConfig.Utils.Windowed)
        {
            var alreadyWindowed = Screen.fullScreenMode == FullScreenMode.Windowed;
            if (AquaMai.AppConfig.Utils.Width == 0 || AquaMai.AppConfig.Utils.Height == 0)
            {
                Screen.fullScreenMode = FullScreenMode.Windowed;
            }
            else
            {
                alreadyWindowed = false;
                Screen.SetResolution(AquaMai.AppConfig.Utils.Width, AquaMai.AppConfig.Utils.Height, FullScreenMode.Windowed);
            }

            hwnd = GetWindowHandle();
            if(alreadyWindowed)
            {
                SetResizeable();
            }
            else
            {
                Task.Run(async () =>
                {
                    await Task.Delay(3000);
                    // Screen.SetResolution has delay
                    SetResizeable();
                });
            }
        }
        else
        {
            var width = AquaMai.AppConfig.Utils.Width == 0 ? Display.main.systemWidth : AquaMai.AppConfig.Utils.Width;
            var height = AquaMai.AppConfig.Utils.Height == 0 ? Display.main.systemHeight : AquaMai.AppConfig.Utils.Height;
            Screen.SetResolution(width, height, FullScreenMode.FullScreenWindow);
        }
    }

    public static void SetResizeable()
    {
        if (hwnd == IntPtr.Zero) return;
        SetWindowLongPtr(hwnd, GWL_STYLE, WS_WHATEVER);
    }

    private delegate bool EnumThreadDelegate(IntPtr hwnd, IntPtr lParam);

    [DllImport("user32.dll")]
    static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

    [DllImport("Kernel32.dll")]
    static extern int GetCurrentThreadId();

    static IntPtr GetWindowHandle()
    {
        IntPtr returnHwnd = IntPtr.Zero;
        var threadId = GetCurrentThreadId();
        EnumThreadWindows(threadId,
            (hWnd, lParam) =>
            {
                if (returnHwnd == IntPtr.Zero) returnHwnd = hWnd;
                return true;
            }, IntPtr.Zero);
        return returnHwnd;
    }

    [DllImport("user32.dll")]
    static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, int dwNewLong);
}