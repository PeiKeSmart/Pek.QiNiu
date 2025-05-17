using NUnit.Framework;
using NUnit.Framework.Legacy;

using Qiniu.Http;
using Qiniu.Storage;
using Qiniu.Util;

namespace Pek.QiNiu.Tests.Storage;

[TestFixture]
public class BucketManagerTests : TestEnv
{
    [Test]
    public void StatTest()
    {
        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        config.UseHttps = true;
        var mac = new Mac(AccessKey, SecretKey);
        var bucketManager = new BucketManager(mac, config);
        var key = "qiniu.png";
        var statRet = bucketManager.Stat(Bucket, key);
        if (statRet.Code != (int)HttpCode.OK)
        {
            Assert.Fail("stat error: " + statRet.ToString());
        }
        Console.WriteLine(statRet.Result.Hash);
        Console.WriteLine(statRet.Result.MimeType);
        Console.WriteLine(statRet.Result.Fsize);
        Console.WriteLine(statRet.Result.FileType);
        ClassicAssert.True(statRet.Result.Hash.Length > 0);
        ClassicAssert.True(statRet.Result.MimeType.Length > 0);
        ClassicAssert.True(statRet.Result.Fsize > 0);
        ClassicAssert.True(statRet.Result.PutTime > 0);
    }

    [Test]
    public void DeleteTest()
    {
        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        var mac = new Mac(AccessKey, SecretKey);
        var bucketManager = new BucketManager(mac, config);
        var newKey = "qiniu-to-delete.png";
        bucketManager.Copy(Bucket, "qiniu.png", Bucket, newKey);
        var deleteRet = bucketManager.Delete(Bucket, newKey);
        if (deleteRet.Code != (int)HttpCode.OK)
        {
            Assert.Fail("delete error: " + deleteRet.ToString());
        }
    }

    [Test]
    public void CopyTest()
    {
        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        var mac = new Mac(AccessKey, SecretKey);
        var bucketManager = new BucketManager(mac, config);
        var newKey = "qiniu-to-copy.png";
        var copyRet = bucketManager.Copy(Bucket, "qiniu.png", Bucket, newKey, true);
        if (copyRet.Code != (int)HttpCode.OK)
        {
            Assert.Fail("copy error: " + copyRet.ToString());
        }
        Console.WriteLine(copyRet.ToString());
    }

    [Test]
    public void MoveTest()
    {
        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        var mac = new Mac(AccessKey, SecretKey);
        var bucketManager = new BucketManager(mac, config);
        var newKey = "qiniu-to-copy.png";
        var copyRet = bucketManager.Copy(Bucket, "qiniu.png", Bucket, newKey, true);
        if (copyRet.Code != (int)HttpCode.OK)
        {
            Assert.Fail("copy error: " + copyRet.ToString());
        }
        Console.WriteLine(copyRet.ToString());

        var moveRet = bucketManager.Move(Bucket, newKey, Bucket, "qiniu-move-target.png", true);
        if (moveRet.Code != (int)HttpCode.OK)
        {
            Assert.Fail("move error: " + moveRet.ToString());
        }
        Console.WriteLine(moveRet.ToString());
    }

    [Test]
    public void ChangeMimeTest()
    {
        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        var mac = new Mac(AccessKey, SecretKey);
        var bucketManager = new BucketManager(mac, config);
        var ret = bucketManager.ChangeMime(Bucket, "qiniu.png", "image/x-png");
        if (ret.Code != (int)HttpCode.OK)
        {
            Assert.Fail("change mime error: " + ret.ToString());
        }
        Console.WriteLine(ret.ToString());
    }

    [Test]
    public void ChangeStatusTest()
    {
        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        var mac = new Mac(AccessKey, SecretKey);
        var bucketManager = new BucketManager(mac, config);
        var key = "qiniu.png";
        var ret = bucketManager.ChangeStatus(Bucket, key, 1);
        if (ret.Code != (int)HttpCode.OK)
        {
            Assert.Fail("change status error: " + ret.ToString());
        }
        var statRet = bucketManager.Stat(Bucket, key);
        if (statRet.Code != (int)HttpCode.OK)
        {
            Assert.Fail("stat error: " + statRet.ToString());
        }
        Assert.That(statRet.Result.Status, Is.EqualTo(1));
        ret = bucketManager.ChangeStatus(Bucket, key, 0);
        if (ret.Code != (int)HttpCode.OK)
        {
            Assert.Fail("change status error: " + ret.ToString());
        }
    }

