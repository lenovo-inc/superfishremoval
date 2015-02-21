using System;

namespace SuperFishRemovalTool.Test.Mocks
{
    class IsAMockException : Exception
    {
        public IsAMockException() : base("The current mock is not valid for this operation.") { }
    }
}
