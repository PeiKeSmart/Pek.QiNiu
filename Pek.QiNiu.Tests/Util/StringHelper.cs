using NUnit.Framework;

namespace Pek.QiNiu.Tests.Util;

[TestFixture]
public class StringHelperTests : TestEnv
{
    [TestCaseSource(typeof(CanonicalMimeHeaderKeyDataClass), nameof(CanonicalMimeHeaderKeyDataClass.TestCases))]
    public string CanonicalMimeHeaderKeyTest(string fieldName)
    {
        return StringHelper.CanonicalMimeHeaderKey(fieldName);
    }
}