    [Test]
    public void ChangeTypeTest()
    {
        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        var mac = new Mac(AccessKey, SecretKey);
        var bucketManager = new BucketManager(mac, config);
        var key = "qiniu.png";

        var newKey = "qiniu-to-change-type.png";
        var copyRet = bucketManager.Copy(Bucket, key, Bucket, newKey, true);
        if (copyRet.Code != (int)HttpCode.OK)
        {
            Assert.Fail("copy error: " + copyRet.ToString());
        }
        Console.WriteLine(copyRet.ToString());

        var ret = bucketManager.ChangeType(Bucket, newKey, 1);
        if (ret.Code != (int)HttpCode.OK && !ret.Text.Contains("already in line stat"))
        {
            Assert.Fail("change type error: " + ret.ToString());
        }
        Console.WriteLine(ret.ToString());
        var statRet = bucketManager.Stat(Bucket, newKey);
        if (statRet.Code != (int)HttpCode.OK)
        {
            Assert.Fail("stat error: " + statRet.ToString());
        }
        Assert.That(statRet.Result.FileType, Is.EqualTo(1));
    }

    [Test]
    public void RestoreArArchiveTest()
    {
        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        var mac = new Mac(AccessKey, SecretKey);
        var bucketManager = new BucketManager(mac, config);

        var newKey = "qiniu-archive-to-restore.png";
        var copyRet = bucketManager.Copy(Bucket, "qiniu.png", Bucket, newKey, true);
        if (copyRet.Code != (int)HttpCode.OK)
        {
            Assert.Fail("copy error: " + copyRet.ToString());
        }
        Console.WriteLine(copyRet.ToString());

        var changeTypeRet = bucketManager.ChangeType(Bucket, newKey, 2);
        if (changeTypeRet.Code != (int)HttpCode.OK && !changeTypeRet.Text.Contains("already in line stat"))
        {
            Assert.Fail("change type error: " + changeTypeRet.ToString());
        }

        var ret = bucketManager.RestoreAr(Bucket, newKey, 2);
        if (ret.Code != (int)HttpCode.OK && !ret.Text.Contains("already in line stat"))
        {
            Assert.Fail("change type error: " + ret.ToString());
        }

        var statRet = bucketManager.Stat(Bucket, newKey);
        if (statRet.Code != (int)HttpCode.OK)
        {
            Assert.Fail("stat error: " + statRet.ToString());
        }
        Assert.That(statRet.Result.RestoreStatus, Is.EqualTo(1));
    }

    [Test]
    public void RestoreArDeepArchiveTest()
    {
        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        var mac = new Mac(AccessKey, SecretKey);
        var bucketManager = new BucketManager(mac, config);

        var newKey = "qiniu-deep-archive-to-restore.png";
        var copyRet = bucketManager.Copy(Bucket, "qiniu.png", Bucket, newKey, true);
        if (copyRet.Code != (int)HttpCode.OK)
        {
            Assert.Fail("copy error: " + copyRet.ToString());
        }
        Console.WriteLine(copyRet.ToString());

        var changeTypeRet = bucketManager.ChangeType(Bucket, newKey, 3);
        if (changeTypeRet.Code != (int)HttpCode.OK && !changeTypeRet.Text.Contains("already in line stat"))
        {
            Assert.Fail("change type error: " + changeTypeRet.ToString());
        }

        var ret = bucketManager.RestoreAr(Bucket, newKey, 2);
        if (ret.Code != (int)HttpCode.OK && !ret.Text.Contains("already in line stat"))
        {
            Assert.Fail("change type error: " + ret.ToString());
        }
    }

    [Test]
    public void DeleteAfterDaysTest()
    {
        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        var mac = new Mac(AccessKey, SecretKey);
        var bucketManager = new BucketManager(mac, config);
        var newKey = "qiniu-to-copy.png";
        var copyRet = bucketManager.Copy(Bucket, "qiniu.png", Bucket, newKey, true);
        if (copyRet.Code != (int)HttpCode.OK)
        {
            Assert.Fail("copy error: " + copyRet.ToString());
        }
        Console.WriteLine(copyRet.ToString());
        var expireRet = bucketManager.DeleteAfterDays(Bucket, newKey, 7);
        if (expireRet.Code != (int)HttpCode.OK)
        {
            Assert.Fail("deleteAfterDays error: " + expireRet.ToString());
        }
        Console.WriteLine(expireRet.ToString());
    }

