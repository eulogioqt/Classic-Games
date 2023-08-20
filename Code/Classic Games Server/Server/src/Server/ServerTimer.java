package Server;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import Server.Objects.UDPServer;
import Server.Objects.User;
import Server.Objects.Utils;

public class ServerTimer extends Thread {
	
	private void sendTimeouts() throws IOException, InterruptedException {
		UDPServer.timeoutList = new ArrayList<>(UDPServer.users.keySet());
		
		for(String key : UDPServer.timeoutList)
			Utils.sendTIMEOUT(UDPServer.users.get(key));
		
		Thread.sleep(10000); // Los que respondan en este tiempo, se quitaran de la lista y no se les desconectara

		for(String key : UDPServer.timeoutList) {
			User offUser = UDPServer.users.get(key);
			if(offUser != null) { // Por si se ha desconectado
				List<User> restUsers = UDPServer.getRestUsers(key);
				
				Utils.sendOFF(offUser, restUsers);
				
				ServerChat.broadcastMessage("&e" + offUser.getData().getName() + " left the server");
				UDPServer.users.remove(key);
			}
		}
	}
	
	@Override
	public void run() {
		while(true) {
			try {
				Thread.sleep(20000);
				
				ServerChat.saveChat();
				sendTimeouts();
			} catch (InterruptedException | IOException e) {
				e.printStackTrace();
			}
		}
	}
}
