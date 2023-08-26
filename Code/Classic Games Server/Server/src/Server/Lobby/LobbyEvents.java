package Server.Lobby;

import Server.Lobby.Objects.Player;

public class LobbyEvents {
	public static void onPlayerChat(Player player, String message) {
		ServerChat.broadcastMessage("<" + player.getName() + "> " + message);
	}
	
	public static void onPlayerJoin(Player player) {
		ServerChat.broadcastMessage("&e" + player.getName()  + " joined the game.");
	}
	
	public static void onPlayerLeave(Player player) {
		ServerChat.broadcastMessage("&e" + player.getName() + " left the game.");
	}
	
	public static void onPlayerMove(Player player) {

	}
}
