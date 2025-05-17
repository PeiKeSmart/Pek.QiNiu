using NUnit.Framework;

using Qiniu.Storage;

namespace Pek.QiNiu.Tests.Storage;

[TestFixture]
public class ZoneHelperTests : TestEnv
{
    [Test]
    public void QueryZoneTest()
    {
        var zone = ZoneHelper.QueryZone(AccessKey, Bucket);
        Assert.That(zone, Is.Not.Null);
    }

    [Test]
    public void QueryZoneWithCustomQueryRegionHost()
    {
        var config = new Config();
        config.SetQueryRegionHost("uc.qbox.me");
        config.UseHttps = true;
        var zone = ZoneHelper.QueryZone(
            AccessKey,
            Bucket,
            config.UcHost()
        );
        Assert.That(zone, Is.Not.Null);
    }

    [Test]
    public void QueryZoneWithBackupHostsTest()
    {
        var config = new Config();
        config.SetQueryRegionHost("fake-uc.csharp.qiniu.com");
        config.SetBackupQueryRegionHosts(new List<string>
                {
                    "unavailable-uc.csharp.qiniu.com",
                    "uc.qbox.me"
                }
        );
        config.UseHttps = true;
        var zone = ZoneHelper.QueryZone(
            AccessKey,
            Bucket,
            config.UcHost(),
            config.BackupQueryRegionHosts()
        );
        Assert.That(zone, Is.Not.Null);
    }

    [Test]
    public void QueryZoneWithUcAndBackupHostsTest()
    {
        var config = new Config();
        config.SetUcHost("fake-uc.csharp.qiniu.com");
        config.SetBackupQueryRegionHosts(new List<string>
                {
                    "unavailable-uc.csharp.qiniu.com",
                    "uc.qbox.me"
                }
        );
        config.UseHttps = true;
        var zone = ZoneHelper.QueryZone(
            AccessKey,
            Bucket,
            config.UcHost(),
            config.BackupQueryRegionHosts()
        );
        Assert.That(zone, Is.Not.Null);
    }
}