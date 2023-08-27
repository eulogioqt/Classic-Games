package Server.Lobby;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;

import Server.ChatColor;
import Server.FileManager;
import Server.Lobby.Commands.External.BroadcastCommand;
import Server.Lobby.Objects.LobbyUser;
import Server.Lobby.Objects.Player;
import Server.Lobby.Objects.UDPLobbyServer;

public class LobbyServer {

	private static LobbyTimer st;
	private static LobbyConsole sc;
	
	public static String MOTD;
	
	public static void onEnable() {
		sc = new LobbyConsole();
		new Thread(sc).start();
		LobbyServer.getServerConsole().sendMessage(ChatColor.DARK_GREEN + "Cargando consola...");
		
		LobbyCommandManager.setExecutor("broadcast", new BroadcastCommand());
		LobbyServer.getServerConsole().sendMessage(ChatColor.DARK_GREEN + "Cargando comandos...");
		
		LobbyChat.loadChat();
		LobbyServer.getServerConsole().sendMessage(ChatColor.DARK_GREEN + "Cargando chat...");
		
		MOTD = FileManager.readFile("config", "MOTD", "Servidor de Classic Games");
		LobbyServer.getServerConsole().sendMessage(ChatColor.DARK_GREEN + "Cargando configuracion...");
		
		st = new LobbyTimer();
		st.start();
		LobbyServer.getServerConsole().sendMessage(ChatColor.DARK_GREEN + "Activando timer...");
	}

	public static List<Player> getOnlinePlayers() {
		List<Player> onlinePlayers = new ArrayList<Player>();
		for(LobbyUser user : UDPLobbyServer.users.values())
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

	public static LobbyConsole getServerConsole() {
		return sc;
	}
	
	
}
