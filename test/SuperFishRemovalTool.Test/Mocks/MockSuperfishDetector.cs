using SuperFishRemovalTool.Utilities;

namespace SuperFishRemovalTool.Test.Mocks
{
    class MockSuperfishDetector : ISuperfishDetector
    {
        public string MockName { get; set; }
        public bool MockDoesExistReturnValue { get; set; }
        public bool MockRemoveReturnValue { get; set; }

        public bool DoesExistShouldThrow { get; set; }
        public bool RemoveShouldThrow { get; set; }

        public string UtilityName { get {
                if (NameShouldThrow)
                    throw new IsAMockException();
                return MockName;
            } }

        public bool NameShouldThrow { get; set; }

        public bool DoesExist()
        {
            if (DoesExistShouldThrow)
                throw new IsAMockException();

            return MockDoesExistReturnValue;
        }

        public bool Remove()
        {
            if (RemoveShouldThrow)
                throw new IsAMockException();

            return MockRemoveReturnValue;
        }
    }
}
