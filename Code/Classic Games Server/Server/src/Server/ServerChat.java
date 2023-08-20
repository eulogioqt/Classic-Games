package Server;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.util.ArrayList;
import java.util.List;

import Server.Objects.ChatColor;
import Server.Objects.UDPServer;
import Server.Objects.User;
import Server.Objects.Utils;

public class ServerChat { // hacerlo como el del minecraft, hashmap de chathistory, uno para cada usuario, pero para eso tiene que haber cuentas
	// no obstante, hacer antes chat para cada uno aunque no se guarde la parte de cada uno, o bien ir probando con el tener nombres distintos sin cuentas
	private static List<String> chatHistory;
	
	public static void saveChat() throws IOException {
		FileOutputStream fos = new FileOutputStream("chat");
		ObjectOutputStream oos = new ObjectOutputStream(fos);
		oos.writeObject(chatHistory);
		oos.close();
	}
	
	@SuppressWarnings("unchecked")
	public static void loadChat () {
		try {
			FileInputStream fis = new FileInputStream("chat");
			ObjectInputStream ois = new ObjectInputStream(fis);
			chatHistory = (ArrayList<String>) ois.readObject();
			ois.close();
		} catch (IOException | ClassNotFoundException e) {
			ServerConsole.sendMessage(ChatColor.RED + "No se ha encontrado historial de chat: " + e.getMessage());
		}
		
		if(chatHistory == null)
			chatHistory = new ArrayList<>();
	}
	
	// si en un futuro elimino que se guarde el chat, mover esta funcion a Utils
	public static void broadcastMessage(String message) {
		chatHistory.add(message);
		
		List<User> users = UDPServer.getPlayers();
		Utils.sendCHAT(message, users);
		
		ServerConsole.sendMessage(message);
	}
	
	public static List<String> getChat() {
		return chatHistory;
	}
}
