public class DISCONNECT {
    private string disconnectMessage;

    private DISCONNECT(string disconnectMessage) {
        this.disconnectMessage = disconnectMessage;
    }

    public string getDisconnectMessage() {
        return disconnectMessage;
    }

    public static DISCONNECT process(string message) {
        string disconnectMessage = message.Substring(11);

        return new DISCONNECT(disconnectMessage);
    }
}
