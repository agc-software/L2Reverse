namespace L2RPPS.PacketStructs
{
    public class BuffInfo
    {
        public ulong SkillUniqueIdentifier { get; set; }
        public uint  SkillIdentifier { get; set; }
        public uint BuffIdentifier { get; set; }
        public uint BuffLevel { get; set; }
        public uint StackCount { get; set; }
        public ulong EndTime { get; set; }

        public void ReadBuffInfo(PacketReader reader)
        {
            SkillUniqueIdentifier = reader.ReadUInt64();
            SkillIdentifier = reader.ReadUInt32();
            BuffIdentifier = reader.ReadUInt32();
            BuffLevel = reader.ReadUInt32();
            StackCount = reader.ReadUInt16();
            EndTime = reader.ReadUInt64();
        }
    }
}