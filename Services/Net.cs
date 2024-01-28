using Cosmos.HAL;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network;
using Cosmos.System.Network.IPv4.UDP.DNS;
using System.Dynamic;

namespace TangerineOS
{
    public class Net
    {
        public static NetworkDevice networkDevice;
        public static void Start()
        {
            Kernel.useNetwork = true;
            if (Kernel.GUIenabled) { GUI.ProcessManager.Run(new GUI.Net()); }
            networkDevice = NetworkDevice.GetDeviceByName("eth0");
            IPConfig.Enable(networkDevice, new Address(192, 168, 0, 1), new Address(255, 255, 255, 0), new Address(192, 168, 1, 254)); //set default ip as 192.168.0.1
            if (NetworkConfiguration.CurrentAddress != null)
            {
                Kernel.useNetwork = true; //enable kernal application to use the IP as configured
            }
        }
        public static string GetInfo()
        {
            if (!Kernel.useNetwork) { Start(); }
            return "IP Address: " + NetworkConfiguration.CurrentAddress.ToString();
        }
    }
}
namespace TangerineOS.GUI
{
    internal class Net : Process
    {
        public Net() : base("Network Service", Priority.Low) { } //low priority because it is not important in the scheduling 
        internal override int Start()
        {
            return 0;
        }
        internal override void Update()
        {
            Kernel.useNetwork = NetworkConfiguration.CurrentAddress != null;
        }
        internal override int Stop() { Kernel.useNetwork = false; return 0; }
    }
}
namespace TangerineOS.Commands //ignore from here onwards as commands prompt are removed in the system
{
    internal class Network : CommandsTree
    {
        internal Network() : base
            ("Network", "Manages network.",
            new Command[] {
            new Command(new string[] {"net"}, "Displays current Network configuration.")
            //new Command(new string[] {"ping"}, "Pings a specified target.", new string[] { "[target] - target to ping"})
            })
        {
        }
        internal override int Execute(string[] args, CommandShell shell)
        {
            switch (args[0])
            {
                case "net":
                    shell.print = Net.GetInfo();
                    return 0;
            }
            return 1;
        }
    }
}