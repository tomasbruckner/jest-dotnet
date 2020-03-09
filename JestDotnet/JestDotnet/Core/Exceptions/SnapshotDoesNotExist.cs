using System;

namespace JestDotnet.Core.Exceptions
{
    public class SnapshotDoesNotExist : Exception
    {
        public SnapshotDoesNotExist(string message) : base(message)
        {
        }
    }
}
