﻿using Lidgren.Network;
using LunaCommon.Enums;
using LunaCommon.Message.Base;
using LunaCommon.Message.Client.Base;
using LunaCommon.Message.Data.Handshake;
using LunaCommon.Message.Interface;
using LunaCommon.Message.Types;
using System.Collections.Generic;

namespace LunaCommon.Message.Client
{
    public class HandshakeCliMsg : CliMsgBase<HandshakeBaseMsgData>
    {
        /// <inheritdoc />
        internal HandshakeCliMsg() { }

        /// <inheritdoc />
        protected override Dictionary<ushort, IMessageData> SubTypeDictionary { get; } = new Dictionary<ushort, IMessageData>
        {
            [(ushort)HandshakeMessageType.Request] = MessageStore.GetMessageData<HandshakeRequestMsgData>(true),
            [(ushort)HandshakeMessageType.Response] = MessageStore.GetMessageData<HandshakeResponseMsgData>(true)
        };

        public override ClientMessageType MessageType => ClientMessageType.Handshake;

        protected override int DefaultChannel => 1;

        public override NetDeliveryMethod NetDeliveryMethod => NetDeliveryMethod.ReliableOrdered;
    }
}