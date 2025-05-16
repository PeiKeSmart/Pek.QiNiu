using NUnit.Framework;

using QiNiu;

namespace Pek.QiNiu.Tests;

/// <summary>
/// 七牛SDK基础测试类
/// </summary>
[TestFixture]
public class QiniuCSharpSDKTests
{
    [Test]
    public void Version_ShouldReturnCorrectVersion()
    {
        // Arrange & Act
        string version = QiniuCSharpSDK.VERSION;

        // Assert
        Assert.That(version, Is.EqualTo("8.7.0"));
    }

    [Test]
    public void RTFX_ShouldReturnCorrectFramework()
    {
        // Arrange & Act
        string rtfx = QiniuCSharpSDK.RTFX;

        // Assert
        // 根据编译条件，RTFX可能有不同的值
        Assert.That(rtfx, Is.Not.Null);
        Assert.That(rtfx, Is.Not.Empty);
    }

    [Test]
    public void ALIAS_ShouldReturnCorrectName()
    {
        // Arrange & Act
        string alias = QiniuCSharpSDK.ALIAS;

        // Assert
        Assert.That(alias, Is.EqualTo("QiniuCSharpSDK"));
    }
}
