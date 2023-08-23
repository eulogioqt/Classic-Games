package Server;
import java.time.LocalDateTime;
import java.util.Scanner;

import Server.Lobby.LobbyCommandManager;
import Server.Lobby.Objects.CommandSender;
import Server.Objects.ChatColor;

public class ServerConsole implements Runnable, CommandSender {	
	// el codigo de CHAT al recibir detecta si es un mensaje o si es un comando (empieza por /)
	// metodo onChat y metodo onCommand
	// interfaz CommandSender, todo el que envia un comando es un CommandSender
	// luego en especifico puede ser un User o un Server
	
	@Override
	public void run() {
		try (Scanner sc = new Scanner(System.in)) {
			while (true) {
				String s = sc.nextLine();
				LobbyCommandManager.executeCommand(this, s);
			}
		}
	}
	private String transformToConsoleColors(String message) { // Replaces ChatColor codes efficiently
		char[] transformedMessage = new char[1024 * 8];
		char[] msg = message.toCharArray();

		int j = 0;
		boolean[] style = new boolean[4];
		for (int i = 0; i < message.length(); i++) {
			if (msg[i] == '&') {
				if (i + 1 < msg.length) {
					char myChar = Character.toLowerCase(msg[i + 1]);
					int listLength = ChatColor.values().length;

					int k = 0;
					while (k < listLength && ChatColor.values()[k].toString().charAt(1) != myChar)
						k++;

					if (k < listLength) {
						String addText = (style[0] ? "\033[22m" : "") + (style[1] ? "\033[23m" : "") + (style[2]  ? "\033[24m" : "");
						
						style[3] = false;
						if(myChar == ChatColor.BOLD.toString().charAt(1))
							style[0] = true;
						else if(myChar == ChatColor.ITALIC.toString().charAt(1))
							style[1] = true;
						else if(myChar == ChatColor.UNDERLINE.toString().charAt(1))
							style[2] = true;
						else 
							style = new boolean[] { false, false, false, true };
						
						char[] color = ((style[3] ? addText : "") + ConsoleColor.valueOf(ChatColor.values()[k].name())).toString().toCharArray();
						int codeLength = color.length;
						for (int h = 0; h < codeLength; h++) {
							transformedMessage[j] = color[h];
							j++;
						}
						
						i++;
					} else
						transformedMessage[j] = msg[i];
				}
			} else
				transformedMessage[j] = msg[i];
			j++;
		}

		LocalDateTime now = LocalDateTime.now();
		String hour =  String.format("%02d", now.getHour());
		String minute = String.format("%02d", now.getMinute());
		String second =  String.format("%02d", now.getSecond());
		
		return "\033[0m" + ConsoleColor.WHITE + "[" + hour + ":" + minute + ":" + second + "] " + new String(transformedMessage) + ConsoleColor.WHITE + "\033[24m";
	}
	
	@Override
	public String getName() {
		return "CONSOLE";
	}
	
	@Override
	public void sendMessage(String message) {
		if (message != null)
			System.out.println( transformToConsoleColors(message));
	}
}
