package Server.Lobby;
import java.util.Scanner;

import Server.WindowsConsole;
import Server.Lobby.Objects.CommandSender;

public class LobbyConsole implements Runnable, CommandSender {	
	@Override
	public void run() {
		try (Scanner sc = new Scanner(System.in)) {
			while (true) {
				String s = sc.nextLine();
				LobbyCommandManager.executeCommand(this, s);
			}
		}
	}
	
	@Override
	public String getName() {
		return "CONSOLE";
	}
	
	@Override
	public void sendMessage(String message) {
		if (message != null)
			WindowsConsole.println(message);
	}
}
