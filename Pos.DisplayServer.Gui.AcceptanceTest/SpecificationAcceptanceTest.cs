using System.Reflection;
using System.Threading;
using System.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pos.Test.Helper;

namespace Pos.DisplayServer.Gui.AcceptanceTest
{
    [CodedUITest]
    public class SpecificationAcceptanceTest
    {
        private static readonly string ApplicationPath = Assembly.GetAssembly(typeof(App)).Location;

        private UIMap map;

        private ApplicationUnderTest applicationUnderTest;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        public UIMap UIMap => this.map ?? (this.map = new UIMap());

        [TestMethod]
        public void GivenClientAvailable_WhenSendingMessage_ThenDisplayed()
        {
            TestHelper.Post("CHF 10.00");
            var entry = (AutomationElement) this.UIMap.UIMainWindowWindow.UIItemsListBoxList.Items[0].NativeElement;

            Assert.AreEqual("127.0.0.1: CHF 10.00", entry.Current.Name);
        }

        [TestMethod]
        public void GivenClientAvailable_WhenSendingOtherMessage_ThenDisplayed()
        {
            TestHelper.Post("CHF 15.00");
            var entry = (AutomationElement) this.UIMap.UIMainWindowWindow.UIItemsListBoxList.Items[0].NativeElement;

            Assert.AreEqual("127.0.0.1: CHF 15.00", entry.Current.Name);
        }

        [TestMethod]
        public void GivenClientAvailable_WhenSendingMultipleMessages_ThenDisplayed()
        {
            TestHelper.Post("CHF 15.00");
            TestHelper.Post("CHF 15.00");
            TestHelper.Post("CHF 15.00");
            Thread.Sleep(3000);
            int count = this.UIMap.UIMainWindowWindow.UIItemsListBoxList.Items.Count;

            Assert.AreEqual(3, count);
        }

        [TestMethod]
        public void GivenClientPortChanged_WhenEnteringOtherPort_ThenMessageStillDisplayed()
        {
            this.UIMap.ClickStopButton();
            this.UIMap.EnterPortValue();
            this.UIMap.ClickStartButton();

            TestHelper.Post("CHF 15.00", "localhost", 6745);

            var entry = (AutomationElement)this.UIMap.UIMainWindowWindow.UIItemsListBoxList.Items[0].NativeElement;

            Assert.AreEqual("127.0.0.1: CHF 15.00", entry.Current.Name);
        }

        [TestInitialize]
        public void MyTestInitialize()
        {
            this.applicationUnderTest = ApplicationUnderTest.Launch(ApplicationPath);
        }

        [TestCleanup]
        public void MyTestCleanup()
        {
            this.applicationUnderTest.Close();
        }
    }
}
