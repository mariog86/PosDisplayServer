using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Pos.DisplayServer.Gui.AcceptanceTest
{
    /// <summary>
    /// Summary description for CodedUITest1
    /// </summary>
    [CodedUITest]
    public class SpecificationAcceptanceTest
    {
        [TestMethod]
        public void GivenClientAvailable_WhenSendingMessage_ThenDisplayed()
        {
            Post("CHF 10.-");
            var entry = (AutomationElement)UIMap.UIMainWindowWindow.UIListBoxList.Items[0].NativeElement;

            Assert.AreEqual("127.0.0.1: CHF 10.00", entry.Current.Name);
        }

        public void Post(string data, string uri = "http://localhost:6740")
        {
            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("login", "abc")
            };

            var content = new FormUrlEncodedContent(pairs);

            var client = new HttpClient { BaseAddress = new Uri(uri) };

            // call sync
            var response = client.PostAsync("/api/membership/exist", content).Result;
            if (response.IsSuccessStatusCode)
            {
            }
        }

        [TestInitialize]
        public void MyTestInitialize()
        {
            applicationUnderTest = ApplicationUnderTest.Launch(ApplicationPath);
        }

        [TestCleanup]
        public void MyTestCleanup()
        {
            applicationUnderTest.Close();
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        private static readonly string ApplicationPath = Assembly.GetAssembly(typeof (App)).Location;
        private ApplicationUnderTest applicationUnderTest;

        public UIMap UIMap => map ?? (map = new UIMap());

        private UIMap map;
    }
}
