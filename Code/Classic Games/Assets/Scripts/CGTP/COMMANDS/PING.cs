public class PING {
    private int onlinePlayers;
    private string version;
    private string MOTD;

    private PING(int onlinePlayers, string version, string MOTD) {
        this.onlinePlayers = onlinePlayers;
        this.version = version;
        this.MOTD = MOTD;
    }

    public int getOnlinePlayers() {
        return onlinePlayers;
    }

    public string getVersion() {
        return version;
    }

    public string getMOTD() {
        return MOTD;
    }

    public static PING process(string message) {
        string[] data = message.Substring(5).Split(" ");

        return new PING(int.Parse(data[0]), data[1], message.Substring(5 + data[0].Length + 1 + data[1].Length + 1));
    }
    public static string getMessage() {
        return "PING";
    }
}
