using System.Text;

using Newtonsoft.Json;

using NUnit.Framework;
using NUnit.Framework.Legacy;

using Qiniu.Http;
using Qiniu.Storage;
using Qiniu.Util;

namespace Pek.QiNiu.Tests.Storage;

[TestFixture]
public class FormUploaderTests : TestEnv
{
    [Test]
    public void UploadFileTest()
    {
        var mac = new Mac(AccessKey, SecretKey);
        var rand = new Random();
        var key = string.Format("UploadFileTest_{0}.dat", rand.Next());

        var tempPath = System.IO.Path.GetTempPath();
        var rnd = new Random().Next(1, 100000);
        var filePath = tempPath + "resumeFile" + rnd.ToString();
        var testBody = new char[4 * 1024 * 1024];
        var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
        var sw = new System.IO.StreamWriter(stream, System.Text.Encoding.Default);
        sw.Write(testBody);
        sw.Close();
        stream.Close();

        var putPolicy = new PutPolicy();
        putPolicy.Scope = Bucket + ":" + key;
        putPolicy.SetExpires(3600);
        putPolicy.DeleteAfterDays = 1;
        var token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        config.UseHttps = true;
        config.UseCdnDomains = true;
        config.ChunkSize = ChunkUnit.U512K;
        var target = new FormUploader(config);
        var result = target.UploadFile(filePath, key, token, new PutExtra());
        Console.WriteLine("form upload result: " + result.ToString());
        Assert.That(result.Code, Is.EqualTo((int)HttpCode.OK));
        System.IO.File.Delete(filePath);
    }

    [Test]
    public void UploadFileV2Test()
    {
        var mac = new Mac(AccessKey, SecretKey);
        var rand = new Random();
        var key = string.Format("UploadFileTest_{0}.dat", rand.Next());

        var tempPath = System.IO.Path.GetTempPath();
        var rnd = new Random().Next(1, 100000);
        var filePath = tempPath + "resumeFile" + rnd.ToString();
        var testBody = new char[4 * 1024 * 1024];
        var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
        var sw = new System.IO.StreamWriter(stream, System.Text.Encoding.Default);
        sw.Write(testBody);
        sw.Close();
        stream.Close();

        var putPolicy = new PutPolicy();
        putPolicy.Scope = Bucket + ":" + key;
        putPolicy.SetExpires(3600);
        putPolicy.DeleteAfterDays = 1;
        var token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        config.UseHttps = true;
        config.UseCdnDomains = true;
        config.ChunkSize = ChunkUnit.U512K;
        var target = new FormUploader(config);
        var extra = new PutExtra();
        extra.Version = "v2";
        extra.PartSize = 4 * 1024 * 1024;
        var result = target.UploadFile(filePath, key, token, extra);
        Console.WriteLine("form upload result: " + result.ToString());
        Assert.That(result.Code, Is.EqualTo((int)HttpCode.OK));
        System.IO.File.Delete(filePath);
    }

    [TestCaseSource(typeof(OperationManagerTests), nameof(OperationManagerTests.PfopOptionsTestCases))]
    public void UploadFileWithPersistOptionsTest(int type, string workflowId)
    {
        var mac = new Mac(AccessKey, SecretKey);
        var bucketName = Bucket;
        var key = "test-pfop/upload-file";

        // generate file to upload
        var tempPath = System.IO.Path.GetTempPath();
        var rnd = new Random().Next(1, 100000);
        var filePath = tempPath + "resumeFile" + rnd.ToString();
        var testBody = new char[4 * 1024 * 1024];
        var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
        var sw = new System.IO.StreamWriter(stream, System.Text.Encoding.Default);
        sw.Write(testBody);
        sw.Close();
        stream.Close();

        // generate put policy
        var putPolicy = new PutPolicy();
        putPolicy.Scope = string.Join(":", bucketName, key);
        putPolicy.SetExpires(3600);
        putPolicy.DeleteAfterDays = 1;

        var persistentKeyBuilder = new StringBuilder("test-pfop/test-pfop-by-upload");
        if (type > 0)
        {
            persistentKeyBuilder.Append("type_" + type);
            putPolicy.PersistentType = type;
        }

        if (!string.IsNullOrEmpty(workflowId))
        {
            putPolicy.PersistentWorkflowTemplateId = workflowId;
        }
        else
        {
            var saveEntry = Base64.UrlSafeBase64Encode(string.Join(
                ":",
                bucketName,
                persistentKeyBuilder.ToString()
            ));
            putPolicy.PersistentOps = "avinfo|saveas/" + saveEntry;
        }

        // upload
        var config = new Config();
        config.UseHttps = true;
        config.UseCdnDomains = true;
        var token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
        var uploader = new FormUploader(config);
        var result = uploader.UploadFile(filePath, key, token, new PutExtra());
        Console.WriteLine("form upload result: " + result.ToString());
        Assert.That(result.Code, Is.EqualTo((int)HttpCode.OK));
        System.IO.File.Delete(filePath);

        // get persist task info
        var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(result.Text.ToString());
        ClassicAssert.IsTrue(dict != null && dict.ContainsKey("persistentId"));
        var manager = new OperationManager(mac, config);
        var persistentId = dict != null && dict.ContainsKey("persistentId") ? dict["persistentId"]?.ToString() : string.Empty;
        var prefopRet = manager.Prefop(persistentId ?? string.Empty);

        // assert the result
        if (prefopRet.Code != (int)HttpCode.OK)
        {
            ClassicAssert.Fail("prefop error: " + prefopRet.ToString());
        }

        ClassicAssert.IsNotNull(prefopRet.Result.CreationDate);
        ClassicAssert.IsTrue(!string.IsNullOrEmpty(prefopRet.Result.CreationDate));

        if (type == 1)
        {
            Assert.That(prefopRet.Result.Type, Is.EqualTo(1));
        }

        if (!string.IsNullOrEmpty(workflowId))
        {
            ClassicAssert.IsNotNull(prefopRet.Result.TaskFrom);
            ClassicAssert.IsTrue(!string.IsNullOrEmpty(prefopRet.Result.TaskFrom));
            ClassicAssert.IsTrue(prefopRet.Result.TaskFrom.Contains(workflowId));
        }
    }
}