using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UITesting;
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
            Post("CHF 10.00");
            var entry = (AutomationElement)UIMap.UIMainWindowWindow.UIListBoxList.Items[0].NativeElement;

            Assert.AreEqual("127.0.0.1: CHF 10.00", entry.Current.Name);
        }

        [TestMethod]
        public void GivenClientAvailable_WhenSendingOtherMessage_ThenDisplayed()
        {
            Post("CHF 15.00");
            var entry = (AutomationElement)UIMap.UIMainWindowWindow.UIListBoxList.Items[0].NativeElement;

            Assert.AreEqual("127.0.0.1: CHF 15.00", entry.Current.Name);
        }

        public void Post(string data, string hostname = "localhost", int port = 6740)
        {
            TcpClient client = new TcpClient();

            client.Connect(hostname, port);
            
            using (var writer = new StreamWriter(client.GetStream(), Encoding.UTF8))
            {
                writer.WriteLine(data);
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
