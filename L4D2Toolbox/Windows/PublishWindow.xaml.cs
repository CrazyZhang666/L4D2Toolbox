using L4D2Toolbox.Data;
using L4D2Toolbox.Utils;

using Steamworks;
using Steamworks.Ugc;
using Steamworks.Data;
using L4D2Toolbox.Helper;

namespace L4D2Toolbox.Windows;

/// <summary>
/// PublishWindow.xaml 的交互逻辑
/// </summary>
public partial class PublishWindow
{
    /// <summary>
    /// 是否为发布Mod，否则为更新Mod
    /// </summary>
    private bool IsPublish = true;

    private readonly ItemInfo OriginItemInfo;

    /// <summary>
    /// 标签 CheckBox 集合
    /// </summary>
    private readonly List<CheckBox> CheckBoxList = new();

    private const string AreaName = "PublishWindowArea";

    //////////////////////////////////////////////////

    public PublishWindow(ItemInfo itemInfo, bool isPublish)
    {
        InitializeComponent();
        this.DataContext = this;

        OriginItemInfo = itemInfo;
        // 发布更新Mod标志
        IsPublish = isPublish;

        // 获取CheckBox控件集合
        foreach (var childSP in MiscUtil.GetChildObject(Tags_CheckBoxGroup))
        {
            foreach (var child in MiscUtil.GetChildObject(childSP))
            {
                if (child is CheckBox)
                    CheckBoxList.Add(child as CheckBox);
            }
        }

        // 更新Mod 填充信息
        if (!isPublish)
        {
            // 标题
            TextBox_Title.Text = itemInfo.Title;

            // 预览图
            Button_PreviewImage.Image = itemInfo.PreviewImage;

            // 文件ID
            Hyperlink_FileId.Inlines.Add($"文件ID：{itemInfo.Id}");
            Hyperlink_FileId.NavigateUri = new Uri(itemInfo.Url);

            // 描述
            TextBox_Description.Text = itemInfo.Description;

            // 可见性
            if (itemInfo.IsPublic)
                RadioButton_IsPublic.IsChecked = true;
            else if (itemInfo.IsFriendsOnly)
                RadioButton_IsFriendsOnly.IsChecked = true;
            else if (itemInfo.IsPrivate)
                RadioButton_IsPrivate.IsChecked = true;
            else if (itemInfo.IsUnlisted)
                RadioButton_IsUnlisted.IsChecked = true;

            // 标签
            foreach (var checkBox in CheckBoxList)
            {
                foreach (var tag in itemInfo.Tags)
                {
                    if (checkBox.Content as string == tag)
                    {
                        checkBox.IsChecked = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 窗口加载事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_Publish_Loaded(object sender, RoutedEventArgs e)
    {
        if (IsPublish)
        {
            Title = "求生之路2 发布创意工坊";
            Button_PublishMod.Content = "发布Mod";
            ChangeLog_Visibility.Visibility = Visibility.Collapsed;

            RadioButton_IsPublic.IsChecked = true;
        }
        else
        {
            Title = "求生之路2 更新Mod信息";
            Button_PublishMod.Content = "更新Mod";
            ChangeLog_Visibility.Visibility = Visibility.Visible;
        }
    }

    /// <summary>
    /// 窗口关闭事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_Publish_Closing(object sender, CancelEventArgs e)
    {
        ProcessUtil.ClearMemory();
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
    /// 报告进度
    /// </summary>
    /// <param name="progress"></param>
    private void ReportProgress(double progress)
    {
        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
        {
            this.ProgressBar_Publish.Value = progress;
        });
    }

    /////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 文件拖放
    /// </summary>
    /// <param name="e"></param>
    /// <param name="fileExte"></param>
    /// <returns></returns>
    private string DropHelper(DragEventArgs e, string[] fileExte)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            var fileNames = e.Data.GetData(DataFormats.FileDrop) as string[];

            if (fileExte.Contains(Path.GetExtension(fileNames[0]).ToLower()))
                return fileNames[0];
            else
                NotifierHelper.Show(NotifierType.Warning, $"当前拖放的目标文件非 {string.Join(", ", fileExte)} 格式，操作取消", AreaName);
        }

        return string.Empty;
    }

    /// <summary>
    /// 选择VPK文件
    /// </summary>
    private void SelectVPKFile()
    {
        var fileDialog = new OpenFileDialog
        {
            Title = "选择要上传的VPK文件",
            RestoreDirectory = true,
            DefaultExt = ".vpk",
            Filter = "VPK文件 (*.vpk)|*.vpk",
            ValidateNames = true,
            AddExtension = true,
            CheckFileExists = false,
            Multiselect = false
        };

        if (fileDialog.ShowDialog() == true)
        {
            TextBox_VPKPath.Text = fileDialog.FileName;
            GetFileSize();
        }
    }

    private void GetFileSize()
    {
        var path = TextBox_VPKPath.Text.Trim();
        if (File.Exists(path))
        {
            TextBlock_FileSize.Text = $"文件大小：{MiscUtil.ByteConverterMB((int)new FileInfo(path).Length)}";
        }
    }

    private void Button_SelectVPK_Click(object sender, RoutedEventArgs e)
    {
        SelectVPKFile();
    }

    private void TextBox_VPKPath_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        SelectVPKFile();
    }

    private void TextBox_VPKPath_PreviewDragOver(object sender, DragEventArgs e)
    {
        e.Effects = DragDropEffects.Copy;
        e.Handled = true;
    }

    private void TextBox_VPKPath_Drop(object sender, DragEventArgs e)
    {
        var file = DropHelper(e, new string[] { ".vpk" });
        if (!string.IsNullOrEmpty(file))
        {
            TextBox_VPKPath.Text = file;
            GetFileSize();
        }
    }

    private void Button_PreviewImage_Click(object sender, RoutedEventArgs e)
    {
        var fileDialog = new OpenFileDialog
        {
            Title = "选择要上传Mod预览图",
            RestoreDirectory = true,
            Filter = "图片文件 (*.png;*.jpg;*.bmp)|*.png;*.jpg;*.bmp",
            ValidateNames = true,
            AddExtension = true,
            CheckFileExists = false,
            Multiselect = false
        };

        if (fileDialog.ShowDialog() == true)
        {
            Button_PreviewImage.Image = fileDialog.FileName;
        }
    }

    private void Button_PreviewImage_Drop(object sender, DragEventArgs e)
    {
        var file = DropHelper(e, new string[] { ".png", ".jpg", ".bmp" });
        if (!string.IsNullOrEmpty(file))
            Button_PreviewImage.Image = file;
    }

    /////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 发布/更新Mod按钮点击事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Button_PublishMod_Click(object sender, RoutedEventArgs e)
    {
        Button_PublishMod.IsEnabled = false;

        if (IsPublish)
            await PublishMod();
        else
            await UpdateMod();

        Button_PublishMod.IsEnabled = true;
    }

    /// <summary>
    /// 发布L4D2创意工坊
    /// </summary>
    private async Task PublishMod()
    {
        ReportProgress(0.1);

        // 标题
        if (string.IsNullOrWhiteSpace(TextBox_Title.Text))
        {
            NotifierHelper.Show(NotifierType.Warning, "Mod标题不能为空，操作取消", AreaName);
            ReportProgress(0.0);
            return;
        }
        var title = TextBox_Title.Text.Trim();

        // 预览图绝对路径
        if (string.IsNullOrEmpty(Button_PreviewImage.Image))
        {
            NotifierHelper.Show(NotifierType.Warning, "Mod预览图不能为空，操作取消", AreaName);
            ReportProgress(0.0);
            return;
        }
        var imgPath = Button_PreviewImage.Image.Trim();
        if (!File.Exists(imgPath))
        {
            NotifierHelper.Show(NotifierType.Warning, "Mod预览图文件为空或不存在，操作取消", AreaName);
            ReportProgress(0.0);
            return;
        }
        // 预览图名称
        var imgName = Path.GetFileName(imgPath);

        // VPK文件绝对路径
        var vpkPath = TextBox_VPKPath.Text.Trim();
        if (!File.Exists(vpkPath))
        {
            NotifierHelper.Show(NotifierType.Warning, "Mod主体VPK文件不能为空或不存在，操作取消", AreaName);
            ReportProgress(0.0);
            return;
        }
        // VPK文件名称
        var vpkName = Path.GetFileName(vpkPath);

        // Mod描述
        var description = TextBox_Description.Text.Trim();

        ReportProgress(0.2);

        // 可见性
        var visibility = new RemoteStoragePublishedFileVisibility();
        if (RadioButton_IsPublic.IsChecked == true)
            visibility = RemoteStoragePublishedFileVisibility.Public;
        else if (RadioButton_IsFriendsOnly.IsChecked == true)
            visibility = RemoteStoragePublishedFileVisibility.FriendsOnly;
        else if (RadioButton_IsPrivate.IsChecked == true)
            visibility = RemoteStoragePublishedFileVisibility.Private;
        else if (RadioButton_IsUnlisted.IsChecked == true)
            visibility = RemoteStoragePublishedFileVisibility.Unlisted;

        // 标签
        var tags = new List<string> { };
        foreach (var checkBox in CheckBoxList)
        {
            if (checkBox.IsChecked == true)
            {
                tags.Add(checkBox.Content as string);
            }
        }
        ReportProgress(0.3);

        // 预览图二进制文件
        var imgData = await File.ReadAllBytesAsync(imgPath);
        ReportProgress(0.4);

        // 上传预览图文件到Steam云存储
        if (!SteamRemoteStorage.FileWrite(imgName, imgData))
        {
            NotifierHelper.Show(NotifierType.Error, "上传预览图文件到Steam云存储失败，操作取消", AreaName);
            ReportProgress(0.0);
            return;
        }
        ReportProgress(0.5);

        // VPK二进制文件
        var vpkData = await File.ReadAllBytesAsync(vpkPath);
        ReportProgress(0.6);

        // 上传VPK文件到Steam云存储
        if (!SteamRemoteStorage.FileWrite(vpkName, vpkData))
        {
            NotifierHelper.Show(NotifierType.Error, "上传VPK文件到Steam云存储失败，操作取消", AreaName);
            ReportProgress(0.0);
            return;
        }
        ReportProgress(0.8);

        // 从Steam云存储发布
        var result_t = await SteamRemoteStorage.PublishWorkshopFile(vpkName, imgName, title, description, visibility, tags);
        if (result_t.Value.Result == Result.OK)
        {
            ReportProgress(1.0);
            NotifierHelper.Show(NotifierType.Success, $"发布L4D2创意工坊成功，物品Id：{result_t.Value.PublishedFileId.Value}", AreaName);
        }
        else
        {
            NotifierHelper.Show(NotifierType.Error, $"发布L4D2创意工坊失败 {result_t.Value.Result}，操作取消", AreaName);
            ReportProgress(0.0);
        }

        SteamRemoteStorage.FileDelete(imgName);
        await Task.Delay(200);
        SteamRemoteStorage.FileDelete(vpkName);
    }

    /// <summary>
    /// 更新选中Mod信息
    /// </summary>
    private async Task UpdateMod()
    {
        ReportProgress(0.1);

        // 创建更新Mod请求
        var editor = new Editor(new PublishedFileId
        {
            Value = OriginItemInfo.Id
        });

        // 标题
        var title = TextBox_Title.Text.Trim();
        if (title != OriginItemInfo.Title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                NotifierHelper.Show(NotifierType.Warning, "Mod标题不能为空，操作取消", AreaName);
                ReportProgress(0.0);
                return;
            }
            editor.WithTitle(title);
        }

        // 预览图
        var imgPath = Button_PreviewImage.Image.Trim();
        if (imgPath != OriginItemInfo.PreviewImage)
        {
            if (!File.Exists(imgPath))
            {
                NotifierHelper.Show(NotifierType.Warning, "Mod预览图文件为空或不存在，操作取消", AreaName);
                ReportProgress(0.0);
                return;
            }
            editor.WithPreviewFile(imgPath);
        }

        ReportProgress(0.2);

        // 描述
        var description = TextBox_Description.Text.Trim();
        if (description != OriginItemInfo.Description)
            editor.WithDescription(description);

        // 可见性
        if (RadioButton_IsPublic.IsChecked == true)
            editor.WithPublicVisibility();
        else if (RadioButton_IsFriendsOnly.IsChecked == true)
            editor.WithFriendsOnlyVisibility();
        else if (RadioButton_IsPrivate.IsChecked == true)
            editor.WithPrivateVisibility();
        else if (RadioButton_IsUnlisted.IsChecked == true)
            editor.WithUnlistedVisibility();

        // 标签
        foreach (var checkBox in CheckBoxList)
        {
            if (checkBox.IsChecked == true)
            {
                editor.WithTag(checkBox.Content as string);
            }
        }

        ReportProgress(0.3);

        // 提交更新请求
        var result = await editor.SubmitAsync();
        if (!result.Success)
        {
            NotifierHelper.Show(NotifierType.Error, $"更新选中Mod信息失败 {result.Result}，操作取消", AreaName);
            ReportProgress(0.0);
            return;
        }

        ReportProgress(0.5);

        // VPK文件绝对路径
        var vpkPath = TextBox_VPKPath.Text.Trim();
        if (string.IsNullOrWhiteSpace(vpkPath))
        {
            NotifierHelper.Show(NotifierType.Success, $"更新选中Mod信息成功 {result.Result}", AreaName);
            ReportProgress(1.0);
            return;
        }

        // 玩家更新了VPK文件
        if (!File.Exists(vpkPath))
        {
            NotifierHelper.Show(NotifierType.Warning, "Mod主体VPK文件不能为空或不存在，操作取消", AreaName);
            ReportProgress(0.0);
            return;
        }

        // VPK文件名称
        var vpkName = Path.GetFileName(vpkPath);
        // VPK二进制文件
        var vpkData = await File.ReadAllBytesAsync(vpkPath);

        ReportProgress(0.6);

        // 上传VPK文件到Steam云存储
        if (!SteamRemoteStorage.FileWrite(vpkName, vpkData))
        {
            NotifierHelper.Show(NotifierType.Error, "上传VPK文件到Steam云存储失败，操作取消", AreaName);
            ReportProgress(0.0);
            return;
        }

        ReportProgress(0.8);

        var fileId = new PublishedFileId
        {
            Value = OriginItemInfo.Id
        };
        var changeLog = TextBox_ChangeLog.Text.Trim();
        // 更新VPK文件和分部分项日志
        if (!SteamRemoteStorage.UpdatePublished(fileId, changeLog, vpkName))
        {
            NotifierHelper.Show(NotifierType.Error, "更新已发布物品VPK文件失败，操作取消", AreaName);
            ReportProgress(0.0);
            return;
        }

        ReportProgress(1.0);
        if (MessageBox.Show("更新已发布物品VPK文件成功，现在关闭窗口吗？", "更新成功",
            MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
        {
            this.Close();
        }
    }
}
