package Server.Lobby;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import Server.Lobby.Objects.LobbyUser;
import Server.Lobby.Objects.UDPLobbyServer;

public class LobbyTimer extends Thread {
	
	private void sendTimeouts() throws IOException, InterruptedException {
		List<String> sentAlives = new ArrayList<>();
		for(LobbyUser user : UDPLobbyServer.users.values()) {
			if(System.currentTimeMillis() - user.getLastMessageTime() >= 30000) {
				LobbyServer.getServerConsole().sendMessage("Enviado STATUS a " + user.getData().getName());
				sentAlives.add(user.getKey());
				LobbyUtils.sendSTATUS(user);
			}
		}
		
		Thread.sleep(10000); // Tiempo para detectar algun nuevo mensaje del usuario

		for(String key : sentAlives) {
			LobbyUser offUser = UDPLobbyServer.users.get(key);
			if(offUser != null && System.currentTimeMillis() - offUser.getLastMessageTime() >= 30000) { // Por si se ha desconectado en ese tiempo
				LobbyServer.getServerConsole().sendMessage("Desconectando a " + offUser.getData().getName());
				
				UDPLobbyServer.kick(offUser, "TIMEOUT: Tu conexion a Internet no es estable");
			}
		}
	}
	
	@Override
	public void run() {
		while(true) {
			try {
				Thread.sleep(20000);
				
				LobbyChat.saveChat();
				sendTimeouts();
			} catch (InterruptedException | IOException e) {
				e.printStackTrace();
			}
		}
	}
}
