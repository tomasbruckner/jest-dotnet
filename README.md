# Jest Snapshot Dotnet

[![CI](https://github.com/tomasbruckner/jest-dotnet/actions/workflows/ci.yml/badge.svg)](https://github.com/tomasbruckner/jest-dotnet/actions/workflows/ci.yml)

Simple snapshot testing with inspiration from amazing [Jest](https://jestjs.io) library.

## Installation
You can install it using [NuGet](https://www.nuget.org/packages/JestDotnet/).

```
dotnet add package JestDotnet
```

### Supported frameworks

| Target | Version |
|--------|---------|
| .NET | 10.0 |

## How it works
If you are unfamiliar with snapshot testing, I recommend you to check [Jest documentation](https://jestjs.io/docs/en/snapshot-testing).

This library works very similarly. When you run `ShouldMatchSnapshot` for the first time, it will generate JSON snapshot of the serialization of the object.
If you run it second time, it will check if the JSON serialization of the object is the same as the saved JSON snapshot.

It is important to `commit snapshots` with your code to the git.

### Updating snapshots
You can mass update snapshot (useful when you add new property to an object, change default value or remove a property).
Simply set environment variable `UPDATE` to `true` and run the tests that you want to update. It will update all snapshots that have failed.

```bash
UPDATE=true dotnet test
```

If you are using JetBrains Rider, go to `Settings -> Build, Execution, Deployment -> Unit Testing -> Test Runner` and set it there.
Remember to unset the variable again.

### Continuous Integration
Snapshots are not created if you are running tests inside Continuous Integration environment (GitHub Actions, GitLab CI, Jenkins, Teamcity, Azure/AWS Pipelines etc.).
Library detects CI environment by checking the `CI` environment variable. If it is set to `true`, test fails if snapshot is missing.

## API

## Extension methods
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

## Static methods
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

JestAssert.ShouldMatchObject(actual, expected);
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
JestAssert.ShouldMatchObject(actual, expected, new JsonDiffOptions
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
For serialization, System.Text.Json is used. By default, snapshots are written with indented formatting, non-ASCII characters are preserved as literal UTF-8, HTML-sensitive characters (`<`, `>`, `&`) are not escaped (using `JavaScriptEncoder.UnsafeRelaxedJsonEscaping`), and double quotes inside string values are escaped as `\"` instead of `\u0022`.

If you need to configure it, you can use `SnapshotSettings` class to specify your own

* `JsonSerializerOptions` (use `SnapshotSettings.CreateSerializerOptions`)

#### Change line endings to LF
Popular use is to change line ending of the `.snap` files. For example if you want to set line ending to Linux `LF`, you can do it like this:

```csharp
SnapshotSettings.NewLine = "\n";

var testObject = new Person
{
    Age = 13,
    DateOfBirth = new DateTime(2008, 7, 7),
    FirstName = "John",
    LastName = "Bam"
};

JestAssert.ShouldMatchSnapshot(testObject);
```

#### Sorting

Properties are sorted alphabetically by default using ordinal string comparison (`AlphabeticalSortModifier.SortProperties`). This ensures deterministic, culture-independent snapshot output regardless of property declaration order.

> **Note:** When overriding `CreateSerializerOptions`, include `AlphabeticalSortModifier.SortProperties` in your `TypeInfoResolver` modifiers to keep the default sorting behavior. Also include `Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping` to keep non-ASCII and HTML characters readable and `ReferenceHandler = ReferenceHandler.IgnoreCycles` to handle circular references.

#### Circular references

Circular references are handled by default via `ReferenceHandler.IgnoreCycles` — cyclic references are serialized as `null` instead of throwing.

### Using Newtonsoft.Json types

If your codebase uses Newtonsoft.Json types (`JObject`, `JArray`, `JToken`), System.Text.Json cannot serialize them correctly out of the box. Register pre-serializers to handle them:

```csharp
// In test setup or assembly initializer
SnapshotSettings.AddPreSerializer<JObject>(obj => obj.ToString());
SnapshotSettings.AddPreSerializer<JArray>(obj => obj.ToString());
```

Then snapshot testing works as expected:

```csharp
var json = JObject.Parse("{\"name\": \"Alice\", \"age\": 30}");
json.ShouldMatchSnapshot();
```

### Custom type serialization (pre-serializers)

The pre-serializer hook is not limited to Newtonsoft — you can register a custom serializer for any type that System.Text.Json cannot handle:

```csharp
SnapshotSettings.AddPreSerializer<MyCustomType>(obj => obj.ToJson());
```

The pre-serializer function should return a valid JSON string. The output is re-serialized through System.Text.Json to ensure consistent formatting. Keys are sorted alphabetically in the final output, regardless of the order returned by the pre-serializer.

To remove all registered pre-serializers (e.g., for test cleanup):

```csharp
SnapshotSettings.ClearPreSerializers();
```

## Caveats
### Dynamic objects
You cannot call neither extension nor `JestAssert` with `dynamic` object. You need to cast it to `object` (or real type).


## Credits

[Package icon](https://www.flaticon.com/free-icon/joker-hat_68335)
