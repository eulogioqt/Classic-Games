public class PlayerData {
	private int x; // crear position
	private int y;
	
	public PlayerData(int x, int y) {
		this.x = x;
		this.y = y;
	}
	
	public void setPosition(int x, int y) {
		this.x = x;
		this.y = y;
	}
	
	public int getX() {
		return x;
	}
	
	public int getY() {
		return y;
	}
	
	public override string ToString() {
		return x + ";" + y;
	}
}
