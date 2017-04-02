using NUnit.Framework;
using System.Collections.Generic;
using InformationFinder;
using Test.Properties;

namespace Tests
{
    [TestFixture]
    public class InfoExtractorTest
    {
        private string _rightLinksAndEmails;
        private List<string> _testData;
        private string _testText;

        [SetUp]
        public void Setup()
        {
            var tempList = new SortedSet<string>() {
                 "http://www.youtube.com",
                "www.youtube.com",
                "ivansila@gmail.com",
                "alexashka2551ywna@yandex.com"
            };

            _rightLinksAndEmails = string.Concat(tempList);

            _testText = Resources.testInfo;
        }

        public void TestObserver(object sender, MatchFondedEventArgs e)
        {
            _testData.Add(e.Founded);
        }

        [Test]
        public void InfoFindingTest()
        {
            //Arrange
            _testData = new List<string>();
            var infoExtractor = InfoExtractor.CreateFromText(_testText);
            infoExtractor.MatchFounded += TestObserver;

            //Act
            infoExtractor.SearchEmails();
            infoExtractor.SearchLinks();

            infoExtractor.MatchFounded -= TestObserver;

            _testData.Sort();

            var testDataString = string.Concat(_testData);

            //Assert
            Assert.AreEqual(_rightLinksAndEmails, testDataString);
        }
    }
}