    [Test]
    public void SetObjectLifecycleTest()
    {
        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        var mac = new Mac(AccessKey, SecretKey);
        var bucketManager = new BucketManager(mac, config);
        var newKey = "qiniu-to-set-object-lifecycle.png";
        var copyRet = bucketManager.Copy(Bucket, "qiniu.png", Bucket, newKey, true);
        if (copyRet.Code != (int)HttpCode.OK)
        {
            Assert.Fail("copy error: " + copyRet.ToString());
        }
        Console.WriteLine(copyRet.ToString());
        var ret = bucketManager.SetObjectLifecycle(
            Bucket,
            newKey,
            10,
            20,
            30,
            40,
            15);
        if (ret.Code != (int)HttpCode.OK)
        {
            Assert.Fail("deleteAfterDays error: " + ret.ToString());
        }
        Console.WriteLine(ret.ToString());
        var statRet = bucketManager.Stat(Bucket, newKey);
        if (statRet.Code != (int)HttpCode.OK)
        {
            Assert.Fail("stat error: " + statRet.ToString());
        }
        ClassicAssert.True(statRet.Result.TransitionToIa > 0);
        ClassicAssert.True(statRet.Result.TransitionToArchiveIr > 0);
        ClassicAssert.True(statRet.Result.TransitionToArchive > 0);
        ClassicAssert.True(statRet.Result.TransitionToDeepArchive > 0);
        ClassicAssert.True(statRet.Result.Expiration > 0);
    }

    [Test]
    public void SetObjectLifecycleCondTest()
    {
        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        var mac = new Mac(AccessKey, SecretKey);
        var bucketManager = new BucketManager(mac, config);
        var newKey = "qiniu-to-set-object-lifecycle-cond.png";

        var copyRet = bucketManager.Copy(Bucket, "qiniu.png", Bucket, newKey, true);
        if (copyRet.Code != (int)HttpCode.OK)
        {
            Assert.Fail("copy error: " + copyRet.ToString());
        }
        Console.WriteLine(copyRet.ToString());

        var statRet = bucketManager.Stat(Bucket, newKey);
        if (statRet.Code != (int)HttpCode.OK)
        {
            Assert.Fail("copy error: " + statRet.ToString());
        }


        var ret = bucketManager.SetObjectLifecycle(
            Bucket,
            newKey,
            new Dictionary<string, string>
            {
                    { "hash", statRet.Result.Hash },
                    { "fsize", statRet.Result.Fsize.ToString() }
            },
            10,
            20,
            30,
            40,
            15);
        if (ret.Code != (int)HttpCode.OK)
        {
            Assert.Fail("deleteAfterDays error: " + ret.ToString());
        }
        Console.WriteLine(ret.ToString());
    }

    [Test]
    public void PrefetchTest()
    {
        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        var mac = new Mac(AccessKey, SecretKey);
        var bucketManager = new BucketManager(mac, config);
        var ret = bucketManager.Prefetch(Bucket, "qiniu.png");
        if (ret.Code != (int)HttpCode.OK && !ret.Text.Contains("bucket source not set"))
        {
            Assert.Fail("prefetch error: " + ret.ToString());
        }
        Console.WriteLine(ret.ToString());
    }

    [Test]
    public void DomainsTest()
    {
        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        var mac = new Mac(AccessKey, SecretKey);
        var bucketManager = new BucketManager(mac, config);
        var ret = bucketManager.Domains(Bucket);
        if (ret.Code != (int)HttpCode.OK)
        {
            Assert.Fail("domains error: " + ret.ToString());
        }
        Console.WriteLine(ret.ToString());
    }


    [Test]
    public void BucketsTest()
    {
        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        var mac = new Mac(AccessKey, SecretKey);
        var bucketManager = new BucketManager(mac, config);
        var ret = bucketManager.Buckets(true);
        if (ret.Code != (int)HttpCode.OK)
        {
            Assert.Fail("buckets error: " + ret.ToString());
        }

        foreach (var bucket in ret.Result)
        {
            Console.WriteLine(bucket);
        }
    }


    [Test]
    public void FetchTest()
    {
        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        var mac = new Mac(AccessKey, SecretKey);
        var bucketManager = new BucketManager(mac, config);
        var resUrl = "http://devtools.qiniu.com/qiniu.png";
        var ret = bucketManager.Fetch(resUrl, Bucket, "qiniu-fetch.png");
        if (ret.Code != (int)HttpCode.OK)
        {
            Assert.Fail("fetch error: " + ret.ToString());
        }
        Console.WriteLine(ret.ToString());

        ret = bucketManager.Fetch(resUrl, Bucket, "");
        if (ret.Code != (int)HttpCode.OK)
        {
            Assert.Fail("fetch error: " + ret.ToString());
        }
        Console.WriteLine(ret.ToString());
    }

