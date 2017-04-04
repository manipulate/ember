namespace Ember {

    using Asterion;
    using Asterion.Out;
    using Sockets = System.Net.Sockets;

    public abstract class ClientBase {

        public Sockets.TcpClient client;

        public void WriteData(string data) {
            try {
                Logger.WriteOutput("Outgoing: " + data, Logger.LogLevel.Debug);

                byte[] bytesToWrite = Utils.StrToBytes(data + "\0");
                client.LingerState = new Sockets.LingerOption(true, 2);

                if (client.GetStream().CanWrite) {
                    client.GetStream().BeginWrite(bytesToWrite, 0, bytesToWrite.Length, WriteCallback, client);
                }
            } catch (System.Exception writeEx) {
                Logger.WriteOutput("Could not write to client: " + writeEx.Message + ".");
            }
        }

        private void WriteCallback(System.IAsyncResult asyncResult) {
            Sockets.TcpClient writeConnection = (Sockets.TcpClient)asyncResult.AsyncState;

            try {
                writeConnection.GetStream().EndWrite(asyncResult);
                writeConnection.LingerState = new Sockets.LingerOption(false, 0);
            } catch (System.Exception writeEx) {
                Logger.WriteOutput("Unable to end writing: " + writeEx.Message);
            }
        }

    }

}
