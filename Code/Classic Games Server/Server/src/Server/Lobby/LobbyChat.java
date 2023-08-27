package Server.Lobby;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;

import Server.ChatColor;
import Server.Lobby.Objects.LobbyUser;
import Server.Lobby.Objects.UDPLobbyServer;

public class LobbyChat { // hacerlo como el del minecraft, hashmap de chathistory, uno para cada usuario, pero para eso tiene que haber cuentas
	// no obstante, hacer antes chat para cada uno aunque no se guarde la parte de cada uno, o bien ir probando con el tener nombres distintos sin cuentas
	private static List<String> chatHistory;
	
	public static void saveChat() {
		try {
			if(!Files.exists(Paths.get("logs")))
				Files.createDirectory(Paths.get("logs"));
			
			FileOutputStream fos = new FileOutputStream("logs/chat");
			ObjectOutputStream oos = new ObjectOutputStream(fos);
			oos.writeObject(chatHistory);
			oos.close();
		} catch (IOException e) {
			LobbyServer.getServerConsole().sendMessage(ChatColor.RED + "No se ha encontrado historial de chat: " + e.getMessage());
		}
	}
	
	@SuppressWarnings("unchecked")
	public static void loadChat () {
		try {
			FileInputStream fis = new FileInputStream("logs/chat");
			ObjectInputStream ois = new ObjectInputStream(fis);
			chatHistory = (ArrayList<String>) ois.readObject();
			ois.close();
		} catch (FileNotFoundException e) {
			saveChat();
		} catch (IOException | ClassNotFoundException e) {
			LobbyServer.getServerConsole().sendMessage(ChatColor.RED + "No se ha encontrado historial de chat: " + e.getMessage());
		}
		
		if(chatHistory == null)
			chatHistory = new ArrayList<>();
	}
	
	public static void broadcastMessage(String message) {
		chatHistory.add(message);
		
		List<LobbyUser> users = new ArrayList<>(UDPLobbyServer.users.values());
		LobbyUtils.sendCHAT(message, users);
		
		LobbyServer.getServerConsole().sendMessage(message);
	}
	
	public static List<String> getChat() {
		return chatHistory;
	}
}
