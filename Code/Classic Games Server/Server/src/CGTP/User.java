package CGTP;
import java.net.InetAddress;

public class User {
	private InetAddress address; // crear key
	private int port;
	
	private UserData data;
	private PlayerData playerData;
	
	public User(InetAddress address, int port, UserData data, PlayerData playerData) {
		this.address = address;
		this.port = port;
		this.data = data;
		this.playerData = playerData;
	}
	
	public InetAddress getAddress() {
		return address;
	}
	
	public int getPort() {
		return port;
	}
	
	public String getKey() {
		return address + ":" + port;
	}

	public UserData getData() {
		return data;
	}
	
	public PlayerData getPlayerData() {
		return playerData;
	}

	@Override
	public String toString() {
		return data + ";" + playerData;
	}
	
	@Override
	public boolean equals(Object o) {
		boolean r = false;
		if(o instanceof User) {
			User u = (User) o;
			r = address.equals(u.getAddress()) && port == u .getPort();
		}
		return r;
	}
}
