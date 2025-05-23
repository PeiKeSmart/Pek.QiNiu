﻿namespace QiNiu;

/// <summary>
/// Qiniu (Cloud) C# SDK for .NET Framework 2.0+/Core/UWP
/// Modules in this SDK:
/// "Storage" 存储相关功能，上传，下载，数据处理，资源管理
/// "CDN",    Fusion CDN, 融合CDN加速;
/// "Util",   Utilities such as MD5 hashing, 实用工具(如MD5哈希计算等);
/// "Http", HTTP Request Manager, HTTP请求管理器
/// </summary>
public class QiniuCSharpSDK
{
    /// <summary>
    /// SDK名称
    /// </summary>
    public const String ALIAS = "QiniuCSharpSDK";

    /// <summary>
    /// 目标框架
    /// </summary>
#if Net20
    public const String RTFX = "NET20";
#elif Net35
    public const String RTFX = "NET35";
#elif Net40
    public const String RTFX = "NET40";
#elif Net45
    public const String RTFX = "NET45";
#elif Net46
    public const String RTFX = "NET46";
#elif NetCore
    public const String RTFX = "NETCore";
#elif WINDOWS_UWP
    public const String RTFX = "UWP";
#else
    public const String RTFX = "UNKNOWN";
#endif

    /// <summary>
    /// SDK版本号
    /// </summary>
    public const String VERSION = "8.7.0";

}
