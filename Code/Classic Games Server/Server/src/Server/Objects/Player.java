package Server.Objects;

public class Player {
	private int x;
	private int y;
	
	private User user;
	
	public Player(int x, int y, User user) {
		this.x = x;
		this.y = y;
		
		this.user = user;
	}
	
	public String getName() {
		return user.getData().getName();
	}
	
	public int getX() {
		return x;
	}
	
	public int getY() {
		return y;
	}
	
	@Override
	public String toString() {
		return x + ";" + y;
	}
	
	protected void setPosition(int x, int y) {
		this.x = x;
		this.y = y;
	}
}
