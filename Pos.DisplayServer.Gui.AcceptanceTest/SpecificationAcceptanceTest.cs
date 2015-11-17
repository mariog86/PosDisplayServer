using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;


namespace Pos.DisplayServer.Gui.AcceptanceTest
{
    /// <summary>
    /// Summary description for CodedUITest1
    /// </summary>
    [CodedUITest]
    public class SpecificationAcceptanceTest
    {
        public SpecificationAcceptanceTest()
        {
        }

        [TestMethod]
        public void GivenClientAvailable_WhenSendingMessage_ThenDisplayed()
        {
            var entries = UIMap.UIMainWindowWindow.UIListBoxList.Items.Select(uit => uit.NativeElement).OfType<Label>().Select(l => l.Text);

            Assert.IsTrue(entries.Any(e => e == "CHF 10.-"));
        }

        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        //Use TestInitialize to run code before running each test 
        [TestInitialize()]
        [DeploymentItem("source", "targetFolder")]
        public void MyTestInitialize()
        {
            applicationUnderTest = ApplicationUnderTest.Launch(ApplicationPath);
        }

        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            applicationUnderTest.Close();
        }

        #endregion

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        private TestContext testContextInstance;
        private static readonly string ApplicationPath = Assembly.GetAssembly(typeof (Gui.App)).Location;
        private ApplicationUnderTest applicationUnderTest;

        public UIMap UIMap
        {
            get
            {
                if ((this.map == null))
                {
                    this.map = new UIMap();
                }

                return this.map;
            }
        }

        private UIMap map;
    }
}
