public class User {
	private string address; // crear key
	private int port;
	
	private UserData data;

	private int x;
	private int y;

    public User(UserData data, int x, int y) {
        this.data = data;

        this.x = x;
        this.y = y;
    }

    public User(string address, int port, UserData data, int x, int y) {
		this.address = address;
		this.port = port;

		this.data = data;

		this.x = x;
		this.y = y;
	}
	
	public string getAddress() {
		return address;
	}
	
	public int getPort() {
		return port;
	}
	
	public string getKey() {
		return address + ":" + port;
	}

	public UserData getData() {
		return data;
	}

    public int getX() {
        return x;
    }

    public int getY() {
        return x;
    }

    public override string ToString() {
		return data + ";" + x + ";" + y;
	}
}
