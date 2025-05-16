using NUnit.Framework;
using Qiniu.Storage;

namespace Pek.QiNiu.Tests.Storage;

/// <summary>
/// 存储配置测试类
/// </summary>
[TestFixture]
public class ConfigTests : TestEnv
{
    private Config _config;

    [SetUp]
    public void Setup()
    {
        _config = new Config();
    }

    [Test]
    public void DefaultProperties_ShouldHaveCorrectValues()
    {
        // Assert
        Assert.That(_config.UseHttps, Is.False);
        Assert.That(_config.UseCdnDomains, Is.False);
        Assert.That(_config.ChunkSize, Is.EqualTo(ChunkUnit.U4096K));
        Assert.That(_config.MaxRetryTimes, Is.EqualTo(3));
    }

    [Test]
    public void UcHost_ShouldReturnCorrectUrl()
    {
        // Act
        string httpUrl = _config.UcHost();
        
        // Assert
        Assert.That(httpUrl, Is.EqualTo("http://uc.qiniuapi.com"));
        
        // 修改为HTTPS
        _config.UseHttps = true;
        string httpsUrl = _config.UcHost();
        
        // Assert
        Assert.That(httpsUrl, Is.EqualTo("https://uc.qiniuapi.com"));
    }

    [Test]
    public void SetUcHost_ShouldChangeHost()
    {
        // Arrange
        string customHost = "custom.qiniuapi.com";
        
        // Act
        _config.SetUcHost(customHost);
        string url = _config.UcHost();
        
        // Assert
        Assert.That(url, Is.EqualTo($"http://{customHost}"));
    }

    [Test]
    public void UcHostTest()
    {
        Config config = new Config();
        string ucHost = config.UcHost();
        Assert.AreEqual("http://uc.qiniuapi.com", ucHost);
        config.SetUcHost("uc.example.com");
        ucHost = config.UcHost();
        Assert.AreEqual("http://uc.example.com", ucHost);

        config = new Config();
        config.UseHttps = true;
        ucHost = config.UcHost();
        Assert.AreEqual("https://uc.qiniuapi.com", ucHost);
        config.SetUcHost("uc.example.com");
        ucHost = config.UcHost();
        Assert.AreEqual("https://uc.example.com", ucHost);
    }
}
