public class PING {
    private int onlinePlayers;

    private PING(int onlinePlayers) {
        this.onlinePlayers = onlinePlayers;
    }

    public int getOnlinePlayers() {
        return onlinePlayers;
    }

    public static PING process(string message) {
        int onlinePlayers = int.Parse(message.Substring(5));

        return new PING(onlinePlayers);
    }
    public static string getMessage() {
        return "PING";
    }
}
