using System.Collections.Generic;
using System.Linq;
using Core.HexSystem;
using NUnit.Framework;

namespace Tests.Tests
{
    public class HexagonGrid
    {
        [Test]
        public void TestHexRingsAroundCoordinates0Rings()
        {
            var testCoordinates = new AxialCoordinates(0, 0);
            var expectedList = new List<AxialCoordinates>() { testCoordinates };
            
            var resultList = Core.HexSystem.HexagonGrid.GetHexRingsAroundCoordinates(testCoordinates, 0).ToList();
            
            Assert.IsTrue(resultList.Count == expectedList.Count && !expectedList.Except(resultList).Any());
        }
        
        [Test]
        public void TestHexRingsAroundCoordinates1Ring()
        {
            var testCoordinates = new AxialCoordinates(0, 0);
            var expectedList = new List<AxialCoordinates>
            {
                testCoordinates,
                new (0, -1),
                new (+1, -1),
                new (+1, 0),
                new (0, +1),
                new (-1, +1),
                new (-1, 0),
            };
            
            var resultList = Core.HexSystem.HexagonGrid.GetHexRingsAroundCoordinates(testCoordinates, 1).ToList();
            
            Assert.IsTrue(resultList.Count == expectedList.Count && !expectedList.Except(resultList).Any());
        }
        
        [Test]
        public void TestHexRingsAroundCoordinates1RingTranslated()
        {
            var testCoordinates = new AxialCoordinates(1, 1);
            var expectedList = new List<AxialCoordinates>
            {
                testCoordinates,
                new (+1, 0),
                new (+2, 0),
                new (+2, +1),
                new (+1, +2),
                new (0, +2),
                new (0, +1),
            };
            
            var resultList = Core.HexSystem.HexagonGrid.GetHexRingsAroundCoordinates(testCoordinates, 1).ToList();
            
            Assert.IsTrue(resultList.Count == expectedList.Count && !expectedList.Except(resultList).Any());
        }
        
        [Test]
        public void TestHexRingsAroundCoordinates2Rings()
        {
            var testCoordinates = new AxialCoordinates(0, 0);
            var expectedList = new List<AxialCoordinates>
            {
                testCoordinates,
                new (0, -1),
                new (+1, -1),
                new (+1, 0),
                new (0, +1),
                new (-1, +1),
                new (-1, 0),
                new (0, -2),
                new (+1, -2),
                new (+2, -2),
                new (+2, -1),
                new (+2, 0),
                new (+1, +1),
                new (0, +2),
                new (-1, +2),
                new (-2, +2),
                new (-2, +1),
                new (-2, 0),
                new (-1, -1),
            };
            
            var resultList = Core.HexSystem.HexagonGrid.GetHexRingsAroundCoordinates(testCoordinates, 2).ToList();
            
            Assert.IsTrue(resultList.Count == expectedList.Count && !expectedList.Except(resultList).Any());
        }
    }
}
