using L4D2Toolbox.Data;
using L4D2Toolbox.Steam;

namespace L4D2Toolbox;

/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public partial class MainWindow
{
    /// <summary>
    /// 导航菜单
    /// </summary>
    public List<NavMenu> NavMenus { get; set; } = new();
    /// <summary>
    /// 导航字典
    /// </summary>
    private readonly Dictionary<string, UserControl> NavDictionary = new();

    /// 主窗口关闭委托
    /// </summary>
    public delegate void WindowClosingDelegate();
    /// <summary>
    /// 主窗口关闭事件
    /// </summary>
    public static event WindowClosingDelegate WindowClosingEvent;

    ///////////////////////////////////////////////////////

    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = this;

        CreateNavMenu();
        Navigate(NavDictionary.First().Key);
    }

    private void Window_Main_Loaded(object sender, RoutedEventArgs e)
    {

    }

    private void Window_Main_Closing(object sender, CancelEventArgs e)
    {
        WindowClosingEvent();
        Workshop.ShutDown();
    }

    /// <summary>
    /// 创建导航菜单
    /// </summary>
    private void CreateNavMenu()
    {
        NavMenus.Add(new() { Icon = "\xe61f", Title = "首页", ViewName = "HomeView" });
        NavMenus.Add(new() { Icon = "\xe899", Title = "管理创意工坊", ViewName = "WorkshopView" });
        NavMenus.Add(new() { Icon = "\xe6a4", Title = "发布新MOD", ViewName = "PublishView" });
        NavMenus.Add(new() { Icon = "\xe77e", Title = "Steam云存储", ViewName = "StorageView" });
        NavMenus.Add(new() { Icon = "\xe704", Title = "自定中文字体", ViewName = "GameFontView" });
        NavMenus.Add(new() { Icon = "\xe606", Title = "常用工具", ViewName = "ToolkitView" });
        NavMenus.Add(new() { Icon = "\xe603", Title = "关于", ViewName = "AboutView" });

        NavMenus.ForEach(menu =>
        {
            var type = Type.GetType($"L4D2Toolbox.Views.{menu.ViewName}");
            NavDictionary.Add(menu.ViewName, Activator.CreateInstance(type) as UserControl);
        });
    }

    /// <summary>
    /// 页面导航
    /// </summary>
    /// <param name="menu"></param>
    [RelayCommand]
    private void Navigate(string viewName)
    {
        if (NavDictionary.ContainsKey(viewName))
        {
            ContentControl_NavRegion.Content = NavDictionary[viewName];
        }
    }
}
