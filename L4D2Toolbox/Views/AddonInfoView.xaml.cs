using L4D2Toolbox.Helper;
using Microsoft.Win32;

namespace L4D2Toolbox.Views;

/// <summary>
/// AddonInfoView.xaml 的交互逻辑
/// </summary>
public partial class AddonInfoView : UserControl
{
    public AddonInfoView()
    {
        InitializeComponent();
        this.DataContext = this;
        MainWindow.WindowClosingEvent += MainWindow_WindowClosingEvent;
    }

    private void MainWindow_WindowClosingEvent()
    {

    }

    private void Button_BuildAddonInfo_Click(object sender, RoutedEventArgs e)
    {
        var addonTitle = TextBox_addonTitle.Text.Trim();
        var addonVersion = TextBox_addonVersion.Text.Trim();
        var addonAuthor = TextBox_addonAuthor.Text.Trim();
        var addonAuthorSteamID = TextBox_addonAuthorSteamID.Text.Trim();
        var addonTagLine = TextBox_addonTagLine.Text.Trim();
        var addonURL0 = TextBox_addonURL0.Text.Trim();
        var addonDescription = TextBox_addonDescription.Text.Trim();

        var builder = new StringBuilder();
        builder.AppendLine("\"AddonInfo\"");
        builder.AppendLine("{");
        builder.AppendLine("\t\"addonSteamAppID\"                550");
        builder.AppendLine("");
        builder.AppendLine($"\t\"addonTitle\"                     \"{addonTitle}\"");
        builder.AppendLine($"\t\"addonVersion\"                   \"{addonVersion}\"");
        builder.AppendLine($"\t\"addonAuthor\"                    \"{addonAuthor}\"");
        builder.AppendLine($"\t\"addonAuthorSteamID\"             \"{addonAuthorSteamID}\"");
        builder.AppendLine($"\t\"addonTagLine\"                   \"{addonTagLine}\"");
        builder.AppendLine($"\t\"addonURL0\"                      \"{addonURL0}\"");
        builder.AppendLine($"\t\"addonDescription\"               \"{addonDescription}\"");
        builder.AppendLine("");
        builder.AppendLine($"\t\"addonContent_Campaign\"          \"{Convert.ToInt32(CheckBox_Campaign.IsChecked == true)}\"");
        builder.AppendLine($"\t\"addonContent_Map\"               \"{Convert.ToInt32(CheckBox_Map.IsChecked == true)}\"");
        builder.AppendLine($"\t\"addonContent_Survivor\"          \"{Convert.ToInt32(CheckBox_Survivor.IsChecked == true)}\"");
        builder.AppendLine($"\t\"addonContent_Skin\"              \"{Convert.ToInt32(CheckBox_Skin.IsChecked == true)}\"");
        builder.AppendLine($"\t\"addonContent_BossInfected\"      \"{Convert.ToInt32(CheckBox_BossInfected.IsChecked == true)}\"");
        builder.AppendLine($"\t\"addonContent_CommonInfected\"    \"{Convert.ToInt32(CheckBox_CommonInfected.IsChecked == true)}\"");
        builder.AppendLine($"\t\"addonContent_Music\"             \"{Convert.ToInt32(CheckBox_Music.IsChecked == true)}\"");
        builder.AppendLine($"\t\"addonContent_Sound\"             \"{Convert.ToInt32(CheckBox_Sound.IsChecked == true)}\"");
        builder.AppendLine($"\t\"addonContent_Prop\"              \"{Convert.ToInt32(CheckBox_Prop.IsChecked == true)}\"");
        builder.AppendLine($"\t\"addonContent_Weapon\"            \"{Convert.ToInt32(CheckBox_Weapon.IsChecked == true)}\"");
        builder.AppendLine($"\t\"addonContent_Script\"            \"{Convert.ToInt32(CheckBox_Script.IsChecked == true)}\"");
        builder.Append('}');

        TextBox_AddonInfo.Text = builder.ToString();

        NotifierHelper.Show(NotifierType.Success, "生成AddonInfo内容成功");
    }

    private void Button_SaveAddonInfo_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(TextBox_AddonInfo.Text))
        {
            NotifierHelper.Show(NotifierType.Success, "AddonInfo内容为空，操作取消");
            return;
        }

        var fileDialog = new SaveFileDialog
        {
            RestoreDirectory = true,
            Filter = "文本文件 (*.txt)|*.txt",
            FileName = "addoninfo.txt"
        };

        if (fileDialog.ShowDialog() == true)
        {
            File.WriteAllText(fileDialog.FileName, TextBox_AddonInfo.Text);
            NotifierHelper.Show(NotifierType.Success, "另存为addoninfo.txt文件成功");
        }
    }
}
