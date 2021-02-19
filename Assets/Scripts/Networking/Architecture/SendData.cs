namespace Networking.Architecture {
    public static class SendData {
        public static void SendTcpData(Packet packet) {
            packet.WriteLength();
            Client.Instance.Tcp.SendData(packet);
        }

        public static void SendUdpData(Packet packet) {
            packet.WriteLength();
            Client.Instance.Udp.SendData(packet);
        }
    }
}
