using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Carlabs.Getit.UnitTests
{
    [TestClass]
    public class GetitTests
    {
        [TestMethod]
        public void SetConfig_NullSetConfig_ThrowsException()
        {
            // Arrange
            Getit getit = new Getit();

            // Assert
            Assert.ThrowsException<ArgumentNullException>(() => getit.Config = null);
        }

        [TestMethod]
        public void SetConfig_NullParamConfig_ThrowsException()
        {
            // Arrange / Assert
            Assert.ThrowsException<ArgumentNullException>(() => new Getit(null));
        }

        [TestMethod]
        public void SetConfig_ValidConfig_ReturnsConfig()
        {
            // Arrange
            IConfig expectedConfig = new Config("http://haystack.calhoon.com");

            //Act
            IGetit getit = new Getit(expectedConfig);

            // Assert
            Assert.AreEqual(expectedConfig, getit.Config);
        }

        [TestMethod]
        public async Task Get_NullConfig_ThrowsException()
        {
            // Arrange
            IGetit getit = new Getit();
            IQuery query = new Query();

            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await getit.Get<String>(query));
        }

        [TestMethod]
        public void Query_Called_ReturnsTypeQuery()
        {
            // Arrange
            IGetit getit = new Getit();

            // Act
            IQuery query = getit.Query();

            // Assert
            Assert.AreEqual(QueryType.Query, query.Type);
        }

        [TestMethod]
        public void Mutation_Called_ReturnsTypeMutation()
        {
            // Arrange
            IGetit getit = new Getit();

            // Act
            IQuery query = getit.Mutation();

            // Assert
            Assert.AreEqual(QueryType.Mutation, query.Type);
        }

        [TestMethod]
        public async Task Get_ValidConfig_ReturnsJson()
        {
            // Arrange
            IGetit getit = Substitute.For<IGetit>();
            IQuery query = new Query();
            query.Raw("{Version}");
            getit.Get<String>(query).ReturnsForAnyArgs(@"{""Version"": ""1234""}");

            // Act
            String jsonResults = await getit.Get<String>(query);

            // Assert
            Assert.AreEqual(@"{""Version"": ""1234""}", jsonResults);
        }
    }
}
