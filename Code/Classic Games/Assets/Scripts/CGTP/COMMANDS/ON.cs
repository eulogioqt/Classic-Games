using System;

public class ON {
    private string key;
    private UserData data;

    private int x;
    private int y;

    private ON(string key, UserData data, int x, int y) {
        this.key = key;
        this.data = data;

        this.x = x;
        this.y = y;
    }

    public string getKey() {
        return key;
    }

    public UserData getData() {
        return data;
    }

    public int getX() {
        return x;
    }
    public int getY() {
        return y;
    }

    public static ON process(string message) {
        string key = message.Substring(3).Split(" ")[0];
        string[] data = message.Substring(3 + key.Length + 1).Split(";");

        return new ON(key, new UserData(data[0]), int.Parse(data[1]), int.Parse(data[2]));
    }
}
