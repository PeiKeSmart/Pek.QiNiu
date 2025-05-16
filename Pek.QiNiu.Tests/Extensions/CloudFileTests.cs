using NUnit.Framework;
using Pek.QiNiu.Extensions;
using System.Text.Json;

namespace Pek.QiNiu.Tests.Extensions
{
    /// <summary>
    /// 云文件测试类
    /// </summary>
    [TestFixture]
    public class CloudFileTests
    {
        [Test]
        public void CloudFile_DefaultProperties_ShouldHaveCorrectValues()
        {
            // Arrange & Act
            var cloudFile = new CloudFile();

            // Assert
            Assert.That(cloudFile.Code, Is.EqualTo(200));
            Assert.That(cloudFile.Message, Is.Null);
            Assert.That(cloudFile.Page, Is.EqualTo(""));
            Assert.That(cloudFile.Token, Is.Null);
            Assert.That(cloudFile.list, Is.Null);
        }

        [Test]
        public void CloudFile_Serialization_ShouldWorkCorrectly()
        {
            // Arrange
            var cloudFile = new CloudFile
            {
                Code = 200,
                Message = "成功",
                Page = "test/",
                Token = "test-token",
                list = new List<ListInfo>
                {
                    new ListInfo
                    {
                        Name = "test.jpg",
                        Size = 1024,
                        Type = "image/jpeg",
                        Time = new DateTime(2023, 1, 1)
                    }
                }
            };

            // Act
            string json = JsonSerializer.Serialize(cloudFile);
            var deserializedCloudFile = JsonSerializer.Deserialize<CloudFile>(json);

            // Assert
            Assert.That(deserializedCloudFile, Is.Not.Null);
            Assert.That(deserializedCloudFile!.Code, Is.EqualTo(cloudFile.Code));
            Assert.That(deserializedCloudFile.Message, Is.EqualTo(cloudFile.Message));
            Assert.That(deserializedCloudFile.Page, Is.EqualTo(cloudFile.Page));
            Assert.That(deserializedCloudFile.Token, Is.EqualTo(cloudFile.Token));
            Assert.That(deserializedCloudFile.list, Is.Not.Null);
            Assert.That(deserializedCloudFile.list!.Count, Is.EqualTo(1));
            Assert.That(deserializedCloudFile.list[0].Name, Is.EqualTo("test.jpg"));
            Assert.That(deserializedCloudFile.list[0].Size, Is.EqualTo(1024));
            Assert.That(deserializedCloudFile.list[0].Type, Is.EqualTo("image/jpeg"));
        }
    }
}
