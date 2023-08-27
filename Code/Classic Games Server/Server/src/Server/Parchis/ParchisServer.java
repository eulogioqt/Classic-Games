package Server.Parchis;

import Server.WindowsConsole;
import Server.Parchis.Objects.Player;

public class ParchisServer {
	
	public static void onEnable() {
		
	}
	
	public static void onPlayerChat(Player player, String message) {
		ParchisUtils.sendCHAT(player.getUser(), message);
		
		WindowsConsole.println("Nuevo mensaje: " + message);
	}
	
}