    [Test]
    public void ListFilesTest()
    {
        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        var mac = new Mac(AccessKey, SecretKey);
        var bucketManager = new BucketManager(mac, config);
        var prefix = "";
        var delimiter = "";
        var limit = 100;
        var marker = "";
        var listRet = bucketManager.ListFiles(Bucket, prefix, marker, limit, delimiter);
        if (listRet.Code != (int)HttpCode.OK)
        {
            Assert.Fail("list files error: " + listRet.ToString());
        }
        Console.WriteLine(listRet.ToString());
        // 需要处理可能存在 key 为 "" 的情况
        var hasEmptyKey = false;
        foreach (var item in listRet.Result.Items)
        {
            ClassicAssert.True(item.Key.Length > 0 || !hasEmptyKey);
            if (item.Key.Length == 0)
            {
                hasEmptyKey = true;
            }
            ClassicAssert.True(item.Hash.Length > 0);
            ClassicAssert.True(item.MimeType.Length > 0);
            ClassicAssert.True(item.Fsize > 0);
            ClassicAssert.True(item.PutTime > 0);
        }
    }

    [Test]
    public void ListBucketTest()
    {
        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        var mac = new Mac(AccessKey, SecretKey);
        var bucketManager = new BucketManager(mac, config);
        var prefix = "";
        var delimiter = "";
        var limit = 100;
        var marker = "";
        do
        {
            var listRet = bucketManager.ListFiles(Bucket, prefix, marker, limit, delimiter);
            if (listRet.Code != (int)HttpCode.OK)
            {
                Assert.Fail("list files error: " + listRet.ToString());
            }
            Console.WriteLine(listRet.ToString());

            marker = listRet.Result.Marker;
        } while (!string.IsNullOrEmpty(marker));
    }

    // batch stat, delete, copy, move, chtype, chgm, deleteAfterDays
    // 批量操作每次不能超过1000个指令
    [Test]
    public void BatchStatTest()
    {
        BatchCopyTest();

        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        var mac = new Mac(AccessKey, SecretKey);
        var bucketManager = new BucketManager(mac, config);
        string[] keys = {
                "qiniu-0.png",
                "qiniu-1.png",
                "qiniu-2.png"
            };

        var ops = new List<string>();
        foreach (var key in keys)
        {
            var op = bucketManager.StatOp(Bucket, key);
            ops.Add(op);
        }

        var ret = bucketManager.Batch(ops);
        if (ret.Code / 100 != 2)
        {
            Assert.Fail("batch error: " + ret.ToString());
        }
        foreach (var info in ret.Result)
        {
            if (info.Code == (int)HttpCode.OK)
            {
                Console.WriteLine("{0}, {1}, {2}, {3}, {4}", info.Data.MimeType,
                    info.Data.PutTime, info.Data.Hash, info.Data.Fsize, info.Data.FileType);
                ClassicAssert.True(info.Data.Hash.Length > 0);
                ClassicAssert.True(info.Data.MimeType.Length > 0);
                ClassicAssert.True(info.Data.Fsize > 0);
                ClassicAssert.True(info.Data.PutTime > 0);
            }
            else
            {
                Console.WriteLine(info.Data.Error);
            }
        }
    }

    [Test]
    public void BatchDeleteTest()
    {
        BatchCopyTest();

        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        var mac = new Mac(AccessKey, SecretKey);
        var bucketManager = new BucketManager(mac, config);
        string[] keys = {
                "qiniu-0.png",
                "qiniu-1.png",
                "qiniu-2.png"
            };

        var ops = new List<string>();
        foreach (var key in keys)
        {
            var op = bucketManager.DeleteOp(Bucket, key);
            ops.Add(op);
        }

        var ret = bucketManager.Batch(ops);
        if (ret.Code / 100 != 2)
        {
            Assert.Fail("batch error: " + ret.ToString());
        }
        foreach (var info in ret.Result)
        {
            if (info.Code == (int)HttpCode.OK)
            {
                Console.WriteLine("delete success");
            }
            else
            {
                Console.WriteLine(info.Data.Error);
            }
        }
    }

