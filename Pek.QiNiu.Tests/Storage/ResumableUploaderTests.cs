using System.Net.Http;
using System.Security.Cryptography;

using Newtonsoft.Json;

using NUnit.Framework;
using NUnit.Framework.Legacy;

using Qiniu.Http;
using Qiniu.Storage;
using Qiniu.Util;

namespace Pek.QiNiu.Tests.Storage;

[TestFixture]
public class ResumableUploaderTests : TestEnv
{
    [Test]
    public void UploadFileTest()
    {
        var mac = new Mac(AccessKey, SecretKey);
        var rand = new Random();
        var key = string.Format("UploadFileTest_{0}.dat", rand.Next());
        var tempPath = System.IO.Path.GetTempPath();
        var rnd = new Random().Next(1, 100000);
        var filePath = Path.Combine(tempPath, "resumeFile" + rnd.ToString());
        var testBody = new char[6 * 1024 * 1024];
        var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
        var sw = new System.IO.StreamWriter(stream, System.Text.Encoding.Default);
        sw.Write(testBody);
        sw.Close();
        stream.Close();
        var putPolicy = new PutPolicy();
        putPolicy.Scope = Bucket + ":" + key;
        putPolicy.SetExpires(3600);
        putPolicy.DeleteAfterDays = 1;
        putPolicy.ReturnBody = "{\"hash\":$(etag),\"fname\":$(fname),\"var_1\":$(x:var_1),\"var_2\":$(x:var_2)}";
        var token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        config.UseHttps = true;
        config.UseCdnDomains = true;
        config.ChunkSize = ChunkUnit.U512K;
        var putExtra = new PutExtra();
        putExtra.MimeType = "application/json";
        putExtra.Params = new Dictionary<string, string>();
        putExtra.Params["x:var_1"] = "val_1";
        putExtra.Params["x:var_2"] = "val_2";
        putExtra.BlockUploadThreads = 2;
        var target = new ResumableUploader(config);
        var result = target.UploadFile(filePath, key, token, putExtra);
        Console.WriteLine("chunk upload result: " + result.ToString());
        Assert.That(result.Code, Is.EqualTo((int)HttpCode.OK));
        var responseBody = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.Text);
        Assert.That(responseBody, Is.Not.Null);
        Assert.That(responseBody.ContainsKey("fname"), Is.True);
        Assert.That(responseBody["fname"], Is.EqualTo(key));
        Assert.That(responseBody["var_1"], Is.EqualTo("val_1"));
        Assert.That(responseBody["var_2"], Is.EqualTo("val_2"));
        var downloadUrl = string.Format("http://{0}/{1}", Domain, key);
        using (var httpClient = new HttpClient())
        using (var httpResp = httpClient.GetAsync(downloadUrl).Result)
        {
            Assert.That((int)httpResp.StatusCode, Is.EqualTo((int)HttpCode.OK));
            Assert.That(httpResp.Content.Headers.ContentType, Is.Not.Null);
            Assert.That(httpResp.Content.Headers.ContentType.MediaType, Is.EqualTo("application/json"));
            using (var md5_1 = MD5.Create())
            using (var md5_2 = MD5.Create())
            using (var fileStream = File.OpenRead(filePath))
            using (var respStream = httpResp.Content.ReadAsStream())
            {
                var checksum1 = md5_1.ComputeHash(fileStream);
                var checksum2 = md5_2.ComputeHash(respStream);
                Assert.That(checksum1, Is.EqualTo(checksum2));
            }
        }
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
        var testBody = new char[6 * 1024 * 1024];
        var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
        var sw = new System.IO.StreamWriter(stream, System.Text.Encoding.Default);
        sw.Write(testBody);
        sw.Close();
        stream.Close();

