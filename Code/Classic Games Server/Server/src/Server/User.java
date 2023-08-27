package Server;

import java.net.InetAddress;

public class User {
	private InetAddress address; // crear key
	private int port;
	
	private long lastMessageTime;
	
	private UserData data;

	public User(InetAddress address, int port, UserData data) {
		this.address = address;
		this.port = port;
		
		this.data = data;
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
	
	public long getLastMessageTime() {
		return lastMessageTime;
	}

	public UserData getData() {
		return data;
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
	
	public void setLastMessageTime(long lastMessageTime) {
		this.lastMessageTime = lastMessageTime;
	}
}
