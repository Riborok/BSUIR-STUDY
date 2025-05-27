namespace Tests;

public class StringFormatterTests
{
    class User(string firstName, string lastName, int[] orders) {
        public string FirstName { get; } = firstName;
        public string LastName { get; } = lastName;
        public int[] Orders { get; } = orders;
    }
    
    private StringFormatter.StringFormatter _shared;
    private User _user;

    [SetUp]
    public void Setup()
    {
        _shared = StringFormatter.StringFormatter.Shared;
        _user = new User(
            firstName: "John",
            lastName: "Doe",
            orders: [1001, 1002, 1003]
        );
    }

    [Test]
    public void Format_SimpleProperty_ReturnsFirstName()
    {
        var result = _shared.Format("Name: {FirstName}", _user);
        Assert.That(result, Is.EqualTo("Name: John"));
    }

    [Test]
    public void Format_MultipleProperties_ReturnsFullName()
    {
        var result = _shared.Format("Full name: {FirstName} {LastName}", _user);
        Assert.That(result, Is.EqualTo("Full name: John Doe"));
    }

    [Test]
    public void Format_ArrayAccess_ReturnsOrderNumber()
    {
        var result = _shared.Format("First order: {Orders[0]}", _user);
        Assert.That(result, Is.EqualTo("First order: 1001"));
    }

    [Test]
    public void Format_ComplexTemplate_ReturnsFormattedString()
    {
        var result = _shared.Format(
            "Customer {LastName}, {FirstName} - Order #{Orders[1]}", 
            _user
        );
        Assert.That(result, Is.EqualTo("Customer Doe, John - Order #1002"));
    }
}