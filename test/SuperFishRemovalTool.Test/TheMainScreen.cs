using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace SuperFishRemovalTool.Test
{
    [TestClass]
    public class TheMainScreen
    {
        [TestMethod]
        public void TheMainScreen_ShouldCalculateOverallResultCorrectly()
        {
            var orderedResults = new List<OverallResult>()
            {
                OverallResult.ItemsFoundAndRemoved,
                OverallResult.NoItemsFound,
                OverallResult.ItemsFoundAndRemoved,
                OverallResult.ItemsFoundAndRemoved,
                OverallResult.NoItemsFound,
            };

            var overallResult = OverallResult.None;
            foreach(var result in orderedResults)
            {
                overallResult = MainScreen.CalculateMergedResult(overallResult, result);
            }

            Assert.AreEqual(OverallResult.ItemsFoundAndRemoved, overallResult);
            //MainScreen.CalculateMergedResult()
        }
    }
}
