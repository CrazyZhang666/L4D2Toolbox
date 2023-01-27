using L4D2Toolbox.Data;
using L4D2Toolbox.Steam;
using L4D2Toolbox.Utils;
using L4D2Toolbox.Helper;
using L4D2Toolbox.Windows;
using Steamworks;

namespace L4D2Toolbox.Views;

/// <summary>
/// SubscribeView.xaml 的交互逻辑
/// </summary>
public partial class SubscribeView : UserControl
{
    /// <summary>
    /// 玩家创意工坊订阅列表动态集合
    /// </summary>
    public ObservableCollection<ItemInfo> ItemInfoLists { get; set; } = new();

    public SubscribeView()
    {
        InitializeComponent();
        this.DataContext = this;
        MainWindow.WindowClosingEvent += MainWindow_WindowClosingEvent;
    }

    /// <summary>
    /// 主窗口关闭事件
    /// </summary>
    private void MainWindow_WindowClosingEvent()
    {

    }

    /// <summary>
    /// 超链接请求导航事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        ProcessUtil.OpenLink(e.Uri.OriginalString);
        e.Handled = true;
    }

    /// <summary>
    /// 刷新订阅列表
    /// </summary>
    private async void Button_RefushSubscribeList_Click(object sender, RoutedEventArgs e)
    {
        Button_RefushSubscribeList.IsEnabled = false;
        NotifierHelper.Show(NotifierType.Notification, "正在刷新玩家求生之路2订阅列表...");

        ItemInfoLists.Clear();

        var itemInfos = await Workshop.GetWorkshopItemList(false);
        itemInfos.ForEach(info =>
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
            {
                ItemInfoLists.Add(info);
            });
        });

        Button_RefushSubscribeList.IsEnabled = true;
        NotifierHelper.Show(NotifierType.Success, "刷新玩家求生之路2订阅列表成功");
    }

    /// <summary>
    /// 取消订阅
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Button_UnSubscribeItem_Click(object sender, RoutedEventArgs e)
    {
        if (ListBox_Workshops.SelectedItem is ItemInfo info)
        {
            if (MessageBox.Show($"您确定要取消订阅这件物品吗？\n\n标题：{info.Title}\n物品ID：{info.Id}",
                "取消订阅", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                if (await Workshop.UnSubscribeItem(info.Id))
                {
                    ItemInfoLists.Remove(info);
                    NotifierHelper.Show(NotifierType.Success, $"物品ID {info.Id} 取消订阅成功");
                }
                else
                {
                    NotifierHelper.Show(NotifierType.Error, $"物品ID {info.Id} 取消订阅失败");
                }
            }
        }
    }
}
