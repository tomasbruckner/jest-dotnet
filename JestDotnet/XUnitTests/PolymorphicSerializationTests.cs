using JestDotnet;
using Xunit;
using XUnitTests.Helpers;

namespace XUnitTests;

public class PolymorphicSerializationTests
{
    [Fact]
    public void ShouldSerializeRuntimeTypeProperties()
    {
        var owner = new PetOwner
        {
            OwnerName = "John",
            Pet = new Dog
            {
                Name = "Rex",
                Breed = "Labrador",
                Age = 5,
            },
        };

        JestAssert.ShouldMatchInlineSnapshot(
            owner,
            @"
{
  ""OwnerName"": ""John"",
  ""Pet"": {
    ""Age"": 5,
    ""Breed"": ""Labrador"",
    ""Name"": ""Rex""
  }
}"
        );
    }

    [Fact]
    public void ShouldSerializeAbstractTypeProperties()
    {
        var vehicle = new Garage
        {
            Location = "Main Street",
            Vehicle = new Car
            {
                Make = "Toyota",
                Doors = 4,
            },
        };

        JestAssert.ShouldMatchInlineSnapshot(
            vehicle,
            @"
{
  ""Location"": ""Main Street"",
  ""Vehicle"": {
    ""Doors"": 4,
    ""Make"": ""Toyota""
  }
}"
        );
    }
}