    [Test]
    public void BatchCopyTest()
    {
        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        var mac = new Mac(AccessKey, SecretKey);
        var bucketManager = new BucketManager(mac, config);
        string[] keys = {
                "qiniu-0.png",
                "qiniu-1.png",
                "qiniu-2.png"
            };

        var ops = new List<string>();
        foreach (var key in keys)
        {
            var op = bucketManager.CopyOp(Bucket, "qiniu.png", Bucket, key, true);
            ops.Add(op);
        }

        var ret = bucketManager.Batch(ops);
        if (ret.Code / 100 != 2)
        {
            Assert.Fail("batch error: " + ret.ToString());
        }
        foreach (var info in ret.Result)
        {
            if (info.Code == (int)HttpCode.OK)
            {
                Console.WriteLine("copy success");
            }
            else
            {
                Console.WriteLine(info.Data.Error);
            }
        }
    }

    [Test]
    public void BatchMoveTest()
    {
        BatchCopyTest();

        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        var mac = new Mac(AccessKey, SecretKey);
        var bucketManager = new BucketManager(mac, config);
        string[] keys = {
                "qiniu-0.png",
                "qiniu-1.png",
                "qiniu-2.png"
            };

        var ops = new List<string>();
        foreach (var key in keys)
        {
            var op = bucketManager.MoveOp(Bucket, key, Bucket, key + "-batch-move", true);
            ops.Add(op);
        }

        var ret = bucketManager.Batch(ops);
        if (ret.Code / 100 != 2)
        {
            Assert.Fail("batch error: " + ret.ToString());
        }
        foreach (var info in ret.Result)
        {
            if (info.Code == (int)HttpCode.OK)
            {
                Console.WriteLine("move success");
            }
            else
            {
                Console.WriteLine(info.Data.Error);
            }
        }
    }

    [Test]
    public void BatchChangeMimeTest()
    {
        BatchCopyTest();

        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        var mac = new Mac(AccessKey, SecretKey);
        var bucketManager = new BucketManager(mac, config);
        string[] keys = {
                "qiniu-0.png",
                "qiniu-1.png",
                "qiniu-2.png"
            };

        var ops = new List<string>();
        foreach (var key in keys)
        {
            var op = bucketManager.ChangeMimeOp(Bucket, key, "image/batch-x-png");
            ops.Add(op);
        }

        var ret = bucketManager.Batch(ops);
        if (ret.Code / 100 != 2)
        {
            Assert.Fail("batch error: " + ret.ToString());
        }
        foreach (var info in ret.Result)
        {
            if (info.Code == (int)HttpCode.OK)
            {
                Console.WriteLine("chgm success");
            }
            else
            {
                Console.WriteLine(info.Data.Error);
            }
        }
    }

    [Test]
    public void BatchChangeTypeTest()
    {
        BatchCopyTest();

        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        var mac = new Mac(AccessKey, SecretKey);
        var bucketManager = new BucketManager(mac, config);
        string[] keys = {
                "qiniu-0.png",
                "qiniu-1.png",
                "qiniu-2.png"
            };

        var ops = new List<string>();
        foreach (var key in keys)
        {
            var op = bucketManager.ChangeTypeOp(Bucket, key, 0);
            ops.Add(op);
        }

        var ret = bucketManager.Batch(ops);
        if (ret.Code / 100 != 2)
        {
            Assert.Fail("batch error: " + ret.ToString());
        }
        foreach (var info in ret.Result)
        {
            if (info.Code == (int)HttpCode.OK)
            {
                Console.WriteLine("chtype success");
            }
            else
            {
                Console.WriteLine(info.Data.Error);
            }
        }
    }

    [Test]
    public void BatchDeleteAfterDaysTest()
    {
        BatchCopyTest();

        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        var mac = new Mac(AccessKey, SecretKey);
        var bucketManager = new BucketManager(mac, config);
        string[] keys = {
                "qiniu-0.png",
                "qiniu-1.png",
                "qiniu-2.png"
            };

        var ops = new List<string>();
        foreach (var key in keys)
        {
            var op = bucketManager.DeleteAfterDaysOp(Bucket, key, 7);
            ops.Add(op);
        }

        var ret = bucketManager.Batch(ops);
        if (ret.Code / 100 != 2)
        {
            Assert.Fail("batch error: " + ret.ToString());
        }
        foreach (var info in ret.Result)
        {
            if (info.Code == (int)HttpCode.OK)
            {
                Console.WriteLine("deleteAfterDays success");
            }
            else
            {
                Console.WriteLine(info.Data.Error);
            }
        }
    }
}