using System;

namespace XUnitTests.Helpers
{
    public class PersonRecursion
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public PersonRecursion Parent { get; set; }
    }
}
