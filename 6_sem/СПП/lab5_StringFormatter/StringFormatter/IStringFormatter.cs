namespace StringFormatter;

public interface IStringFormatter
{
    string Format(string template, object target);
}