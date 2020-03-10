# Jest Snapshot Dotnet
Simple snapshot testing with inspiration from amazing [Jest](https://jestjs.io) library.

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
