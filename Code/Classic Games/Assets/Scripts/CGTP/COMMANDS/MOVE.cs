public class MOVE {
	private string key;
	private int x;
	private int y;
	
	private MOVE(string key, int x, int y) {
		this.key = key;
		this.x = x;
		this.y = y;
	}
	
	public string getKey() {
		return key;
	}

	public int getX() {
		return x;
	}
	
	public int getY() {
		return y;
	}
	
	public static MOVE process(string message) {
        string key = message.Substring(5).Split(" ")[0];
        string[] pos = message.Substring(5 + key.Length + 1).Split(";");

        return new MOVE(key, int.Parse(pos[0]), int.Parse(pos[1]));
	}
	
	public static string getMessage(int x, int y) {
		return "MOVE " + x + ";" + y;
	}
}
