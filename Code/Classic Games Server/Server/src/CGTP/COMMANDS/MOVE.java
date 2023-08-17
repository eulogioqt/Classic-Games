package CGTP.COMMANDS;

public class MOVE {
	private int x;
	private int y;
	
	private MOVE(int x, int y) {
		this.x = x;
		this.y = y;
	}
	
	public int getX() {
		return x;
	}
	
	public int getY() {
		return y;
	}
	
	public static MOVE process(String message) {
		String[] data = message.substring(5).split(";");
		
		return new MOVE(Integer.parseInt(data[0]), Integer.parseInt(data[1]));
	}
	
	public static String getMessage(String movedUserKey, int x, int y) {
		return "MOVE " + movedUserKey + " " + x + ";" + y;
	}
}
