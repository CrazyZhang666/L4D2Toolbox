using L4D2Toolbox.Data;
using L4D2Toolbox.Steam;
using L4D2Toolbox.Utils;
using L4D2Toolbox.Helper;
using L4D2Toolbox.Windows;
using Steamworks;

namespace L4D2Toolbox.Views;

/// <summary>
/// WorkshopView.xaml 的交互逻辑
/// </summary>
public partial class WorkshopView : UserControl
{
    /// <summary>
    /// 玩家创意工坊物品列表动态集合
    /// </summary>
    public ObservableCollection<ItemInfo> ItemInfoLists { get; set; } = new();

    public WorkshopView()
    {
        InitializeComponent();
        this.DataContext = this;
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
    /// 发布新MOD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_PublishMOD_Click(object sender, RoutedEventArgs e)
    {
        var publishWindow = new PublishWindow(new ItemInfo(), true)
        {
            Owner = MainWindow.MainWindowInstance
        };
        publishWindow.ShowDialog();
    }

    /// <summary>
    /// 刷新MOD列表
    /// </summary>
    private async void Button_RefushMODList_Click(object sender, RoutedEventArgs e)
    {
        ProcessUtil.ClearMemory();

        Button_RefushMODList.IsEnabled = false;
        NotifierHelper.Show(NotifierType.Notification, "正在刷新玩家创意工坊项目列表...");

        ItemInfoLists.Clear();

        var itemInfos = await Workshop.GetWorkshopItemList();
        itemInfos.ForEach(info =>
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
            {
                ItemInfoLists.Add(info);
            });
        });

        Button_RefushMODList.IsEnabled = true;
        NotifierHelper.Show(NotifierType.Success, "刷新玩家创意工坊项目列表成功");
    }

    /// <summary>
    /// 更新选中MOD信息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_UpdateSelectedMOD_Click(object sender, RoutedEventArgs e)
    {
        if (ListBox_Workshops.SelectedItem is ItemInfo info)
        {
            var publishWindow = new PublishWindow(info, false)
            {
                Owner = MainWindow.MainWindowInstance
            };
            publishWindow.ShowDialog();
        }
    }

    /// <summary>
    /// 删除选中MOD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Button_DeleteSelectedMOD_Click(object sender, RoutedEventArgs e)
    {
        if (ListBox_Workshops.SelectedItem is ItemInfo info)
        {
            if (MessageBox.Show($"您确定要删除这件物品吗？此操作不可撤销！\n\n标题：{info.Title}\n物品ID：{info.Id}",
                "删除物品", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                if (await Workshop.DeletePublishedItem(info.Id))
                {
                    ItemInfoLists.Remove(info);
                    NotifierHelper.Show(NotifierType.Success, $"物品ID {info.Id} 删除成功");
                }
                else
                {
                    NotifierHelper.Show(NotifierType.Error, $"物品ID {info.Id} 删除失败");
                }
            }
        }
    }
}
