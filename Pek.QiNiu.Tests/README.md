# Pek.QiNiu.Tests - 七牛（云）SDK测试项目

## 项目简介

这是基于NUnit的七牛云SDK测试项目，用于对`Pek.QiNiu`和`Pek.QiNiu.Extensions`项目进行单元测试。

## 环境要求

- .NET 6.0 或更高版本
- NUnit 4.0.1
- NUnit3TestAdapter 4.5.0
- Moq 4.20.70

## 项目结构

- `Extensions/` - 七牛云扩展功能测试
- `Storage/` - 存储相关功能测试
- `Util/` - 工具类测试
- `TestHelpers/` - 测试辅助类

## 运行测试

### 使用Visual Studio

1. 在Visual Studio中打开解决方案
2. 打开"测试资源管理器"窗口（测试 > 测试资源管理器）
3. 点击"运行所有测试"按钮

### 使用命令行

```bash
# 运行所有测试
dotnet test Pek.QiNiu.Tests

# 运行特定测试
dotnet test Pek.QiNiu.Tests --filter "FullyQualifiedName~Pek.QiNiu.Tests.QiniuCSharpSDKTests"
```

## 添加新测试

1. 在相应的目录下创建新的测试类
2. 使用`[TestFixture]`特性标记测试类
3. 使用`[Test]`特性标记测试方法
4. 使用`Assert`类进行断言

示例：

```csharp
using NUnit.Framework;

namespace Pek.QiNiu.Tests
{
    [TestFixture]
    public class NewFeatureTests
    {
        [Test]
        public void TestMethod_ShouldBehaveAsExpected()
        {
            // Arrange
            var expected = true;
            
            // Act
            var actual = true;
            
            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
```
