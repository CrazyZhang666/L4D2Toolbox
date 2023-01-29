namespace L4D2Toolbox.Core;

public static class Globals
{
    /// <summary>
    /// 求生之路2 AppID
    /// </summary>
    public const int AppID = 550;

    /// <summary>
    /// 求生之路2 Authing Tools AppID
    /// </summary>
    public const int App2ID = 563;

    /////////////////////////////////////////////////////////////

    public const string OutputDir = ".\\__Output";
    public const string ConfigDir = ".\\Config";
    public const string CacheDir = ".\\Cache";

    public const string AppDataDir = ".\\AppData";

    public const string SharedDir = $"{AppDataDir}\\__Shared";

    public const string AppSurvivorsDir = $"{AppDataDir}\\Survivors";
    public const string AppWeaponsDir = $"{AppDataDir}\\Weapons";

    public const string AppSurvivorsQc = $"{AppSurvivorsDir}\\__Main.qci";
    public const string AppWeaponsQc = $"{AppWeaponsDir}\\__Main.qci";

    public const string MacTypeDir = $"{AppDataDir}\\MacType";
    public const string FontLoaderExec = $"{MacTypeDir}\\MacLoader.exe";

    public const string ToolkitDir = $"{AppDataDir}\\Toolkit";
    public const string GCFScapeExec = $"{ToolkitDir}\\GCFScape\\GCFScape.exe";
    public const string VTFEditExec = $"{ToolkitDir}\\VTFEdit\\VTFEdit.exe";
    public const string BSPSourceExec = $"{ToolkitDir}\\BSPSource\\bspsrc.bat";
    public const string VPK2Exec = $"{ToolkitDir}\\VPK\\vpk.exe";
    public const string CrowbarExec = $"{ToolkitDir}\\Crowbar\\Crowbar.exe";

    public static string FullOutputDir = Directory.GetCurrentDirectory() + "\\__Output";

    /////////////////////////////////////////////////////////////

    public static string UnPackDir { get; set; } = string.Empty;

    public static string UnPackAddonImagePath
    {
        get { return $"{UnPackDir}\\addonimage.jpg"; }
    }

    public static string UnPackAddonInfoPath
    {
        get { return $"{UnPackDir}\\addoninfo.txt"; }
    }

    public static string UnPackMaterialsDir
    {
        get { return $"{UnPackDir}\\materials"; }
    }

    public static string UnPackVGUIDir
    {
        get { return $"{UnPackMaterialsDir}\\vgui"; }
    }

    public static string UnPackSurvivorsDecoDir
    {
        get { return $"{UnPackDir}\\models\\survivors\\decompiled 0.72"; }
    }

    public static string UnPackWeaponsDecoDir
    {
        get { return $"{UnPackDir}\\models\\weapons\\arms\\decompiled 0.72"; }
    }

    /////////////////////////////////////////////////////////////

    public static string L4D2MainExec { get; set; } = string.Empty;
    public static string L4D2MainDir { get; set; } = string.Empty;

    public static string StudiomdlExec
    {
        get { return $"{L4D2MainDir}\\bin\\studiomdl.exe"; }
    }

    public static string VPKExec
    {
        get { return $"{L4D2MainDir}\\bin\\vpk.exe"; }
    }

    public static string HammerExec
    {
        get { return $"{L4D2MainDir}\\bin\\hammer.exe"; }
    }

    public static string L4D2AddonListTxt
    {
        get { return $"{L4D2MainDir}\\left4dead2\\addonlist.txt"; }
    }

    public static string L4D2SurvivorsDir
    {
        get { return $"{L4D2MainDir}\\left4dead2\\models\\survivors"; }
    }

    public static string L4D2WeaponsDir
    {
        get { return $"{L4D2MainDir}\\left4dead2\\models\\weapons\\arms"; }
    }

    public static string L4D2AddonsDir
    {
        get { return $"{L4D2MainDir}\\left4dead2\\addons"; }
    }

    public static string L4D2WorkshopDir
    {
        get { return $"{L4D2MainDir}\\left4dead2\\addons\\workshop"; }
    }

    public static string L4D2MapsDir
    {
        get { return $"{L4D2MainDir}\\left4dead2\\maps"; }
    }

    public static string L4D2ComplieArgs
    {
        get { return $"-game \"{L4D2MainDir}\\left4dead2\" -nop4 -verbose"; }
    }
}