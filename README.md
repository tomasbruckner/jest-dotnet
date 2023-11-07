# Jest Snapshot Dotnet
Simple snapshot testing with inspiration from amazing [Jest](https://jestjs.io) library.

## Installation
You can install it using [Nuget](https://www.nuget.org/packages/JestDotnet/).

## How it works
If you are unfamiliar with snapshot testing, I recommend you to check [Jest documentation](https://jestjs.io/docs/en/snapshot-testing).
 
 This library works very similarly. When you run `ShouldMatchSnapshot` for the first time, it will generate JSON snapshot of the serialization of the object.
 If you run it second time, it will check if the JSON serialization of the object is the same as the saved JSON snapshot.
 
 It is important to `commit snapshots` with your code to the git.

### Updating snapshots
You can mass update snapshot (useful when you add new property to an object, change default value or remove a property).
Simply set environment variable `UPDATE` to `true` and run the tests that you want to update. It will update all snapshots that have failed.

If you are using JetBrains Rider, go to `Settings -> Build, Execution, Deployment -> Unit Testing -> Test Runner` and set it there.
Remember to unset the variable again.

### Continuous Integration
Snapshots are not created if you are running tests inside Continuous Integration environment (Gitlab CI, Jenkins, Teamcity, Azure/AWS Pipelines etc.).
Library detects CI environment by check `CI` environment variable. If it is set to `true`, test fails if snapshot is missing.

## API

## Extensions methods
### ShouldMatchSnapshot
```c#
public static void ShouldMatchSnapshot(this object actual, string hint = "");
```

#### Example
```c#
var person = new Person
{
    Age = 13,
    DateOfBirth = new DateTime(2008, 7, 7),
    FirstName = "John",
    LastName = "Bam"
};

person.ShouldMatchSnapshot();
```

### ShouldMatchInlineSnapshot
```c#
public static void ShouldMatchInlineSnapshot(this object actual, string inlineSnapshot);
```

#### Example
```c#
var person = new Person
{
    Age = 13,
    DateOfBirth = new DateTime(2008, 7, 7),
    FirstName = "John",
    LastName = "Bam"
};

person.ShouldMatchInlineSnapshot(@"
    {
        ""FirstName"": ""John"",
        ""LastName"": ""Bam"",
        ""DateOfBirth"": ""2008-07-07T00:00:00"",
        ""Age"": 13,    
    }"
);
```

### ShouldMatchObject
```c#
public static void ShouldMatchObject(this object actual, object expected);
```

#### Example
```c#
var actual = new Person
{
    Age = 13,
    DateOfBirth = new DateTime(2008, 7, 7),
    FirstName = "John",
    LastName = "Bam"
};

var expected = new Person
{
    Age = 13,
    DateOfBirth = new DateTime(2008, 7, 7),
    FirstName = "John",
    LastName = "Bam"
};

actual.ShouldMatchObject(expected);
```

## Methods
If you don't like extension methods, you can use static class `JestAssert`

### ShouldMatchSnapshot
```c#
public static void ShouldMatchSnapshot(object actual, string hint = "");
```

#### Example
```c#
var person = new Person
{
    Age = 13,
    DateOfBirth = new DateTime(2008, 7, 7),
    FirstName = "John",
    LastName = "Bam"
};

JestAssert.ShouldMatchSnapshot(person);
```

### ShouldMatchInlineSnapshot
```c#
public static void ShouldMatchInlineSnapshot(object actual, string inlineSnapshot);
```

#### Example
```c#
var person = new Person
{
    Age = 13,
    DateOfBirth = new DateTime(2008, 7, 7),
    FirstName = "John",
    LastName = "Bam"
};

JestAssert.ShouldMatchInlineSnapshot(person, @"
    {
        ""FirstName"": ""John"",
        ""LastName"": ""Bam"",
        ""DateOfBirth"": ""2008-07-07T00:00:00"",
        ""Age"": 13,    
    }"
);
```

### ShouldMatchObject
```c#
public static void ShouldMatchObject(object actual, object expected);
```

#### Example
```c#
var actual = new Person
{
    Age = 13,
    DateOfBirth = new DateTime(2008, 7, 7),
    FirstName = "John",
    LastName = "Bam"
};

var expected = new Person
{
    Age = 13,
    DateOfBirth = new DateTime(2008, 7, 7),
    FirstName = "John",
    LastName = "Bam"
};

JestAssert.ShouldMatchObject(actual,expected);
```

## Advanced
### Excluding properties
If you want to exclude some properties from the diff, you can use `SnapshotSettings` class to specify your own

* diffing options (use `SnapshotSettings.CreateDiffOptions`)

Example:
```csharp
SnapshotSettings.CreateDiffOptions = () => new JsonDiffOptions
{
    PropertyFilter = (s, context) => s != "LastName"
};
```
or pass `JsonDiffOptions` as optional argument

```csharp
var actual = new Person
{
    Age = 13,
    DateOfBirth = new DateTime(2008, 7, 7),
    FirstName = "John",
    LastName = "Bam"
};

var expected = new Person
{
    Age = 13,
    DateOfBirth = new DateTime(2008, 7, 7),
    FirstName = "John",
    LastName = ""
};

// this does not throw an exception and the test completes successfully
// property "LastName" is ignored from the diff
JestAssert.ShouldMatchObject(actual,expected, new JsonDiffOptions
{
    PropertyFilter = (s, context) => s != "LastName"
});
```

### Configuring directory and file extensions
If you need to configure it, you can use `SnapshotSettings` class to specify your own 
* extension instead of `.snap` (use `SnapshotSettings.SnapshotExtension`)
* directory instead of `__snapshots__` (use `SnapshotSettings.SnapshotDirectory`) 
* function that generates directory, extension and filename (use `SnapshotSettings.CreatePath`)

Popular use is to change directory of the snapshot files. You can do it like this:

```csharp
SnapshotSettings.SnapshotDirectory = "__custom__";

var testObject = new Person
{
    Age = 13,
    DateOfBirth = new DateTime(2008, 7, 7),
    FirstName = "John",
    LastName = "Bam"
};

JestAssert.ShouldMatchSnapshot(testObject);

// you can return it back using
SnapshotSettings.SnapshotDirectory = SnapshotSettings.DefaultSnapshotDirectory;
```

### Configuring serialization
For serialization, I am using Json.NET. If you need to configure it, you can use `SnapshotSettings` class to specify your own 

* `JsonSerializer` (use `SnapshotSettings.CreateJsonSerializer`)
* `JTokenWriter` (use `SnapshotSettings.CreateJTokenWriter`)
* `StringWriter` (use `SnapshotSettings.CreateStringWriter`) 
* `JsonTextWriter` (use `SnapshotSettings.CreateJsonTextWriter`).


#### Change line endings to LF
Popular use is to change line ending of the `.snap` files. For example if you want to set line ending to Linux `LF`, you can do it like this:

```csharp
SnapshotSettings.CreateStringWriter = () => new StringWriter(CultureInfo.InvariantCulture)
{
    NewLine = "\n"
};

var testObject = new Person
{
    Age = 13,
    DateOfBirth = new DateTime(2008, 7, 7),
    FirstName = "John",
    LastName = "Bam"
};

JestAssert.ShouldMatchSnapshot(testObject);
```

`SnapshotSettings` expects you define your own function that returns new configured instance.

#### Sort properties alphabetically
```csharp
SnapshotSettings.CreateJsonSerializer = () =>
{
    var serializer = SnapshotSettings.DefaultCreateJsonSerializer();
    serializer.ContractResolver = new AlphabeticalPropertySortContractResolver();

    return serializer;
};

var testObject = new Person
{
    Age = 13,
    DateOfBirth = new DateTime(2008, 7, 7),
    FirstName = "John",
    LastName = "Bam"
};

JestAssert.ShouldMatchSnapshot(testObject);
```

For Newtonsoft.Json, the resolver can look something like this

```csharp
public sealed class AlphabeticalPropertySortContractResolver : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        var properties = base.CreateProperties(type, memberSerialization);

        return properties.OrderBy(p => p.PropertyName).ToList();
    }
}
```


## Caveats
### Dynamic objects
You cannot call neither extension nor `JestAssert` with `dynamic` object. You need to cast it to `object` (or real type).


## Credits

[Package icon](https://www.flaticon.com/free-icon/joker-hat_68335)
