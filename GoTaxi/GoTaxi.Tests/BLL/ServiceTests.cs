using GoTaxi.BLL.Services;
using GoTaxi.DAL.Models;

namespace GoTaxi.Tests.GoTaxi.BLL.NUnitTests
{
    public class ServiceTests
    {
        [Test]
        public void TestCalculateDistance()
        {
            // Arrange
            Location location1 = new Location(-74.0060, 40.7128); // Example location
            Location location2 = new Location(-118.2437, 34.0522); // Example location

            double expectedResult = 3935.746;

            // Act
            double actualResult = DistanceCalculator.CalculateDistance(location1, location2);

            // Assert
            Assert.That(actualResult, Is.EqualTo(expectedResult).Within(0.1));
        }

        [Test]
        public void TestDegreesToRadiansWithNinetyResultsPiDividedByTwo()
        {
            // Arrange
            double expectedResult = Math.PI / 2;

            // Act
            double actualResult = DistanceCalculator.DegreesToRadians(90);

            // Assert
            Assert.That(actualResult, Is.EqualTo(expectedResult).Within(0.0001));
        }
    }
}