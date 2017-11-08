﻿using Lidgren.Network;
using LunaCommon;
using LunaCommon.Enums;
using LunaCommon.Message.Data.PlayerConnection;
using LunaCommon.Message.Server;
using LunaServer.Context;
using LunaServer.Log;
using LunaServer.Plugin;
using LunaServer.Server;
using LunaServer.System;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LunaServer.Client
{
    public class ClientConnectionHandler
    {
        public static void ConnectClient(NetConnection newClientConnection)
        {
            var newClientObject = new ClientStructure(newClientConnection.RemoteEndPoint)
            {
                Subspace = int.MinValue,
                PlayerStatus = new PlayerStatus(),
                ConnectionStatus = ConnectionStatus.Connected,
                Connection = newClientConnection,
                LastSendTime = 0,
                LastReceiveTime = ServerContext.ServerClock.ElapsedMilliseconds
            };

            Task.Run(() => MessageSender.StartSendingOutgoingMessages(newClientObject));

            LmpPluginHandler.FireOnClientConnect(newClientObject);

            ServerContext.Clients.TryAdd(newClientObject.Endpoint, newClientObject);
            VesselUpdateRelaySystem.AddPlayer(newClientObject);
            LunaLog.Debug($"Online Players: {ServerContext.PlayerCount}, connected: {ServerContext.Clients.Count}");
        }

        public static void DisconnectClient(ClientStructure client, string reason = "")
        {
            if (!string.IsNullOrEmpty(reason))
                LunaLog.Debug($"{client.PlayerName} sent Connection end message, reason: {reason}");

            VesselUpdateRelaySystem.RemovePlayer(client);

            //Remove Clients from list
            if (ServerContext.Clients.ContainsKey(client.Endpoint))
            {
                ServerContext.Clients.TryRemove(client.Endpoint, out client);
                LunaLog.Debug($"Online Players: {ServerContext.PlayerCount}, connected: {ServerContext.Clients.Count}");
            }

            if (client.ConnectionStatus != ConnectionStatus.Disconnected)
            {
                client.ConnectionStatus = ConnectionStatus.Disconnected;
                LmpPluginHandler.FireOnClientDisconnect(client);
                if (client.Authenticated)
                {
                    ChatSystem.RemovePlayer(client.PlayerName);

                    var msgData = ServerContext.ServerMessageFactory.CreateNewMessageData<PlayerConnectionLeaveMsgData>();
                    msgData.PlayerName = client.PlayerName;

                    MessageQueuer.RelayMessage<PlayerConnectionSrvMsg>(client, msgData);
                    LockSystem.ReleasePlayerLocks(client.PlayerName);

                    if (!ServerContext.Clients.Any(c => c.Value.Subspace == client.Subspace))
                    {
                        WarpSystem.RemoveSubspace(client.Subspace);
                        VesselRelaySystem.RemoveSubspace(client.Subspace);
                    }
                }

                try
                {
                    client.Connection?.Disconnect(reason);
                }
                catch (Exception e)
                {
                    LunaLog.Debug($"Error closing client Connection: {e.Message}");
                }
                ServerContext.LastPlayerActivity = ServerContext.ServerClock.ElapsedMilliseconds;
            }
        }
    }
}