namespace Core;

public class GeneratorContext(Random random, Faker faker) {
    public Random Random { get; } = random;
    public Faker Faker { get; } = faker;
}