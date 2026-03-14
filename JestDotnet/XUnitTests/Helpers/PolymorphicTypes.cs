namespace XUnitTests.Helpers;

public interface IAnimal
{
    string Name { get; }
}

public record Dog : IAnimal
{
    public string Name { get; init; } = "";
    public string Breed { get; init; } = "";
    public int Age { get; init; }
}

public record Cat : IAnimal
{
    public string Name { get; init; } = "";
    public bool IsIndoor { get; init; }
}

public record PetOwner
{
    public string OwnerName { get; init; } = "";
    public IAnimal Pet { get; init; } = null!;
}

public abstract class Vehicle
{
    public string Make { get; set; } = "";
}

public class Car : Vehicle
{
    public int Doors { get; set; }
}

public record Garage
{
    public string Location { get; init; } = "";
    public Vehicle Vehicle { get; init; } = null!;
}
