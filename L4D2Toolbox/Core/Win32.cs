namespace L4D2Toolbox.Core;

public static class Win32
{
    [DllImport("user32.dll")]
    private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    private static extern int SetForegroundWindow(IntPtr hwnd);

    [DllImport("user32.dll")]
    private static extern IntPtr GetWindow(IntPtr hWnd, GetWindowType uCmd);

    [DllImport("user32.dll")]
    private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

    [DllImport("user32.dll")]
    private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

    private enum GetWindowType : uint
    {
        GW_HWNDFIRST = 0,
        GW_HWNDLAST = 1,
        GW_HWNDNEXT = 2,
        GW_HWNDPREV = 3,
        GW_OWNER = 4,
        GW_CHILD = 5,
        GW_ENABLEDPOPUP = 6
    }

    private const int CB_SHOWDROPDOWN = 0x014F;
    private const int CB_SETCURSEL = 0x014E;
    private const int WM_SETFOCUS = 0x07;

    /// <summary>
    /// 设置区域MFC窗口Combobox控件选中索引
    /// </summary>
    /// <param name="index">0，501</param>
    public static void SetComboboxSelectIndex(int index)
    {
        var regionHandle = FindWindow(null, "区域");
        if (regionHandle != IntPtr.Zero)
        {
            SetForegroundWindow(regionHandle);

            var childHandle = GetWindow(regionHandle, GetWindowType.GW_CHILD);
            var comboboxHandle = FindWindowEx(childHandle, IntPtr.Zero, "ComboBox", null);

            SendMessage(comboboxHandle, CB_SHOWDROPDOWN, 1, 0);
            SendMessage(comboboxHandle, CB_SETCURSEL, index, 0);
            SendMessage(comboboxHandle, WM_SETFOCUS, 0, 0);
        }
    }
}
