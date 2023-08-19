using System;

public class ON {
    private string key;
    private UserData data;
    private PlayerData playerData;

    private ON(string key, UserData data, PlayerData playerData) {
        this.key = key;
        this.data = data;
        this.playerData = playerData;
    }

    public string getKey() {
        return key;
    }

    public UserData getData() {
        return data;
    }

    public PlayerData getPlayerData() {
        return playerData;
    }

    public static ON process(string message) {
        string key = message.Substring(3).Split(" ")[0];
        string[] data = message.Substring(3 + key.Length + 1).Split(";");

        return new ON(key, new UserData(data[0]), new PlayerData(int.Parse(data[1]), int.Parse(data[2])));
    }
}
