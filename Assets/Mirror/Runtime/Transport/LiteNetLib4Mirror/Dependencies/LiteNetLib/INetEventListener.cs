using System.Net;
using System.Net.Sockets;

namespace LiteNetLib
{
    /// <summary>
    /// Type of message that you receive in OnNetworkReceiveUnconnected event
    /// </summary>
    public enum UnconnectedMessageType
    {
        BasicMessage,
        Broadcast
    }

    /// <summary>
    /// Disconnect reason that you receive in OnPeerDisconnected event
    /// </summary>
    public enum DisconnectReason
    {
        ConnectionFailed,
        Timeout,
        HostUnreachable,
        RemoteConnectionClose,
        DisconnectPeerCalled,
        ConnectionRejected,
        InvalidProtocol,
        UnknownHost
    }

    /// <summary>
    /// Additional information about disconnection
    /// </summary>
    public struct DisconnectInfo
    {
        /// <summary>
        /// Additional info why peer disconnected
        /// </summary>
        public DisconnectReason Reason;

        /// <summary>
        /// Error code (if reason is SocketSendError or SocketReceiveError)
        /// </summary>
        public SocketError SocketErrorCode;

        /// <summary>
        /// Additional data that can be accessed (only if reason is RemoteConnectionClose)
        /// </summary>
        public NetPacketReader AdditionalData;
    }

    public interface INetEventListener
    {
        /// <summary>
        /// New remote peer connected to host, or client connected to remote host
        /// </summary>
        /// <param name="peer">Connected peer object</param>
        void OnPeerConnected(NetPeer peer);

        /// <summary>
        /// Peer disconnected
        /// </summary>
        /// <param name="peer">disconnected peer</param>
        /// <param name="disconnectInfo">additional info about reason, errorCode or data received with disconnect message</param>
        void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo);

        /// <summary>
        /// Network error (on send or receive)
        /// </summary>
        /// <param name="endPoint">From endPoint (can be null)</param>
        /// <param name="socketError">Socket error</param>
        void OnNetworkError(IPEndPoint endPoint, SocketError socketError);

        /// <summary>
        /// Received some data
        /// </summary>
        /// <param name="peer">From peer</param>
        /// <param name="reader">DataReader containing all received data</param>
        /// <param name="deliveryMethod">Type of received packet</param>
        void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod);

        /// <summary>
        /// Received unconnected message
        /// </summary>
        /// <param name="remoteEndPoint">From address (IP and Port)</param>
        /// <param name="reader">Message data</param>
        /// <param name="messageType">Message type (simple, discovery request or responce)</param>
        void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType);

        /// <summary>
        /// Latency information updated
        /// </summary>
        /// <param name="peer">Peer with updated latency</param>
        /// <param name="latency">latency value in milliseconds</param>
        void OnNetworkLatencyUpdate(NetPeer peer, int latency);

        /// <summary>
        /// On peer connection requested
        /// </summary>
        /// <param name="request">Request information (EndPoint, internal id, additional data)</param>
        void OnConnectionRequest(ConnectionRequest request);
    }

    public class EventBasedNetListener : INetEventListener
    {
        public delegate void OnPeerConnected(NetPeer peer);
        public delegate void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo);
        public delegate void OnNetworkError(IPEndPoint endPoint, SocketError socketError);
        public delegate void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod);
        public delegate void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType);
        public delegate void OnNetworkLatencyUpdate(NetPeer peer, int latency);

        public delegate void OnConnectionRequest(ConnectionRequest request);

        public event OnPeerConnected PeerConnectedEvent;
        public event OnPeerDisconnected PeerDisconnectedEvent;
        public event OnNetworkError NetworkErrorEvent;
        public event OnNetworkReceive NetworkReceiveEvent;
        public event OnNetworkReceiveUnconnected NetworkReceiveUnconnectedEvent;
        public event OnNetworkLatencyUpdate NetworkLatencyUpdateEvent;
        public event OnConnectionRequest ConnectionRequestEvent;

        public void ClearPeerConnectedEvent()
        {
            PeerConnectedEvent = null;
        }

        public void ClearPeerDisconnectedEvent()
        {
            PeerConnectedEvent = null;
        }

        public void ClearNetworkErrorEvent()
        {
            NetworkErrorEvent = null;
        }

        public void ClearNetworkReceiveEvent()
        {
            NetworkReceiveEvent = null;
        }

        public void ClearNetworkReceiveUnconnectedEvent()
        {
            NetworkReceiveUnconnectedEvent = null;
        }

        public void ClearNetworkLatencyUpdateEvent()
        {
            NetworkLatencyUpdateEvent = null;
        }

        public void ClearConnectionRequestEvent()
        {
            ConnectionRequestEvent = null;
        }

        void INetEventListener.OnPeerConnected(NetPeer peer)
		{
			PeerConnectedEvent?.Invoke(peer);
		}

        void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
		{
			PeerDisconnectedEvent?.Invoke(peer, disconnectInfo);
		}

        void INetEventListener.OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
		{
			NetworkErrorEvent?.Invoke(endPoint, socketErrorCode);
		}

        void INetEventListener.OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
		{
			NetworkReceiveEvent?.Invoke(peer, reader, deliveryMethod);
		}

        void INetEventListener.OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
		{
			NetworkReceiveUnconnectedEvent?.Invoke(remoteEndPoint, reader, messageType);
		}

        void INetEventListener.OnNetworkLatencyUpdate(NetPeer peer, int latency)
		{
			NetworkLatencyUpdateEvent?.Invoke(peer, latency);
		}

        void INetEventListener.OnConnectionRequest(ConnectionRequest request)
		{
			ConnectionRequestEvent?.Invoke(request);
		}
    }
}
