package CGTP.COMMANDS;

import CGTP.PlayerData;
import CGTP.UserData;

public class HOLA {
	private UserData data;
	private PlayerData playerData;
	
	private HOLA(UserData data, PlayerData playerData) {
		this.data = data;
		this.playerData = playerData;
	}
	
	public UserData getData() {
		return data;
	}
	
	public PlayerData getPlayerData() {
		return playerData;
	}
	
	public static HOLA process(String message) {
		String data = message.substring(5);
		String[] list = data.split(";");
		
		return new HOLA(new UserData(list[0]), new PlayerData(Integer.parseInt(list[1]), Integer.parseInt(list[2])));
	}
}
