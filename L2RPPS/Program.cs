using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using L2RPPS.Packets;
using L2RPPS.PacketStructs;
using SharpPcap;
using PacketDotNet;

namespace L2RPPS
{
    class Program
    {
        private static readonly byte[] EncryptionKey = {0xA7, 0x84, 0x20, 0xD0, 0xC9, 0x78, 0xB3, 0x9A};
        private static readonly List<byte> IncomingBuffer = new List<byte>();
        private static readonly PacketTypes.Handler PacketDictionaryHandler = new PacketTypes.Handler();
        private static readonly PacketTypes.Package PacketDictionaryPackage = new PacketTypes.Package();

        static void Main(string[] args)
        {
            // Print SharpPcap version
            var ver = SharpPcap.Version.VersionString;
            Console.WriteLine("PacketDotNet example using SharpPcap {0}", ver);

            // Retrieve the device list
            var devices = CaptureDeviceList.Instance;

            // If no devices were found print an error
            if (devices.Count < 1)
            {
                Console.WriteLine("No devices were found on this machine");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("The following devices are available on this machine:");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine();

            var i = 0;

            // Print out the devices
            foreach (var dev in devices)
            {
                /* Description */
                Console.WriteLine("{0}) {1} {2}", i, dev.Name, dev.Description);
                i++;
            }

            Console.WriteLine();
            Console.Write("-- Please choose a device to capture: ");
            i = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException());

            // Register a cancel handler that lets us break out of our capture loop
            Console.CancelKeyPress += HandleCancelKeyPress;

            var device = devices[i];

            // Open the device for capturing
            const int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);

            // Settings
            device.Filter = "src port 12000 and dst port 2450 and len > 60";
            //device.Filter = "src port 4632 and dst port 12000 and len > 60";

            device.OnPacketArrival += Device_OnPacketArrival;

            Console.WriteLine();
            Console.WriteLine("-- Listening on {0}, hit 'ctrl-c' to stop...", device.Name);

            Console.WriteLine(device.Statistics.ToString());

            device.StartCapture();

            Console.ReadKey();

            // Print out the device statistics
            Console.WriteLine(device.Statistics.ToString());

            // Close the pcap device
            device.Close();
        }

        private static void Device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            var rawPacket = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
            var extractRawPacket = (TcpPacket) rawPacket.Extract(typeof(TcpPacket));

            if (extractRawPacket == null) return;

            IncomingBuffer.AddRange(extractRawPacket.PayloadDataHighPerformance.ActualBytes());

            while (IncomingBuffer.Count > 2)
            {
                var packetLength = BitConverter.ToUInt16(IncomingBuffer.GetRange(0, 2).ToArray(), 0);

                if (IncomingBuffer.Count >= packetLength)
                {
                    var payloadData = IncomingBuffer.GetRange(3, packetLength - 3).ToArray();
                    IncomingBuffer.RemoveRange(0, packetLength);

                    DecryptPacket(payloadData);
//                    foreach (var x in payloadData)
//                    {
//                        Console.WriteLine("===========================");
//                        Console.WriteLine(x);
//                        Console.WriteLine("===========================");
//                    }
                    ParsePacket(new PacketReader(payloadData));
                }
                else
                {
                    break;
                }

            }

        }

        private static void DecryptPacket(IList<byte> packet)
        {
            for (var i = 0; i < packet.Count; i++)
            {
                packet[i] = (byte) (packet[i] ^ EncryptionKey[i % EncryptionKey.Length]);
            }
        }

        public static void PacketIdentifier(PacketTypes.Package package, PacketReader packetReader)
        {
            PacketDictionaryPackage.Reader = packetReader;
            PacketDictionaryHandler.DictionaryPacketHandlers[package.Type](typeof(PacketTypes.Package));
        }

        private static void ParsePacket(PacketReader packet)
        {
            var packetId = (ushort) (packet.ReadUInt16() - 1);

            PacketDictionaryPackage.Id = packetId;

            try
            {
                //Console.WriteLine($"=========== [ {packetId} ] ============");
                PacketIdentifier(PacketDictionaryPackage, packet);
            }

            catch (Exception e)
            {
                //Console.WriteLine(e);
            }

        }

        static void HandleCancelKeyPress(Object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("-- Stopping capture");

            // tell the handler that we are taking care of shutting down, don't
            // shut us down after we return because we need to do just a little
            // bit more processing to close the open capture device etc
            e.Cancel = true;
        }
    }
}
