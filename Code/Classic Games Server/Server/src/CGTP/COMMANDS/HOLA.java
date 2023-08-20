package CGTP.COMMANDS;

import Server.Objects.UserData;

public class HOLA {
	private UserData data;
	
	private int x;
	private int y;

	private HOLA(UserData data, int x, int y) {
		this.data = data;
		this.x = x;
		this.y = y;
	}
	
	public UserData getData() {
		return data;
	}
	
	public int getX() {
		return x;
	}
	
	public int getY() {
		return y;
	}
	
	public static HOLA process(String message) {
		String data = message.substring(5);
		String[] list = data.split(";");
		
		return new HOLA(new UserData(list[0]), Integer.parseInt(list[1]), Integer.parseInt(list[2]));
	}
}
