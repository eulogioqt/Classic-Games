import java.net.InetAddress;

public class User {
	private InetAddress address;
	private int port;
	
	private String name;
	private int x;
	private int y;
	
	public User(InetAddress address, int port, String name) {
		this.name = name;
		this.address = address;
		this.port = port;
	}
	
	public void setPosition(int x, int y) {
		this.x = x;
		this.y = y;
	}
	
	public InetAddress getAddress() {
		return address;
	}
	
	public int getPort() {
		return port;
	}
	
	public String getName() {
		return name;
	}
	
	public int getX() {
		return x;
	}
	
	public int getY() {
		return y;
	}
	
	@Override
	public String toString() {
		return "User(" + name + ", " + address.toString() + ", " + port + ")";
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
