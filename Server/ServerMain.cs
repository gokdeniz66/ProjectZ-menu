using System;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace Menu.Server
{
    public class ServerMain : BaseScript
    {
        public ServerMain()
        {
            Debug.WriteLine("Hi from Menu.Server");
        }

        [Command("hello_server")]
        public void HelloServer()
        {
            Debug.WriteLine("Sure, hello.");
        }
    }
}
