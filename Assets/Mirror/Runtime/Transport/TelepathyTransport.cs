// wraps Telepathy for use as HLAPI TransportLayer
using UnityEngine;
namespace Mirror
{
    [HelpURL("https://github.com/vis2k/Telepathy/blob/master/README.md")]
    public class TelepathyTransport : Transport
    {
        public ushort port = 7777;

        [Tooltip("Nagle Algorithm can be disabled by enabling NoDelay")]
        public bool NoDelay = true;

        protected Telepathy.Client client = new Telepathy.Client();
        protected Telepathy.Server server = new Telepathy.Server();

        void Awake()
        {
            // tell Telepathy to use Unity's Debug.Log
            Telepathy.Logger.Log = Debug.Log;
            Telepathy.Logger.LogWarning = Debug.LogWarning;
            Telepathy.Logger.LogError = Debug.LogError;

            // configure
            client.NoDelay = NoDelay;
            server.NoDelay = NoDelay;

            // HLAPI's local connection uses hard coded connectionId '0', so we
            // need to make sure that external connections always start at '1'
            // by simple eating the first one before the server starts
            Telepathy.Server.NextConnectionId();

            Debug.Log("TelepathyTransport initialized!");
        }

        // client
        public override bool ClientConnected() { return client.Connected; }
        public override void ClientConnect(string address) { client.Connect(address, port); }
        public override bool ClientSend(int channelId, byte[] data) { return client.Send(data); }

        bool ProcessClientMessage()
        {
            if (client.GetNextMessage(out Telepathy.Message message))
            {
                switch (message.eventType)
                {
                    case Telepathy.EventType.Connected:
                        OnClientConnected.Invoke();
                        break;
                    case Telepathy.EventType.Data:
                        OnClientDataReceived.Invoke(message.data);
                        break;
                    case Telepathy.EventType.Disconnected:
                        OnClientDisconnected.Invoke();
                        break;
                    default:
                        // TODO:  Telepathy does not report errors at all
                        // it just disconnects,  should be fixed
                        OnClientDisconnected.Invoke();
                        break;
                }
                return true;
            }
            return false;
        }
        public override void ClientDisconnect() { client.Disconnect(); }

        // IMPORTANT: set script execution order to >1000 to call Transport's
        //            LateUpdate after all others. Fixes race condition where
        //            e.g. in uSurvival Transport would apply Cmds before
        //            ShoulderRotation.LateUpdate, resulting in projectile
        //            spawns at the point before shoulder rotation.
        public void LateUpdate()
        {
            // note: we need to check enabled in case we set it to false
            // when LateUpdate already started.
            // (https://github.com/vis2k/Mirror/pull/379)
            while (enabled && ProcessClientMessage()) {}
            while (enabled && ProcessServerMessage()) {}
        }

        // server
        public override bool ServerActive() { return server.Active; }
        public override void ServerStart() { server.Start(port); }
        public override bool ServerSend(int connectionId, int channelId, byte[] data) { return server.Send(connectionId, data); }
        public bool ProcessServerMessage()
        {
            if (server.GetNextMessage(out Telepathy.Message message))
            {
                switch (message.eventType)
                {
                    case Telepathy.EventType.Connected:
                        OnServerConnected.Invoke(message.connectionId);
                        break;
                    case Telepathy.EventType.Data:
                        OnServerDataReceived.Invoke(message.connectionId, message.data);
                        break;
                    case Telepathy.EventType.Disconnected:
                        OnServerDisconnected.Invoke(message.connectionId);
                        break;
                    default:
                        // TODO handle errors from Telepathy when telepathy can report errors
                        OnServerDisconnected.Invoke(message.connectionId);
                        break;
                }
                return true;
            }
            return false;
        }
        public override bool ServerDisconnect(int connectionId) { return server.Disconnect(connectionId); }
        public override string ServerGetClientAddress(int connectionId) { return server.GetClientAddress(connectionId); }
        public override void ServerStop() { server.Stop(); }

        // common
        public override void Shutdown()
        {
            Debug.Log("TelepathyTransport Shutdown()");
            client.Disconnect();
            server.Stop();
        }

        public override int GetMaxPacketSize(int channelId)
        {
            // Telepathy's limit is Array.Length, which is int
            return int.MaxValue;
        }

        public override string ToString()
        {
            if (server.Active && server.listener != null)
            {
                return "Telepathy Server port: " + server.listener.LocalEndpoint;
            }
            else if (client.Connecting || client.Connected)
            {
                return "Telepathy Client ip: " + client.client.Client.RemoteEndPoint;
            }
            return "Telepathy (inactive/disconnected)";
        }
    }
}
