using System;
using System.Numerics;

namespace L2RPPS.PacketStructs
{
    public class SightEnterNotify
    {
        public uint PlayerCount { get; set; }
        public ulong PlayerUserIdentification { get; set; } //PktOtherPlayer::SetId(uint64_t)
        public string PlayerName { get; set; } //PktOtherPlayer::SetName(FString const&)
        public uint Race { get; set; } //PktOtherPlayer::SetRaceInfoId(uint32_t)
        public uint Class { get; set; } //PktOtherPlayer::SetClassInfoId(uint32_t)
        public uint Level { get; set; } //PktOtherPlayer::SetLevel(uint16_t)
        public Vector3 Pos { get; set; } //PktOtherPlayer::SetPos(FVector const&) TODO: convert this shit to vector sometime around
        public float Direction { get; set; } //PktOtherPlayer::SetDir(FVector const&) TODO: this too
        public uint CurrentHealth { get; set; } //PktOtherPlayer::SetCurHp(uint32_t)
        public uint MaxHealth { get; set; } //PktOtherPlayer::SetMaxHp(uint32_t)
        public uint MoveSpeed { get; set; } //PktOtherPlayer::SetMoveSpeed(uint32_t)
        public uint BuffCount { get; set; } //PktOtherPlayer::SetBuffInfos(std::list<PktBuffInfo, std::allocator<PktBuffInfo> > const&)
        public byte CombatMode { get; set; } //PktOtherPlayer::SetCombatMode(bool)
        public byte SoulshotsEnabled { get; set; } //PktOtherPlayer::SetSoulShotEnable(bool)
        public byte HostileStatus { get; set; } //PktOtherPlayer::SetPkStatus(EPkStatus)
        public byte HostileState { get; set; } //PktOtherPlayer::SetPkAttackState(bool)
        public uint HostilePoint { get; set; } //PktOtherPlayer::SetPkPoint(uint32_t) (chaos value?)

        public string ReadSightEnterNotify(PacketReader reader)
        {
            PlayerCount = reader.ReadUInt16();

            if (PlayerCount <= 0) return string.Empty;

            for (var i = 0; i < PlayerCount; i++)
            {
                PlayerUserIdentification = reader.ReadUInt64();
                PlayerName = reader.ReadString();
                Race = reader.ReadUInt32();
                Class = reader.ReadUInt32();
                Level = reader.ReadUInt16();
                Pos = reader.ReadVector3();
                Direction = reader.ReadSingle();
                CurrentHealth = reader.ReadUInt32();
                MaxHealth = reader.ReadUInt32();
                MoveSpeed = reader.ReadUInt32();
                BuffCount = reader.ReadUInt16();
                for (var j = 0; j < BuffCount; j++)
                {
                    new BuffInfo().ReadBuffInfo(reader);
                }
                CombatMode = reader.ReadByte();
                SoulshotsEnabled = reader.ReadByte();
                HostileStatus = reader.ReadByte();
                HostileState = reader.ReadByte();
                HostilePoint = reader.ReadUInt32();

            }

            return $"GUID: {PlayerUserIdentification} NAME: {PlayerName} RACE: {Race}{Environment.NewLine}" +
                   $"CLASS: {Class} LEVEL: {Level} XYZ: {Pos}{Environment.NewLine}" +
                   $"DIRECTION: {Direction} CURRENT HEALTH: {CurrentHealth}/{MaxHealth} SPEED: {MoveSpeed}{Environment.NewLine}" +
                   $"BUFF COUNT: {BuffCount} COMBAT MODE: {CombatMode} SOULSHOT: {SoulshotsEnabled}{Environment.NewLine}" +
                   $"PVP MODE: {HostileStatus} PVP STATE: {HostileState} PVP POINT: {HostilePoint}";
        }
    }
}