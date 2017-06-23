using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{

    interface IUSBBus 
    {

        string USBNameInterface { get; set; }
        int USBSpeedInterface { get; set; }

        void Connect(IUSBBus USBDevice);
        void Disconnect(IUSBBus USBDevice);

        void Send(byte[] data, BaseDevice Device);
        void Receive(byte[] data);

        event EventHandler OnConnect;
        event EventHandler OnDisconnect;
        event EventHandler OnFailure;
    }

    interface ISata 
    {
        string SATANameInterface { get; set; }
        int SATASpeedInterface { get; set; }

        void Connect(ISata SataDevice);
        void Disconnect(ISata SataDevice);

        void Send(byte[] data, BaseDevice Device);
        void Receive(byte[] data);

        event EventHandler OnConnect;
        event EventHandler OnDisconnect;
        event EventHandler OnFailure;

    }

    interface INetwork 
    {
        string NetworkNameInterface { get; set; }
        int NetworkSpeedInterface { get; set; }

        void Connect(INetwork NetworkDevice);
        void Disconnect(INetwork NetworkDevice);

        void Send(byte[] data, BaseDevice Device);
        void Receive(byte[] data);

        string Protocol { get; set; }

        event EventHandler OnConnect;
        event EventHandler OnDisconnect;
        event EventHandler OnFailure;
    }

    interface IInnerBus 
    {
        string InnerNameInterface { get; set; }
        int InnerSpeedInterface { get; set; }

        void Connect(IInnerBus InnerBusDevice);
        void Disconnect(IInnerBus InnerBusDevice);

        void Send(byte[] data, BaseDevice Device);
        void Receive(byte[] data);

        event EventHandler OnConnect;
        event EventHandler OnDisconnect;
        event EventHandler OnFailure;

    }

    public class MyEventArgs : EventArgs
    {
        public string info;

        public MyEventArgs()
        {
        }
        public MyEventArgs(string inf)
        {
            info = inf;
        }
    }

    class Messages
    {
        public void UniversalHandler(object sender, EventArgs e)
        {
            MyEventArgs m = (MyEventArgs)e;
            Console.WriteLine(m.info);
        }
    }

    class BaseDevice
    {
        public byte[] Memory;
        public string NameDevice;

    }

    class RAM : BaseDevice, IInnerBus
    {
        int Freq;

        IInnerBus Spot;
        string InnerBusName;

        public event EventHandler OnConnect;
        public event EventHandler OnDisconnect;
        public event EventHandler OnFailure;

        public RAM(int RAMSize = 2048, string Name = "RAM", int Freq = 2400)
        {
            Memory = new byte[RAMSize];
            NameDevice = Name;
            this.Freq = Freq;
            InnerBusName = Name;
            this.Freq = Freq;
        }

        public string InnerNameInterface { get => InnerBusName; set =>InnerBusName = value; }
        public int InnerSpeedInterface { get =>Freq; set=> Freq = value; }

        public void Connect(IInnerBus InnerBusDevice)
        {
            if (Spot != InnerBusDevice)
            {
                Spot = InnerBusDevice;
                InnerBusDevice.Connect(this);
                OnConnect?.Invoke(this, new MyEventArgs(InnerBusDevice.InnerNameInterface + " connected to" + InnerBusName));
            }
            
        }

        public void Disconnect(IInnerBus InnerBusDevice)
        {
            if (Spot != null)
            {
                Spot = null;
                InnerBusDevice.Disconnect(this);

                OnDisconnect?.Invoke(this, new MyEventArgs(InnerBusDevice.InnerNameInterface + "disconnected from " + InnerBusName));
            }
            else
                OnFailure(this, new MyEventArgs(InnerBusDevice.InnerNameInterface + "failed connect to " + InnerBusName));
        }

        public void Receive(byte[] data)
        {
            Memory = data;
        }

        public void Send(byte[] data, BaseDevice Device)
        {
            IInnerBus obj = Device as IInnerBus;
            if (obj != null)
                Device.Memory = data;
        }

    }

    class HDD : BaseDevice, ISata
    {
        
        string HDDSataName;
        int SataSpeed;
        ISata Port;
        public int RPM;

        public event EventHandler OnConnect;
        public event EventHandler OnDisconnect;
        public event EventHandler OnFailure;

        public string SATANameInterface { get => HDDSataName; set => HDDSataName = value; }
        public int SATASpeedInterface { get => SataSpeed; set => SataSpeed = value; }

        public HDD (int HDDSize = 8192, string Name = "HDD", int Speed = 3028, int RPM = 9100)
        {
            Memory = new byte[8192];
            NameDevice = Name;
            this.RPM = RPM;
            HDDSataName = Name;
            SataSpeed = Speed;
        }

        public void Connect(ISata SataDevice)
        {
            if (Port != SataDevice)
            {
                Port = SataDevice;
                SataDevice.Connect(this);
                OnConnect?.Invoke(this, new MyEventArgs(SataDevice.SATANameInterface + " connected to " + HDDSataName));
            }
           
        }

        public void Disconnect(ISata SataDevice)
        {
            if (Port != null)
            {
                Port = null;
                SataDevice.Disconnect(this);
                OnDisconnect?.Invoke(this, new MyEventArgs(SataDevice.SATANameInterface + "disconnected from " + SATANameInterface));
            }
            
        }

        public void Receive(byte[] data)
        {
            Memory = data;
        }

        public void Send(byte[] data, BaseDevice Device)
        {
            ISata obj = Device as ISata;
            if (obj != null)
                Device.Memory = data;
        }
    }

    class NIC : BaseDevice, INetwork
    {
        string protocol, NetworkName;
        int Speed;
        INetwork Port;

        public event EventHandler OnConnect;
        public event EventHandler OnDisconnect;
        public event EventHandler OnFailure;

        public NIC(string Name = "NIC", int BufferSize = 128, string Protocol = "802.11bgn", int Speed = 100)
        {
            NameDevice = Name;
            Memory = new byte[BufferSize];
            this.protocol = Protocol;
            this.Speed = Speed;
            Port = null;
            NetworkName = Name;
        }

        public string NetworkNameInterface { get => NetworkName; set => NetworkName = value; }
        public int NetworkSpeedInterface { get => Speed; set => Speed = value; }
        public string Protocol { get => protocol; set => protocol = value; }

        public void Connect(INetwork NetworkDevice)
        {
            if (Port != NetworkDevice)
            {
                Port = NetworkDevice;
                NetworkDevice.Connect(this);
            }
        }

        public void Disconnect(INetwork NetworkDevice)
        {
            if (Port != null)
            {
                Port = null;
                NetworkDevice.Disconnect(this);
                OnDisconnect?.Invoke(this, new MyEventArgs(NetworkDevice.NetworkNameInterface + "disconnected from " + NetworkNameInterface));
            }
            else
                OnFailure(this, new MyEventArgs(NetworkDevice.NetworkNameInterface + "failed connect to " + NetworkNameInterface));
        }

        public void Receive(byte[] data)
        {
            Memory = data;
        }

        public void Send(byte[] data, BaseDevice Device)
        {
            INetwork obj = Device as INetwork;
            if (obj != null)
                Device.Memory = data;
        }
    }

    class Printer : BaseDevice, IUSBBus
    {
        string USBName;
        int USBSpeed;

        public NIC NetAdapter;
        IUSBBus USBSpot;

        public event EventHandler OnConnect;
        public event EventHandler OnDisconnect;
        public event EventHandler OnFailure;

        public string USBNameInterface { get => USBName; set => USBName = value; }
        public int USBSpeedInterface { get => USBSpeed; set => USBSpeed= value; }
      
        public Printer (string Name ="Printer", int Speed = 400, int BufferSize = 256)
        {
            Memory = new byte[BufferSize];
            NameDevice = Name;
            USBName = Name;

            USBSpeed = Speed;
           
            NetAdapter = new NIC();
        }

        public void Connect(IUSBBus USBDevice)
        {
            if (USBSpot != USBDevice)
            {
                USBSpot = USBDevice;
                USBDevice.Connect(this);
            }
        }

        public void Disconnect(IUSBBus USBDevice)
        {
            if (USBSpot != null)
            {
                USBSpot = null;
                USBDevice.Disconnect(this);

                OnDisconnect?.Invoke(this, new MyEventArgs(USBDevice.USBNameInterface + "disconnected from " + USBNameInterface));
            }
           
        }

        public void Receive(byte[] data)
        {
            Memory = data;
        }

 
        public void Send(byte[] data, BaseDevice Device)
        {
            IUSBBus obj = Device as IUSBBus;
            if (obj != null)
                Device.Memory = data;
        }
    }

    class Keyboard : BaseDevice, IUSBBus
    {
        string USBName;
        int USBSpeed;
        public IUSBBus USBSpot;

        public event EventHandler OnConnect;
        public event EventHandler OnDisconnect;
        public event EventHandler OnFailure;

        public string USBNameInterface { get => USBName; set => USBName = value; }
        public int USBSpeedInterface { get => USBSpeed; set => USBSpeed = value; }

        public Keyboard (string Name = "Keyboard", int Speed = 400, int BufferSize = 16)
        {
            Memory = new byte[BufferSize];
            NameDevice = Name;
            USBSpeed = Speed;
            USBName = Name;
            USBSpot = null;
        }


        public void Connect(IUSBBus USBDevice)
        {
            if (USBSpot != USBDevice)
            {
                USBSpot = USBDevice;
                USBDevice.Connect(this);
                OnConnect?.Invoke(this, new MyEventArgs(USBDevice.USBNameInterface + "connected to " + USBName));
            }
            
        }

        public void Disconnect(IUSBBus USBDevice)
        {
            if (USBSpot != null)
            {
                USBSpot = null;
                USBDevice.Disconnect(this);
                OnDisconnect?.Invoke(this, new MyEventArgs(USBDevice.USBNameInterface + "disconnected from " + USBName));
            }
            else               
                OnFailure(this, new MyEventArgs(USBDevice.USBNameInterface + "failed connect to" + USBName));
        }
      
        public void Receive(byte[] data)
        {
            Memory = data;
        }

       public void Send(byte[] data, BaseDevice Device)
        {
            IUSBBus obj = Device as IUSBBus;
            if (obj != null)
                Device.Memory = data;
        }
    }

    class Scaner : BaseDevice, IUSBBus
    {
        string USBName;
        int USBSpeed;
        public IUSBBus USBSpot;

        public event EventHandler OnConnect;
        public event EventHandler OnDisconnect;
        public event EventHandler OnFailure;

        public string USBNameInterface { get => USBName; set => USBName = value; }
        public int USBSpeedInterface { get => USBSpeed; set => USBSpeed = value; }

        public Scaner(string Name = "Scanner", int Speed = 400, int BufferSize = 256)
        {
            Memory = new byte[BufferSize];
            NameDevice = Name;
            USBSpeed = Speed;
            USBName = Name;
            USBSpot = null;
        }


        public void Connect(IUSBBus USBDevice)
        {
            if (USBSpot != USBDevice)
            {
                USBSpot = USBDevice;
                USBDevice.Connect(this);
            }
        }

        public void Disconnect(IUSBBus USBDevice)
        {
            if (USBSpot != null)
            {
                USBSpot = null;
                USBDevice.Disconnect(this);

                OnDisconnect?.Invoke(this, new MyEventArgs(USBDevice.USBNameInterface + "disconnected from " + USBNameInterface));
            }
            else
                OnFailure(this, new MyEventArgs(USBDevice.USBNameInterface + "failed connect to " + USBNameInterface));
        }

        public void Receive(byte[] data)
        {
            Memory = data;
        }


        public void Send(byte[] data, BaseDevice Device)
        {
            IUSBBus obj = Device as IUSBBus;
            if (obj != null)
                Device.Memory = data;
        }
    }

    class MotherBoard: BaseDevice, ISata, IUSBBus, IInnerBus, INetwork
    {
        List<ISata> SataPorts;
        List<IUSBBus> USBHub;
        List<INetwork> NetworkAdapter;
        List<IInnerBus> InnerBus;

        public event EventHandler OnConnect;
        public event EventHandler OnDisconnect;
        public event EventHandler OnFailure;

        string SATAName, USBName, InnerName, NetName, Protocol;
        int SATASpeed, USBSpeed, InnerSpeed, NetSpeed;
        
        public MotherBoard(string Name = "MotherBoard", int CPUCash = 8)
        {
            SataPorts = new List<ISata>(6);
            USBHub = new List<IUSBBus>(10);
            NetworkAdapter = new List<INetwork>();
            InnerBus = new List<IInnerBus>();

            NameDevice = Name;
            Memory = new byte[CPUCash];

            SATAName = "MB Sata";
            USBName = "MB USB";
            InnerName = "INNER BUS";
            NetName = "MB Net";
            Protocol = "802.11bgn";

            SATASpeed = 480;
            USBSpeed = 120;
            InnerSpeed = 1024;
            NetSpeed = 512;

        }

        string ISata.SATANameInterface { get => SATAName; set => SATAName = value; }
        int ISata.SATASpeedInterface { get => SATASpeed; set => SATASpeed = value; }
        string IUSBBus.USBNameInterface { get => USBName; set => USBName = value; }
        int IUSBBus.USBSpeedInterface { get => USBSpeed; set => USBSpeed = value; }
        string IInnerBus.InnerNameInterface { get => InnerName; set => InnerName = value; }
        int IInnerBus.InnerSpeedInterface { get => InnerSpeed; set => InnerSpeed = value; }
        string INetwork.NetworkNameInterface { get => NetName; set => NetName = value; }
        int INetwork.NetworkSpeedInterface { get => NetSpeed; set => NetSpeed = value; }
        string INetwork.Protocol { get => Protocol; set => Protocol = value; }

        public int FindElem(object obj)
        {

            if (obj as ISata != null)

                for (int i = 0; i < SataPorts.Count; i++)
                {
                    if (obj == SataPorts[i])
                        return i;
                }

            else
                    if (obj as IInnerBus != null)

                for (int i = 0; i < InnerBus.Count; i++)
                {
                    if (obj == InnerBus[i])
                        return i;
                }
            else
                 if (obj as IUSBBus != null)

                for (int i = 0; i < USBHub.Count; i++)
                {
                    if (obj == USBHub[i])
                        return i;
                }

            else
                if (obj as INetwork != null)

                for (int i = 0; i < NetworkAdapter.Count; i++)
                {
                    if (NetworkAdapter[i] == obj)
                        return i;
                }
            return -1;
        }

        public void ShowAllDev()
        {
            Console.WriteLine("\nSATA devices:");

            foreach (var item in SataPorts)
            {
                Console.WriteLine(item.SATANameInterface + " " + item.SATASpeedInterface + " " );
            }

            Console.WriteLine("\nUSB devices:");
            foreach (var item in USBHub)
            {
                Console.WriteLine(item.USBNameInterface + " " + item.USBSpeedInterface);
            }

            Console.WriteLine("\nInnerBase devices:");
            foreach (var item in InnerBus)
            {
                Console.WriteLine(item.InnerNameInterface + " " + item.InnerSpeedInterface);
            }

            Console.WriteLine("\nNetwork devices:");
            foreach (var item in NetworkAdapter)
            {
                Console.WriteLine(item.NetworkNameInterface + " " + item.NetworkSpeedInterface);
            }
        }

        void ISata.Connect(ISata SataDevice)
        {
            if (SataPorts.Capacity != (SataPorts.Count))
            {
                SataPorts.Add(SataDevice);
                SataDevice.Connect(SataPorts[FindElem(SataDevice)]);

                OnConnect?.Invoke(this, new MyEventArgs(SataDevice.SATANameInterface + " connected to " + SATAName));
            }
            else
                OnFailure(this, new MyEventArgs(SataDevice.SATANameInterface + " failed connect to " + SATAName));
        }

        void IUSBBus.Connect(IUSBBus USBDevice)
        {
            if (USBHub.Capacity != USBHub.Count)
            {
                USBHub.Add(USBDevice);
                USBDevice.Connect(USBHub[FindElem(USBDevice)]);
                OnConnect?.Invoke(this, new MyEventArgs(USBDevice.USBNameInterface + "connected to " + USBName));
            }
            else
                OnFailure(this, new MyEventArgs(USBDevice.USBNameInterface + " failed connect to" + USBName));
        }

        void IInnerBus.Connect(IInnerBus InnerBusDevice)
        {
            InnerBus.Add(InnerBusDevice);
            InnerBusDevice.Connect(InnerBus[FindElem(InnerBusDevice)]);
            OnConnect?.Invoke(this, new MyEventArgs(InnerBusDevice.InnerNameInterface + " connected to " + InnerName));
       
        }

        void INetwork.Connect(INetwork NetworkDevice)
        {
            NetworkAdapter.Add(NetworkDevice);
            NetworkDevice.Connect(NetworkAdapter[FindElem(NetworkDevice)]);
            OnConnect?.Invoke(this, new MyEventArgs(NetworkDevice.NetworkNameInterface + " connected to " + NetName));
        }

        void ISata.Disconnect(ISata SataDevice)
        {
            SataDevice.Disconnect(SataPorts[FindElem(SataDevice)]);
            SataPorts.Remove(SataDevice);
            OnDisconnect?.Invoke(this, new MyEventArgs(SataDevice.SATANameInterface + " disconnected from " + SATAName));
        }

        void IUSBBus.Disconnect(IUSBBus USBDevice)
        {
            USBDevice.Disconnect(USBHub[FindElem(USBDevice)]);
            USBHub.Remove(USBDevice);
            OnDisconnect?.Invoke(this, new MyEventArgs(USBDevice.USBNameInterface + " disconnected from " + USBName));
       
        }
   
        void IInnerBus.Disconnect(IInnerBus InnerBusDevice)
        {
            InnerBusDevice.Disconnect(InnerBus[FindElem(InnerBusDevice)]);
            InnerBus.Remove(InnerBusDevice);
            OnDisconnect?.Invoke(this, new MyEventArgs(InnerBusDevice.InnerNameInterface + " disconnected from " + InnerName));
        }


        void INetwork.Disconnect(INetwork NetworkDevice)
        {
            NetworkDevice.Disconnect(NetworkAdapter[FindElem(NetworkDevice)]);
            NetworkAdapter.Remove(NetworkDevice);
            OnDisconnect?.Invoke(this, new MyEventArgs(NetworkDevice.NetworkNameInterface + " disconnected from " + NetName));
        }


        void ISata.Send(byte[] data, BaseDevice Device)
        {
            ISata obj = Device as ISata;
            if (obj != null)
                Device.Memory = data;
        }

        void ISata.Receive(byte[] data)
        {
            Memory = data;
        }

        void IUSBBus.Send(byte[] data, BaseDevice Device)
        {
            IUSBBus obj = Device as IUSBBus;
            if (obj != null)
                Device.Memory = data;
        }

        void IUSBBus.Receive(byte[] data)
        {
            Memory = data;
        }

        void IInnerBus.Send(byte[] data, BaseDevice Device)
        {
            IInnerBus obj = Device as IInnerBus;
            if (obj != null)
                Device.Memory = data;
        }

        void IInnerBus.Receive(byte[] data)
        {
            Memory = data;
        }

        
        void INetwork.Send(byte[] data, BaseDevice Device)
        {
            INetwork obj = Device as INetwork;
            if (obj != null)
                Device.Memory = data;
        }

        void INetwork.Receive(byte[] data)
        {
            Memory = data;
        }
    }


    

    class Program
    {
        static void Main(string[] args)
        {
            Messages m = new Messages();
            
            MotherBoard MB = new MotherBoard("Asus",16);
            HDD HDD1 = new HDD(8192,"Seagate 2",8200);
            HDD HDD2 = new HDD(8192, "Kingston XZ", 600, 8200);
            Keyboard keyboard = new Keyboard("A4Tech-200 keyboard", 400, 8);
            Printer printer = new Printer("HP-300");
            Printer printer1 = new Printer("HP-100");
            Scaner scanner = new Scaner("Lenovo scanner");
            RAM Ram1 = new RAM(4096, "Asus RAM",1700);
            RAM Ram2 = new RAM(4096, "KingstoN RAM",3200);
            NIC net = new NIC("Broadcom");

            var SATAMB = (ISata)MB;
            var USBMB = (IUSBBus)MB;
            var InnerMB = (IInnerBus)MB;
            var NetMB = (INetwork)MB;

            
            MB.OnConnect += new EventHandler(m.UniversalHandler);
            MB.OnDisconnect += new EventHandler(m.UniversalHandler);
            MB.OnFailure += new EventHandler(m.UniversalHandler);

            HDD1.OnConnect += new EventHandler(m.UniversalHandler);
            HDD1.OnDisconnect += new EventHandler(m.UniversalHandler);
            HDD1.OnFailure += new EventHandler(m.UniversalHandler);

            SATAMB.Connect(HDD1);
            SATAMB.Connect(HDD2);

            USBMB.Connect(keyboard);
            USBMB.Connect(printer);
            USBMB.Connect(scanner);

            NetMB.Connect(net);
            NetMB.Connect(printer1.NetAdapter);

            InnerMB.Connect(Ram1);
            InnerMB.Connect(Ram2);

            SATAMB.Disconnect(HDD2);
            USBMB.Disconnect(printer);


            MB.ShowAllDev();
          

           // MB.ShowSataDev();

            Console.ReadKey(true);

            
        }
    }
}
