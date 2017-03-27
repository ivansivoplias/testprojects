using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using InformationFinder;

namespace Tests
{
    [TestFixture]
    public class InfoExtractorTest
    {
        private string _rightLinksAndEmails;
        private List<string> _testData;
        private string _testFilename;

        [SetUp]
        public void Setup()
        {
            var tempList = new List<string>() {
                 "http://www.youtube.com",
                "www.youtube.com",
                "/youtube.com",
                "ivansila@gmail.com",
                "alexashka2551ywna@yandex.com"
            };

            tempList.Sort();

            _rightLinksAndEmails = string.Join("", tempList);

            _testFilename = GetCurrentDirectory() + "\\Resources\\testInfo.txt";
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
            var infoExtractor = InfoExtractor.Create(_testFilename);
            infoExtractor.MatchFounded += TestObserver;

            //Act
            infoExtractor.SearchEmails();
            infoExtractor.SearchLinks();

            infoExtractor.MatchFounded -= TestObserver;

            _testData.Sort();

            var testDataString = string.Join("", _testData);

            //Assert
            Assert.AreEqual(_rightLinksAndEmails, testDataString);
        }

        public string GetCurrentDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}