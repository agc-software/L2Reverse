using System;
using System.Collections.Generic;
using System.Linq;
using L2RPPS.PacketStructs;

namespace L2RPPS.Packets
{
    class PacketTypes
    {
        public enum PacketType
        {
            Version = 0,
            VersionResult = 1,
            Login = 2,
            LoginResult = 3,
            Logout = 4,
            KickOut = 13,
            KickOutNotify = 14,
            KickOutResult = 15,
            PlayerListRead = 18,
            PlayerListReadResult = 19,
            PlayerSelect = 24,
            PlayerSelectResult = 25,
            NoticeNotify = 39,
            Version2 = 40,
            Version2Result = 41,
            PktMapPlayerPositionRead = 118,
            PktMapPlayerPositionReadResult = 119,
            SightEnterNotify = 204,
            SightLeaveNotify = 205,
            CharacterDieNotify = 207,
            PktPlayerInfoReadResult = 250
            // More enum members here
        }

        private static PacketReader CurrentReader { get; set; }

        public class Package
        {
            public PacketReader Reader {
                get => CurrentReader;
                set => CurrentReader = value;
            }
            public int Id { get; set; }

            public PacketType Type => (PacketType)Id;
        }

        public class Handler
        {
            public IDictionary<PacketType, Action<Type>> DictionaryPacketHandlers = new Dictionary<PacketType, Action<Type>>()
            {
                {PacketType.SightEnterNotify, package =>
                    {
                        try
                        {
                            Console.WriteLine(new SightEnterNotify().ReadSightEnterNotify(CurrentReader));
                        }
                        catch (Exception e)
                        {
                            //Console.WriteLine(e);
                        }
                    }}
                // More handlers here
            };
        }
    }
}
