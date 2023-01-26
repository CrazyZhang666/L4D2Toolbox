using L4D2Toolbox.Data;
using L4D2Toolbox.Utils;
using L4D2Toolbox.Helper;

using Steamworks;

namespace L4D2Toolbox.Views;

/// <summary>
/// StorageView.xaml 的交互逻辑
/// </summary>
public partial class StorageView : UserControl
{
    /// <summary>
    /// 玩家Steam云存储文件列表动态集合
    /// </summary>
    public ObservableCollection<StorageInfo> StorageInfoLists { get; set; } = new();

    public StorageView()
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
    /// 刷新列表
    /// </summary>
    private void RefreshList()
    {
        Button_RefreshStorageList.IsEnabled = false;
        NotifierHelper.Show(NotifierType.Notification, "正在刷新Steam云存储文件列表...");

        int index = 1;
        StorageInfoLists.Clear();

        foreach (var file in SteamRemoteStorage.Files)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Background, () =>
            {
                StorageInfoLists.Add(new()
                {
                    Index = index++,
                    Name = file,
                    Size = MiscUtil.ByteConverterMB(SteamRemoteStorage.FileSize(file)),
                    Date = MiscUtil.FormatDateTime(SteamRemoteStorage.FileTime(file)),
                    Exists = MiscUtil.BoolToFlag(SteamRemoteStorage.FileExists(file)),
                    Persisted = MiscUtil.BoolToFlag(SteamRemoteStorage.FilePersisted(file))
                });
            });
        }

        RefushQuotaInfo();

        Button_RefreshStorageList.IsEnabled = true;
        NotifierHelper.Show(NotifierType.Success, "刷新Steam云存储文件列表成功");
    }

    /// <summary>
    /// 刷新配额信息
    /// </summary>
    private void RefushQuotaInfo()
    {
        var quotaBytes = SteamRemoteStorage.QuotaBytes;
        var quotaUsedBytes = SteamRemoteStorage.QuotaUsedBytes;

        Border_QuotaUse.Width = 1.0 * quotaUsedBytes / quotaBytes * 300;
        TextBlock_QuotaInfo.Text = $"{MiscUtil.ByteConverterMB(quotaUsedBytes)} 已存储 / {MiscUtil.ByteConverterMB(quotaBytes)} 总大小";

        NotifierHelper.Show(NotifierType.Success, "刷新Steam云存储配额信息成功");
    }

    /// <summary>
    /// 刷新列表
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_RefreshStorageList_Click(object sender, RoutedEventArgs e)
    {
        RefreshList();
    }

    /// <summary>
    /// 删除选中项
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_DeleteStorage_Click(object sender, RoutedEventArgs e)
    {
        if (ListBox_Storages.SelectedItem is StorageInfo info)
        {
            if (MessageBox.Show($"您确定要删除这个文件吗？此操作不可撤销！\n\n文件名：{info.Name}\n文件大小：{info.Size}",
                "删除文件", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                if (SteamRemoteStorage.FileDelete(info.Name))
                    NotifierHelper.Show(NotifierType.Success, $"删除文件 {info.Name} 成功");
                else
                    NotifierHelper.Show(NotifierType.Error, $"删除文件 {info.Name} 失败");

                RefreshList();
            }
        }
    }

    /// <summary>
    /// 清空全部文件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_ClearStorage_Click(object sender, RoutedEventArgs e)
    {
        if (MessageBox.Show($"您确定要清空Steam云存储文件吗？此操作不可撤销！",
            "清空Steam云存储文件", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
        {
            foreach (var file in SteamRemoteStorage.Files)
            {
                SteamRemoteStorage.FileDelete(file);
            }

            NotifierHelper.Show(NotifierType.Success, "清空Steam云存储文件成功");
            RefreshList();
        }
    }
}
