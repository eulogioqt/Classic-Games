public class User {
	private string address; // crear key
	private int port;
	
	private UserData data;
	private PlayerData playerData;
	
	public User(string address, int port, UserData data, PlayerData playerData) {
		this.address = address;
		this.port = port;
		this.data = data;
		this.playerData = playerData;
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
	
	public PlayerData getPlayerData() {
		return playerData;
	}

	public override string ToString() {
		return data + ";" + playerData;
	}
}
