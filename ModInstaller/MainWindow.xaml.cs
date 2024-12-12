using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace ModInstaller;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MainPageModel _viewModel;
    public MainWindow()
    {
        InitializeComponent();

        _viewModel = DataContext as MainPageModel;

        if (!DesignerProperties.GetIsInDesignMode(this))
            _viewModel?.Init();

        if (_viewModel.SilentMode)
        {
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            ShowInTaskbar = false;
            Opacity = 0;
            Loaded += MainWindow_Loaded;
        }
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        HideOnAltTab();
    }

    private void HideOnAltTab()
    {
        if (PresentationSource.FromVisual(this) is HwndSource hwndSource)
        {
            var handle = hwndSource.Handle;
            var exStyle = GetWindowLong(handle, WndAttribute.GWL_EXSTYLE);
            exStyle |= WndExStyle.WS_EX_TOOLWINDOW; // 设置 WS_EX_TOOLWINDOW 标志
            exStyle &= ~WndExStyle.WS_EX_APPWINDOW; // 去除 WS_EX_APPWINDOW 标志
            SetWindowLong(handle, WndAttribute.GWL_EXSTYLE, exStyle);
        }
    }

    private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            // _viewModel?.SearchMod(SearchTermTextBox.Text);
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _ = AppShell.Instance.LoadConfig();
    }

    #region WndExStyle

    public static class WndExStyle
    {
        /*
         * Extended Window Styles
         */
        public const uint WS_EX_DLGMODALFRAME = 0x00000001;
        public const uint WS_EX_NOPARENTNOTIFY = 0x00000004;
        public const uint WS_EX_TOPMOST = 0x00000008;
        public const uint WS_EX_ACCEPTFILES = 0x00000010;
        public const uint WS_EX_TRANSPARENT = 0x00000020;

        public const uint WS_EX_MDICHILD = 0x00000040;
        public const uint WS_EX_TOOLWINDOW = 0x00000080;
        public const uint WS_EX_WINDOWEDGE = 0x00000100;
        public const uint WS_EX_CLIENTEDGE = 0x00000200;
        public const uint WS_EX_CONTEXTHELP = 0x00000400;

        public const uint WS_EX_RIGHT = 0x00001000;
        public const uint WS_EX_LEFT = 0x00000000;
        public const uint WS_EX_RTLREADING = 0x00002000;
        public const uint WS_EX_LTRREADING = 0x00000000;
        public const uint WS_EX_LEFTSCROLLBAR = 0x00004000;
        public const uint WS_EX_RIGHTSCROLLBAR = 0x00000000;

        public const uint WS_EX_CONTROLPARENT = 0x00010000;
        public const uint WS_EX_STATICEDGE = 0x00020000;
        public const uint WS_EX_APPWINDOW = 0x00040000;


        public const uint WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE);
        public const uint WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);


        public const uint WS_EX_LAYERED = 0x00080000;

        public const uint WS_EX_NOINHERITLAYOUT = 0x00100000;// Disable inheritence of mirroring by children

        public const uint WS_EX_NOREDIRECTIONBITMAP = 0x00200000;

        public const uint WS_EX_LAYOUTRTL = 0x00400000; // Right to left mirroring

        public const uint WS_EX_COMPOSITED = 0x02000000;

        public const uint WS_EX_NOACTIVATE = 0x08000000;
    }

    #endregion

    public enum WndAttribute : int
    {
        GWL_EXSTYLE = -20,      //	设定一个新的扩展风格。
        GWL_HINSTANCE = -6,     //  设置一个新的应用程序实例句柄。
        GWL_ID = -12,           //	设置一个新的窗口标识符。
        GWL_STYLE = -16,        //	设定一个新的窗口风格。
        GWL_USERDATA = -21,     //	设置与窗口有关的32位值。每个窗口均有一个由创建该窗口的应用程序使用的32位值。
        GWL_WNDPROC = -4,       //	为窗口设定一个新的处理函数。
        GWL_HWNDPARENT = -8,    //	改变子窗口的父窗口, 应使用SetParent函数。
    }

    [DllImport("user32.DLL")]
    public static extern uint GetWindowLong(IntPtr hWnd, WndAttribute nIndex);

    [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
    public static extern uint SetWindowLong(IntPtr hWnd, WndAttribute nIndex, uint dwNewLong);

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);
}