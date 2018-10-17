using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Carlabs.Getit.UnitTests
{
    [TestClass]
    public class ConfigTests
    {
        [TestMethod]
        public void SetUrl_NullUrl_ThrowsException()
        {
            // Arrange
            Config config = new Config();

            // Assert
            Assert.ThrowsException<ArgumentException>(() => config.SetUrl(null));
        }

        [TestMethod]
        public void SetUrl_EmptyUrl_ThrowsException()
        {
            // Arrange
            Config config = new Config();

            // Assert
            Assert.ThrowsException<ArgumentException>(() => config.SetUrl(" "));
        }

        [TestMethod]
        public void SetUrl_ValidUrl_ReturnsUrl()
        {
            // Arrange
            Config config = new Config();
            const string url = "https://randy.butternubs.com/graphql";

            // Act
            config.SetUrl(url);

            // Assert
            Assert.AreEqual(url, config.Url);
        }
    }
}
