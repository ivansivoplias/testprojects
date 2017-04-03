using NUnit.Framework;
using System.Collections.Generic;
using InformationFinder;

namespace Tests
{
    [TestFixture]
    public class InfoExtractorTest
    {
        private SortedSet<string> _expectedResult;
        private SortedSet<string> _testData;

        [SetUp]
        public void Setup()
        {
            _expectedResult = new SortedSet<string>() {
                 "http://www.youtube.com",
                "www.youtube.com",
                "ivansila@gmail.com",
                "alexashka2551ywna@yandex.com",
                "ddh@next.site.org"
            };
        }

        public void TestObserver(object sender, MatchFondedEventArgs e)
        {
            _testData.Add(e.Founded);
        }

        [Test]
        public void InfoFindingTest()
        {
            //Arrange
            _testData = new SortedSet<string>();
            var infoExtractor = InfoExtractor.CreateFromText(Test.Properties.Resources.testInfo);
            infoExtractor.MatchFounded += TestObserver;

            //Act
            infoExtractor.SearchEmails();
            infoExtractor.SearchLinks();

            infoExtractor.MatchFounded -= TestObserver;

            //Assert
            Assert.AreEqual(_expectedResult, _testData);
        }
    }
}