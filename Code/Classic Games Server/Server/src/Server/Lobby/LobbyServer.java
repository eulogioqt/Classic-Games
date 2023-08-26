package Server.Lobby;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;

import Server.FileManager;
import Server.ServerConsole;
import Server.ServerTimer;
import Server.Lobby.Commands.External.BroadcastCommand;
import Server.Lobby.Objects.Player;
import Server.Lobby.Objects.UDPLobbyServer;
import Server.Objects.ChatColor;
import Server.Objects.User;

public class LobbyServer {

	private static ServerTimer st;
	private static ServerConsole sc;
	
	public static String MOTD;
	
	public static void onEnable() {
		sc = new ServerConsole();
		new Thread(sc).start();
		LobbyServer.getServerConsole().sendMessage(ChatColor.DARK_GREEN + "Cargando consola...");
		
		LobbyCommandManager.setExecutor("broadcast", new BroadcastCommand());
		LobbyServer.getServerConsole().sendMessage(ChatColor.DARK_GREEN + "Cargando comandos...");
		
		ServerChat.loadChat();
		LobbyServer.getServerConsole().sendMessage(ChatColor.DARK_GREEN + "Cargando chat...");
		
		MOTD = FileManager.readFile("config", "MOTD", "Servidor de Classic Games");
		LobbyServer.getServerConsole().sendMessage(ChatColor.DARK_GREEN + "Cargando configuracion...");
		
		st = new ServerTimer();
		st.start();
		LobbyServer.getServerConsole().sendMessage(ChatColor.DARK_GREEN + "Activando timer...");
	}

	public static List<Player> getOnlinePlayers() {
		List<Player> onlinePlayers = new ArrayList<Player>();
		for(User user : UDPLobbyServer.users.values())
			onlinePlayers.add(user.getPlayer());
		return onlinePlayers;
	}
	
	public static Player getPlayer(String name) {
		Player player = null;
		
		List<Player> onlinePlayers = getOnlinePlayers();
		Iterator<Player> iter = onlinePlayers.iterator();
		while(iter.hasNext() && player == null) {
			Player p = iter.next();
			if(p.getName().equals(name))
				player = p;
		}
		
		return player;
	}

	public static ServerConsole getServerConsole() {
		return sc;
	}
	
	
}
