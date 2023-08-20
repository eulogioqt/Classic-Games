package Server.Lobby;
import Server.ServerChat;
import Server.ServerConsole;
import Server.ServerTimer;
import Server.Objects.ChatColor;
import Server.Objects.Player;

public class LobbyServer {

	public static void onEnable() {
		ServerChat.loadChat();
		ServerConsole.sendMessage(ChatColor.DARK_GREEN + "Cargando chat...");
		
		ServerTimer st = new ServerTimer();
		st.start();
		ServerConsole.sendMessage(ChatColor.DARK_GREEN + "Activando timer...");
		
		ServerConsole sc = new ServerConsole();
		sc.start();
	}
	
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