        var putPolicy = new PutPolicy();
        putPolicy.Scope = Bucket + ":" + key;
        putPolicy.SetExpires(3600);
        putPolicy.DeleteAfterDays = 1;
        putPolicy.ReturnBody = "{\"hash\":$(etag),\"fname\":$(fname),\"var_1\":$(x:var_1),\"var_2\":$(x:var_2)}";
        var token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());

        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        config.UseHttps = true;
        config.UseCdnDomains = true;
        config.ChunkSize = ChunkUnit.U512K;
        var extra = new PutExtra();
        extra.MimeType = "application/json";
        extra.Version = "v2";
        extra.PartSize = 4 * 1024 * 1024;
        extra.Params = new Dictionary<string, string>();
        extra.Params["x:var_1"] = "val_1";
        extra.Params["x:var_2"] = "val_2";
        extra.BlockUploadThreads = 2;
        var target = new ResumableUploader(config);
        var result = target.UploadFile(filePath, key, token, extra);
        Console.WriteLine("chunk upload result: " + result.ToString());
        Assert.That(result.Code, Is.EqualTo((int)HttpCode.OK));
        var responseBody = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.Text);
        Assert.That(responseBody, Is.Not.Null);
        Assert.That(responseBody.ContainsKey("fname"), Is.True);
        Assert.That(responseBody["fname"], Is.EqualTo(key));
        Assert.That(responseBody["var_1"], Is.EqualTo("val_1"));
        Assert.That(responseBody["var_2"], Is.EqualTo("val_2"));

        var downloadUrl = string.Format("http://{0}/{1}", Domain, key);
        using (var httpClient = new HttpClient())
        using (var httpResp = httpClient.GetAsync(downloadUrl).Result)
        {
            Assert.That((int)httpResp.StatusCode, Is.EqualTo((int)HttpCode.OK));
            Assert.That(httpResp.Content.Headers.ContentType, Is.Not.Null);
            Assert.That(httpResp.Content.Headers.ContentType.MediaType, Is.EqualTo("application/json"));
            using (var md5_1 = MD5.Create())
            using (var md5_2 = MD5.Create())
            using (var fileStream = File.OpenRead(filePath))
            using (var respStream = httpResp.Content.ReadAsStream())
            {
                var checksum1 = md5_1.ComputeHash(fileStream);
                var checksum2 = md5_2.ComputeHash(respStream);
                Assert.That(checksum1, Is.EqualTo(checksum2));
            }
        }

        System.IO.File.Delete(filePath);
    }

    [Test]
    public void UploadFileV2WithoutKeyTest()
    {
        var mac = new Mac(AccessKey, SecretKey);

        var tempPath = Path.GetTempPath();
        var rnd = new Random().Next(1, 100000);
        var filePath = tempPath + "resumeFile" + rnd.ToString();
        var testBody = new char[6 * 1024 * 1024];
        var stream = new FileStream(filePath, FileMode.Create);
        var sw = new StreamWriter(stream, System.Text.Encoding.Default);
        sw.Write(testBody);
        sw.Close();
        stream.Close();

        var putPolicy = new PutPolicy();
        putPolicy.Scope = Bucket;
        putPolicy.SetExpires(3600);
        putPolicy.DeleteAfterDays = 1;
        var token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());

        var config = new Config();
        config.Zone = Zone.ZONE_CN_East;
        config.UseHttps = true;
        config.UseCdnDomains = true;
        config.ChunkSize = ChunkUnit.U512K;
        var extra = new PutExtra();
        extra.MimeType = "application/json";
        extra.Version = "v2";
        extra.PartSize = 4 * 1024 * 1024;
        var target = new ResumableUploader(config);
        var result = target.UploadFile(filePath, string.Empty, token, extra);
        Console.WriteLine("chunk upload result: " + result.ToString());
        Assert.That(result.Code, Is.EqualTo((int)HttpCode.OK));
        var responseBody = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.Text);
        Assert.That(responseBody, Is.Not.Null);
        Assert.That(responseBody.ContainsKey("hash"), Is.True);
        Assert.That(responseBody.ContainsKey("key"), Is.True);
        Assert.That(responseBody["hash"], Is.EqualTo(responseBody["key"]));

        var downloadUrl = string.Format("http://{0}/{1}", Domain, responseBody["key"]);
        using (var httpClient = new HttpClient())
        using (var httpResp = httpClient.GetAsync(downloadUrl).Result)
        {
            Assert.That((int)httpResp.StatusCode, Is.EqualTo((int)HttpCode.OK));
            Assert.That(httpResp.Content.Headers.ContentType, Is.Not.Null);
            Assert.That(httpResp.Content.Headers.ContentType.MediaType, Is.EqualTo("application/json"));
            using (var md5_1 = MD5.Create())
            using (var md5_2 = MD5.Create())
            using (var fileStream = File.OpenRead(filePath))
            using (var respStream = httpResp.Content.ReadAsStream())
            {
                var checksum1 = md5_1.ComputeHash(fileStream);
                var checksum2 = md5_2.ComputeHash(respStream);
                Assert.That(checksum1, Is.EqualTo(checksum2));
            }
        }

        File.Delete(filePath);
    }

    [Test]
    public void ResumeUploadFileTest()
    {
        var mac = new Mac(AccessKey, SecretKey);
        var rand = new Random();
        var key = string.Format("UploadFileTest_{0}.dat", rand.Next());

        var tempPath = System.IO.Path.GetTempPath();
        var rnd = new Random().Next(1, 100000);
        var filePath = tempPath + "resumeFile" + rnd.ToString();
        var testBody = new char[5 * 1024 * 1024];
        var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
        var sw = new System.IO.StreamWriter(stream, System.Text.Encoding.Default);
        sw.Write(testBody);
        sw.Close();
        stream.Close();
        var fs = System.IO.File.OpenRead(filePath);

        var putPolicy = new PutPolicy();
        putPolicy.Scope = Bucket + ":" + key;
        putPolicy.SetExpires(3600);
        putPolicy.DeleteAfterDays = 1;
        var token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());

        var config = new Config();
        config.UseHttps = true;
        config.Zone = Zone.ZONE_CN_East;
        config.UseCdnDomains = true;
        config.ChunkSize = ChunkUnit.U512K;
        var target = new ResumableUploader(config);
        var extra = new PutExtra();
        //设置断点续传进度记录文件
        extra.ResumeRecordFile = ResumeHelper.GetDefaultRecordKey(filePath, key);
        Console.WriteLine("record file:" + extra.ResumeRecordFile);
        var result = target.UploadStream(fs, key, token, extra);
        Console.WriteLine("resume upload: " + result.ToString());
        Assert.That(result.Code, Is.EqualTo((int)HttpCode.OK));

        var downloadUrl = string.Format("http://{0}/{1}", Domain, key);
        using (var httpClient = new HttpClient())
        using (var httpResp = httpClient.GetAsync(downloadUrl).Result)
        {
            Assert.That((int)httpResp.StatusCode, Is.EqualTo((int)HttpCode.OK));
            Assert.That(httpResp.Content.Headers.ContentType, Is.Not.Null);
            Assert.That(httpResp.Content.Headers.ContentType.MediaType, Is.EqualTo("application/json"));
            using (var md5_1 = MD5.Create())
            using (var md5_2 = MD5.Create())
            using (var fileStream = File.OpenRead(filePath))
            using (var respStream = httpResp.Content.ReadAsStream())
            {
                var checksum1 = md5_1.ComputeHash(fileStream);
                var checksum2 = md5_2.ComputeHash(respStream);
                Assert.That(checksum1, Is.EqualTo(checksum2));
            }
        }

        System.IO.File.Delete(filePath);
    }

    [Test]
    public void ResumeUploadFileV2Test()
    {
        var mac = new Mac(AccessKey, SecretKey);
        var config = new Config();
        config.UseHttps = true;
        config.Zone = Zone.ZONE_CN_East;
        config.UseCdnDomains = true;
        config.ChunkSize = ChunkUnit.U512K;
        var target = new ResumableUploader(config);
        var extra = new PutExtra();
        extra.PartSize = 4 * 1024 * 1024;
        extra.Version = "v2";

        var sizes = new int[5] { extra.PartSize / 2, extra.PartSize, extra.PartSize + 1, extra.PartSize * 2, 10 * 1024 * 1024 };
        foreach (var i in sizes)
        {
            var testBody = new char[i];
            var rand = new Random();
            var key = string.Format("UploadFileTest_{0}.dat", rand.Next());

            var tempPath = System.IO.Path.GetTempPath();
            var rnd = new Random().Next(1, 100000);
            var filePath = tempPath + "resumeFile" + rnd.ToString();
            var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
            var sw = new System.IO.StreamWriter(stream, System.Text.Encoding.Default);
            sw.Write(testBody);
            sw.Close();
            stream.Close();
            var fs = System.IO.File.OpenRead(filePath);

            var putPolicy = new PutPolicy();
            putPolicy.Scope = Bucket + ":" + key;
            putPolicy.SetExpires(3600);
            putPolicy.DeleteAfterDays = 1;
            var token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());

            //设置断点续传进度记录文件
            extra.ResumeRecordFile = ResumeHelper.GetDefaultRecordKey(filePath, key);
            Console.WriteLine("record file:" + extra.ResumeRecordFile);
            var result = target.UploadStream(fs, key, token, extra);
            Console.WriteLine("resume upload: " + result.ToString());
            Assert.That(result.Code, Is.EqualTo((int)HttpCode.OK));

            var downloadUrl = string.Format("http://{0}/{1}", Domain, key);
            using (var httpClient = new HttpClient())
            using (var httpResp = httpClient.GetAsync(downloadUrl).Result)
            {
                Assert.That((int)httpResp.StatusCode, Is.EqualTo((int)HttpCode.OK));
                Assert.That(httpResp.Content.Headers.ContentType, Is.Not.Null);
                Assert.That(httpResp.Content.Headers.ContentType.MediaType, Is.EqualTo("application/json"));
                using (var md5_1 = MD5.Create())
                using (var md5_2 = MD5.Create())
                using (var fileStream = File.OpenRead(filePath))
                using (var respStream = httpResp.Content.ReadAsStream())
                {
                    var checksum1 = md5_1.ComputeHash(fileStream);
                    var checksum2 = md5_2.ComputeHash(respStream);
                    Assert.That(checksum1, Is.EqualTo(checksum2));
                }
            }

            System.IO.File.Delete(filePath);
        }
    }

    [Test]
    public void ResumeUploadFileV2WithoutKeyTest()
    {
        var mac = new Mac(AccessKey, SecretKey);
        var config = new Config();
        config.UseHttps = true;
        config.Zone = Zone.ZONE_CN_East;
        config.UseCdnDomains = true;
        config.ChunkSize = ChunkUnit.U512K;
        var target = new ResumableUploader(config);
        var extra = new PutExtra();
        extra.PartSize = 4 * 1024 * 1024;
        extra.Version = "v2";

        var sizes = new int[5] { extra.PartSize / 2, extra.PartSize, extra.PartSize + 1, extra.PartSize * 2, 10 * 1024 * 1024 };
        foreach (var i in sizes)
        {
            var testBody = new char[i];
            var rand = new Random();

            var tempPath = Path.GetTempPath();
            var rnd = new Random().Next(1, 100000);
            var filePath = tempPath + "resumeFile" + rnd.ToString();
            var stream = new FileStream(filePath, FileMode.Create);
            var sw = new StreamWriter(stream, System.Text.Encoding.Default);
            sw.Write(testBody);
            sw.Close();
            stream.Close();
            var fs = File.OpenRead(filePath);

            var putPolicy = new PutPolicy();
            putPolicy.Scope = Bucket;
            putPolicy.SetExpires(3600);
            putPolicy.DeleteAfterDays = 1;
            var token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());

            //设置断点续传进度记录文件
            extra.ResumeRecordFile = ResumeHelper.GetDefaultRecordKey(filePath, rand.Next().ToString());
            Console.WriteLine("record file:" + extra.ResumeRecordFile);
            var result = target.UploadStream(fs, string.Empty, token, extra);
            Console.WriteLine("resume upload: " + result.ToString());
            Assert.That(result.Code, Is.EqualTo((int)HttpCode.OK));
            var responseBody = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.Text);
            Assert.That(responseBody, Is.Not.Null);
            Assert.That(responseBody.ContainsKey("hash"), Is.True);
            Assert.That(responseBody.ContainsKey("key"), Is.True);
            Assert.That(responseBody["hash"], Is.EqualTo(responseBody["key"]));

            var downloadUrl = string.Format("http://{0}/{1}", Domain, responseBody["key"]);
            using (var httpClient = new HttpClient())
            using (var httpResp = httpClient.GetAsync(downloadUrl).Result)
            {
                Assert.That((int)httpResp.StatusCode, Is.EqualTo((int)HttpCode.OK));
                Assert.That(httpResp.Content.Headers.ContentType, Is.Not.Null);
                Assert.That(httpResp.Content.Headers.ContentType.MediaType, Is.EqualTo("application/json"));
                using (var md5_1 = MD5.Create())
                using (var md5_2 = MD5.Create())
                using (var fileStream = File.OpenRead(filePath))
                using (var respStream = httpResp.Content.ReadAsStream())
                {
                    var checksum1 = md5_1.ComputeHash(fileStream);
                    var checksum2 = md5_2.ComputeHash(respStream);
                    Assert.That(checksum1, Is.EqualTo(checksum2));
                }
            }

            File.Delete(filePath);
        }
    }

}