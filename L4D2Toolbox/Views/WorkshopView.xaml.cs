using L4D2Toolbox.Data;
using L4D2Toolbox.Steam;
using L4D2Toolbox.Utils;

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
    /// 刷新MOD列表
    /// </summary>
    private async void Button_RefushMODList_Click(object sender, RoutedEventArgs e)
    {
        Button_RefushMODList.IsEnabled = false;
        ItemInfoLists.Clear();

        var itemInfos = await Workshop.GetUserPublished();
        if (itemInfos.Count > 0)
        {
            itemInfos.ForEach(info =>
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
                {
                    ItemInfoLists.Add(info);
                });
            });
        }

        Button_RefushMODList.IsEnabled = true;
    }
}
