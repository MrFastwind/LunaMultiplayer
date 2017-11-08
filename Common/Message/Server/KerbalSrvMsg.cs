﻿using Lidgren.Network;
using LunaCommon.Enums;
using LunaCommon.Message.Base;
using LunaCommon.Message.Data.Kerbal;
using LunaCommon.Message.Interface;
using LunaCommon.Message.Server.Base;
using LunaCommon.Message.Types;
using System.Collections.Generic;

namespace LunaCommon.Message.Server
{
    public class KerbalSrvMsg : SrvMsgBase<KerbalBaseMsgData>
    {
        /// <inheritdoc />
        internal KerbalSrvMsg() { }

        /// <inheritdoc />
        protected override Dictionary<ushort, IMessageData> SubTypeDictionary { get; } = new Dictionary<ushort, IMessageData>
        {
            [(ushort)KerbalMessageType.Reply] = MessageStore.GetMessageData<KerbalReplyMsgData>(true),
            [(ushort)KerbalMessageType.Proto] = MessageStore.GetMessageData<KerbalProtoMsgData>(true)
        };

        public override ServerMessageType MessageType => ServerMessageType.Kerbal;
        protected override int DefaultChannel => 7;
        public override NetDeliveryMethod NetDeliveryMethod => NetDeliveryMethod.ReliableOrdered;
    }
}