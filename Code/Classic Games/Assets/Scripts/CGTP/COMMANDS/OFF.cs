public class OFF {
    private string key;

    private OFF(string key) {
        this.key = key;
    }

    public string getKey() {
        return key;
    }

    public static OFF process(string message) {
        string key = message.Substring(4);

        return new OFF(key);
    }
}
