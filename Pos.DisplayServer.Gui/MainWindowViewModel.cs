using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Pos.DisplayServer.Gui
{
    public class MainWindowViewModel
    {
        private HttpListener httpListener;

        public MainWindowViewModel()
        {
            httpListener = new HttpListener();
            httpListener.Start();
            var httpListenerContext = httpListener.GetContext();
        }
    }
}
