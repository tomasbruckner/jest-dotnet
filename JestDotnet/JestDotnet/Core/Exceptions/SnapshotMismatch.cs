using System;

namespace JestDotnet.Core.Exceptions
{
    public class SnapshotMismatch : Exception
    {
        public SnapshotMismatch(string message) : base(message)
        {
        }
    }
}
