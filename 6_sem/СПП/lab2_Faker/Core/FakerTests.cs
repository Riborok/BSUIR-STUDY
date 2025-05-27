using Core.ValueGenerator;
using NUnit.Framework;

namespace Core;

[TestFixture]
public class FakerTests
{
    private Faker faker;

    [SetUp]
    public void Setup()
    {
        faker = new Faker();
    }

    [Test]
    public void CreateInt_ReturnsNonDefaultInt()
    {
        int i = faker.Create<int>();
        Assert.That(i, Is.Not.EqualTo(0));
    }

    [Test]
    public void CreateDouble_ReturnsNonDefaultDouble()
    {
        double d = faker.Create<double>();
        Assert.That(d, Is.Not.EqualTo(0.0));
    }

    [Test]
    public void CreateString_ReturnsNonEmptyString()
    {
        string s = faker.Create<string>();
        Assert.That(s, Is.Not.Null.Or.Empty);
    }

    [Test]
    public void CreateDateTime_ReturnsValidDateTime()
    {
        DateTime dt = faker.Create<DateTime>();
        Assert.That(dt.Year, Is.InRange(1, DateTime.Now.Year));
    }

    [Test]
    public void CreateBool_ReturnsBool()
    {
        bool b = faker.Create<bool>();
        Assert.That(b, Is.InstanceOf<bool>());
    }

    [Test]
    public void CreateFloat_ReturnsNonDefaultFloat()
    {
        float f = faker.Create<float>();
        Assert.That(f, Is.Not.EqualTo(0f));
    }

    [Test]
    public void CreateLong_ReturnsNonDefaultLong()
    {
        long l = faker.Create<long>();
        Assert.That(l, Is.Not.EqualTo(0L));
    }

    [Test]
    public void CreateListOfInt_ReturnsNonEmptyList()
    {
        List<int> list = faker.Create<List<int>>();
        Assert.That(list, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public void CreateArrayOfInt_ReturnsNonEmptyArray()
    {
        int[] array = faker.Create<int[]>();
        Assert.That(array, Is.Not.Null.And.Not.Empty);
    }

    public class TestClass
    {
        public string StringProp { get; set; }
        public int IntProp { get; set; }
    }

    [Test]
    public void CreateCustomObject_ByReflectionWorks()
    {
        TestClass obj = faker.Create<TestClass>();
        Assert.That(obj, Is.Not.Null);
        Assert.That(obj.StringProp, Is.Not.Null.Or.Empty);
        Assert.That(obj.IntProp, Is.Not.EqualTo(0));
    }

    public class ClassA { public ClassB B { get; set; } }
    public class ClassB { public ClassC C { get; set; } }
    public class ClassC { public ClassA A { get; set; } }

    [Test]
    public void CreateObjectWithCyclicDependency_DoesNotCauseStackOverflow()
    {
        ClassA a = faker.Create<ClassA>();
        Assert.That(a, Is.Not.Null);
        Assert.That(a.B, Is.Not.Null);
        Assert.That(a.B.C, Is.Not.Null);
        Assert.That(a.B.C.A, Is.Null);
    }

    public class Foo { public string City { get; set; } }

    public class CustomCityGenerator : IValueGenerator
    {
        public bool CanGenerate(Type type) => type == typeof(string);
        public object Generate(Type type, GeneratorContext context) => "TestCity";
    }

    [Test]
    public void CreateCustomConfiguredObject_ReturnsConfiguredValue()
    {
        FakerConfig config = new FakerConfig();
        config.Add<Foo, string, CustomCityGenerator>(x => x.City);
        Faker customFaker = new Faker(config);
        Foo foo = customFaker.Create<Foo>();
        Assert.That(foo.City, Is.EqualTo("TestCity"));
    }
}