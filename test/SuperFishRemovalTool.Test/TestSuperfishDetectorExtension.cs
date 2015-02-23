using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperFishRemovalTool.Test.Mocks;
using SuperFishRemovalTool.Utilities;
using System;

namespace SuperFishRemovalTool.Test
{
    [TestClass]
    public class TestSuperfishDetectorExtension
    {
        private MockSuperfishDetector GetMock()
        {
            return new MockSuperfishDetector();
        }

        private FixResult Remove(ISuperfishDetector detector)
        {
            return SuperfishDetectorExtension.RemoveItem(detector);
        }

        /// <summary>
        /// Although the functionality is an extension method, best to avoid the obvious.
        /// </summary>
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ProvidingNullThrowsException()
        {
            SuperfishDetectorExtension.RemoveItem(null);
        }

        [TestMethod]
        public void NameIsPropagated()
        {
            const string expected = "HelloWorld";
            var m = GetMock();
            m.MockName = expected;
            var ret = Remove(m);
            Assert.AreEqual(expected, ret.NameOfItem);
        }

        [TestMethod]
        public void NullNameIsPropagatedAsEmptyString()
        {
            const string expected = "";
            var m = GetMock();
            m.MockName = null;
            var ret = Remove(m);
            Assert.AreEqual(expected, ret.NameOfItem);
        }

        [TestMethod]
        public void NameExceptionIsPropagatedAsEmptyString()
        {
            const string expected = "";
            var m = GetMock();
            m.NameShouldThrow = true;
            var ret = Remove(m);
            Assert.AreEqual(expected, ret.NameOfItem);
        }

        [TestMethod]
        public void NameExceptionIsLogged()
        {
            Assert.Inconclusive("Logging discussion required.");
        }

        [TestMethod]
        public void ItemExistingAndRemovedProdicesNoError()
        {
            var m = GetMock();
            m.MockDoesExistReturnValue = true;
            m.MockRemoveReturnValue = true;
            var ret = Remove(m);
            Assert.IsTrue(ret.DidExist);
            Assert.IsTrue(ret.WasRemoved);
            Assert.IsFalse(ret.DidFail);
            Assert.Inconclusive("");
        }

        [TestMethod]
        public void ItemExistingAndNotRemovedProdicesNoError()
        {
            var m = GetMock();
            m.MockDoesExistReturnValue = true;
            m.MockRemoveReturnValue = false;
            var ret = Remove(m);
            Assert.IsTrue(ret.DidExist);
            Assert.IsFalse(ret.WasRemoved);
            Assert.IsFalse(ret.DidFail);
        }

        [TestMethod]
        public void ExistenceExceptionMeansRemoveValueIsNotPropagated()
        {
            var m = GetMock();
            m.DoesExistShouldThrow = true;
            m.MockDoesExistReturnValue = true;
            m.MockRemoveReturnValue = true;
            var ret = Remove(m);
            Assert.IsFalse(ret.DidExist);
            Assert.IsFalse(ret.WasRemoved);
            Assert.IsTrue(ret.DidFail);
        }

        [TestMethod]
        public void RremovalThowingGivesCorectValueForExistence()
        {
            var m = GetMock();
            m.RemoveShouldThrow = true;
            m.MockDoesExistReturnValue = true;
            m.MockRemoveReturnValue = true;
            var ret = Remove(m);
            Assert.IsTrue(ret.DidExist);
            Assert.IsFalse(ret.WasRemoved);
            Assert.IsTrue(ret.DidFail);
        }

        [TestMethod]
        public void ExistenceReturningFalseShouldNotRemove()
        {
            var m = GetMock();
            m.MockDoesExistReturnValue = false;
            m.MockRemoveReturnValue = false;
            var ret = Remove(m);
            Assert.IsFalse(ret.DidFail);
            Assert.Inconclusive("Needs Discussion on whether Remove() should be called on a false DoesExist.");
            Assert.IsFalse(ret.DidExist);
            Assert.IsFalse(ret.WasRemoved);

        }


    }
}
