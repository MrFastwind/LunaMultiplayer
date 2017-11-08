﻿using LunaCommon.Message.Types;

namespace LunaCommon.Message.Data.Warp
{
    public class WarpChangeSubspaceMsgData : WarpBaseMsgData
    {
        /// <inheritdoc />
        internal WarpChangeSubspaceMsgData() { }
        public override WarpMessageType WarpMessageType => WarpMessageType.ChangeSubspace;
        public string PlayerName { get; set; }
        public int Subspace { get; set; }
    }
}