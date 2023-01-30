using L4D2Toolbox.Data;
using L4D2Toolbox.Utils;
using L4D2Toolbox.Helper;

using Steamworks;
using Steamworks.Ugc;
using Steamworks.Data;
using System.Windows;

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

    /// <summary>
    /// 保存更新MOD的原始信息
    /// </summary>
    private ItemInfo OriginItemInfo { get; }

    /// <summary>
    /// 标签 CheckBox 集合
    /// </summary>
    private readonly List<CheckBox> CheckBoxList = new();

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
                NotifierHelper.Show(NotifierType.Warning, $"当前拖放的目标文件非 {string.Join(", ", fileExte)} 格式，操作终止");
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
            PublishMod();
        else
            UpdateMod();

        Button_PublishMod.IsEnabled = true;
    }

    /// <summary>
    /// 发布L4D2创意工坊
    /// </summary>
    private void PublishMod()
    {
        // 标题
        if (string.IsNullOrWhiteSpace(TextBox_Title.Text))
        {
            NotifierHelper.Show(NotifierType.Warning, "Mod标题不能为空，操作终止");
            return;
        }
        var title = TextBox_Title.Text.Trim();

        // 预览图绝对路径
        if (string.IsNullOrEmpty(Button_PreviewImage.Image))
        {
            NotifierHelper.Show(NotifierType.Warning, "Mod预览图不能为空，操作终止");
            return;
        }
        var imgPath = Button_PreviewImage.Image.Trim();
        if (!File.Exists(imgPath))
        {
            NotifierHelper.Show(NotifierType.Warning, "Mod预览图文件为空或不存在，操作终止");
            return;
        }

        // VPK文件绝对路径
        var vpkPath = TextBox_VPKPath.Text.Trim();
        if (!File.Exists(vpkPath))
        {
            NotifierHelper.Show(NotifierType.Warning, "Mod主体VPK文件不能为空或不存在，操作终止");
            return;
        }

        // Mod描述
        var description = TextBox_Description.Text.Trim();

        // 可见性
        var visibility = 0;
        if (RadioButton_IsPublic.IsChecked == true)
            visibility = 0;
        else if (RadioButton_IsFriendsOnly.IsChecked == true)
            visibility = 1;
        else if (RadioButton_IsPrivate.IsChecked == true)
            visibility = 2;
        else if (RadioButton_IsUnlisted.IsChecked == true)
            visibility = 3;

        // 标签
        var tags = new List<string>();
        foreach (var checkBox in CheckBoxList)
        {
            if (checkBox.IsChecked == true)
            {
                tags.Add(checkBox.Content as string);
            }
        }

        var uploadData = new UploadData
        {
            IsPublish = true,
            IMGPath = imgPath,
            VPKPath = vpkPath,
            Title = title,
            Description = description,
            Visibility = visibility,
            Tags = tags
        };
        // 启动上传窗口
        var uploadWindow = new UploadWindow(uploadData)
        {
            Owner = this
        };
        uploadWindow.ShowDialog();
    }

    /// <summary>
    /// 更新选中Mod信息
    /// </summary>
    private void UpdateMod()
    {
        var uploadData = new UploadData();
        uploadData.FileId = OriginItemInfo.Id;

        // 标题
        var title = TextBox_Title.Text.Trim();
        if (title != OriginItemInfo.Title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                NotifierHelper.Show(NotifierType.Warning, "Mod标题不能为空，操作终止");
                return;
            }
            uploadData.Title = title;
        }

        // 预览图
        var imgPath = Button_PreviewImage.Image.Trim();
        if (imgPath != OriginItemInfo.PreviewImage)
        {
            if (!File.Exists(imgPath))
            {
                NotifierHelper.Show(NotifierType.Warning, "Mod预览图文件为空或不存在，操作终止");
                return;
            }
            uploadData.IMGPath = imgPath;
        }

        // 描述
        var description = TextBox_Description.Text.Trim();
        if (description != OriginItemInfo.Description)
            uploadData.Description = description;

        // 可见性
        if (RadioButton_IsPublic.IsChecked == true)
            uploadData.Visibility = 0;
        else if (RadioButton_IsFriendsOnly.IsChecked == true)
            uploadData.Visibility = 1;
        else if (RadioButton_IsPrivate.IsChecked == true)
            uploadData.Visibility = 2;
        else if (RadioButton_IsUnlisted.IsChecked == true)
            uploadData.Visibility = 3;

        // 标签
        foreach (var checkBox in CheckBoxList)
        {
            if (checkBox.IsChecked == true)
            {
                uploadData.Tags.Add(checkBox.Content as string);
            }
        }

        // VPK文件绝对路径
        var vpkPath = TextBox_VPKPath.Text.Trim();
        if (!string.IsNullOrWhiteSpace(vpkPath))
            uploadData.VPKPath = vpkPath;

        // 更新日志
        var changeLog = TextBox_ChangeLog.Text.Trim();
        if (!string.IsNullOrWhiteSpace(changeLog))
            uploadData.ChangeLog = changeLog;

        // 启动上传窗口
        var uploadWindow = new UploadWindow(uploadData)
        {
            Owner = this
        };
        uploadWindow.ShowDialog();
    }
}
