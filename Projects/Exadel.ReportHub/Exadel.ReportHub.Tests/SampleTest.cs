namespace Exadel.ReportHub.Tests;

[TestFixture]
public class SampleTest
{
    [Test]
    public void AlwaysPasses()
    {
        var a = 5 + 3;
        var b = a;

        Assert.That(b, Is.EqualTo(a));
    }
}
