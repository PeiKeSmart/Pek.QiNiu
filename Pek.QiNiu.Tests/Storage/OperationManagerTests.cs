using System.Collections;
using System.Text;

using NUnit.Framework;

using Qiniu.Http;
using Qiniu.Storage;
using Qiniu.Util;

namespace Pek.QiNiu.Tests.Storage;

[TestFixture]
public class OperationManagerTests : TestEnv
{
    private OperationManager getOperationManager()
    {
        var mac = new Mac(AccessKey, SecretKey);
        var config = new Config();
        config.UseHttps = true;

        var manager = new OperationManager(mac, config);
        return manager;
    }

    [Test]
    public void PfopAndPrefopTest()
    {
        var key = "qiniu.mp4";
        var force = true;
        var pipeline = "sdktest";
        var notifyUrl = "http://api.example.com/qiniu/pfop/notify";
        var saveMp4Entry = Base64.UrlSafeBase64Encode(Bucket + ":avthumb_test_target.mp4");
        var saveJpgEntry = Base64.UrlSafeBase64Encode(Bucket + ":vframe_test_target.jpg");
        var avthumbMp4Fop = "avthumb/mp4|saveas/" + saveMp4Entry;
        var vframeJpgFop = "vframe/jpg/offset/1|saveas/" + saveJpgEntry;
        var fops = string.Join(";", new string[] { avthumbMp4Fop, vframeJpgFop });

        var manager = getOperationManager();
        var pfopRet = manager.Pfop(Bucket, key, fops, pipeline, notifyUrl, force);
        if (pfopRet.Code != (int)HttpCode.OK)
        {
            Assert.Fail("pfop error: " + pfopRet.ToString());
        }
        Console.WriteLine(pfopRet.PersistentId);

        var ret = manager.Prefop(pfopRet.PersistentId);
        if (ret.Code != (int)HttpCode.OK)
        {
            Assert.Fail("prefop error: " + ret.ToString());
        }
        Console.WriteLine(ret.ToString());
    }

    public static IEnumerable PfopOptionsTestCases
    {
        get
        {
            yield return new TestCaseData(
                0, // type
                null // workflow template id
            );
            yield return new TestCaseData(
                1,
                null
            );
            yield return new TestCaseData(
                0,
                "test-workflow"
            );
        }
    }

    [TestCaseSource(typeof(OperationManagerTests), nameof(PfopOptionsTestCases))]
    public void PfopWithOptionsTest(int type, string workflowId)
    {
        var bucketName = Bucket;
        var key = "qiniu.mp4";

        var persistentKeyBuilder = new StringBuilder("test-pfop/test-pfop-by-api");
        if (type > 0)
        {
            persistentKeyBuilder.Append("type_" + type);
        }

        string fops;
        if (!string.IsNullOrEmpty(workflowId))
        {
            fops = null;
        }
        else
        {
            var saveEntry = Base64.UrlSafeBase64Encode(string.Join(
                ":",
                bucketName,
                persistentKeyBuilder.ToString()
            ));
            fops = "avinfo|saveas/" + saveEntry;
        }

        var manager = getOperationManager();
        var pfopRet = manager.Pfop(
            Bucket,
            key,
            fops,
            null,
            null,
            true,
            type,
            workflowId
        );
        if (pfopRet.Code != (int)HttpCode.OK)
        {
            Assert.Fail("pfop error: " + pfopRet);
        }

        var prefopRet = manager.Prefop(pfopRet.PersistentId);
        if (prefopRet.Code != (int)HttpCode.OK)
        {
            Assert.Fail("prefop error: " + prefopRet);
        }

        Assert.That(prefopRet.Result.CreationDate, Is.Not.Null.And.Not.Empty);

        if (type == 1)
        {
            Assert.That(prefopRet.Result.Type, Is.EqualTo(1));
        }

        if (!string.IsNullOrEmpty(workflowId))
        {
            Assert.That(prefopRet.Result.TaskFrom, Is.Not.Null.And.Not.Empty);
            Assert.That(prefopRet.Result.TaskFrom.Contains(workflowId), Is.True);
        }
    }
}