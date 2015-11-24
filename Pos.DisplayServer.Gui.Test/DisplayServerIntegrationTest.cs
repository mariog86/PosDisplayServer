using System.Collections.Specialized;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using Pos.Test.Helper;

namespace Pos.DisplayServer.Gui.Test
{
    [TestFixture]
    public class DisplayServerIntegrationTest
    {
        private MainWindowViewModel mainWindowViewModel;

        [SetUp]
        public void SetUp()
        {
            this.mainWindowViewModel = new MainWindowViewModel();
        }

        [TearDown]
        public void TearDown()
        {
            this.mainWindowViewModel.Dispose();
        }

        [TestCase("CHF 10.00")]
        [TestCase("CHF 15.00")]
        public void GivenClientAvailable_WhenSendingMessage_ThenDisplayed(string message)
        {
            TestHelper.Post(message);
            this.WaitTillListModified();
            this.mainWindowViewModel.Items.Should().ContainSingle(item => item.EndsWith(message));
        }

        [Test]
        public void GivenClientAvailable_WhenSendingMultipleMessages_ThenDisplayed()
        {
            string message = "CHF 10.00";
            TestHelper.Post(message);
            this.WaitTillListModified();
            TestHelper.Post(message);
            this.WaitTillListModified();
            TestHelper.Post(message);
            this.WaitTillListModified();
            this.mainWindowViewModel.Items.Should().HaveCount(3);
        }

        [TestCase("CHF 10.00")]
        [TestCase("CHF 15.00")]
        public void GivenServerIsStarted_WhenStoppedPortedChangedAndStarted_ThenMessagesReceived(string message)
        {
            ushort newPort = 6745;
            this.mainWindowViewModel.StopCommand.Execute(null);
            this.mainWindowViewModel.Port = newPort;
            this.mainWindowViewModel.StartCommand.Execute(null);
            TestHelper.Post(message, "localhost", newPort);
            this.WaitTillListModified();
            this.mainWindowViewModel.Items.Should().ContainSingle(item => item.EndsWith(message));
        }

        private void WaitTillListModified()
        {
            var finished = new ManualResetEvent(false);
            this.mainWindowViewModel.Items.As<INotifyCollectionChanged>().CollectionChanged +=
                (sender, args) => finished.Set();
            finished.WaitOne(5000);
        }
    }
}
