import java.io.IOException;
import java.util.Scanner;

public class ServerConsole extends Thread {
	private enum ConsoleColor {
		BLACK("\033[30m"),
		DARK_BLUE("\033[34m"),
		DARK_GREEN("\033[32m"),
		DARK_AQUA("\033[36m"),
		DARK_RED("\033[31m"),
		DARK_PURPLE("\033[35m"),
		GOLD("\033[33m"),
		GRAY("\033[37m"),
		DARK_GRAY("\033[90m"),
		BLUE("\033[94m"),
		GREEN("\033[92m"),
		AQUA("\033[96m"),
		RED("\033[91m"),
		LIGHT_PURPLE("\033[95m"),
		YELLOW("\033[93m"),
		WHITE("\033[97m");
		
		private final String value;
		private ConsoleColor(String value) {
			this.value = value;
		}
		
		@Override
		public String toString() {
			return value;
		}
	}
	
	@Override
	public void run() {
		try (Scanner sc = new Scanner(System.in)) {
			while(true) {
				String s = sc.nextLine();
				ServerChat.broadcastMessage("[CONSOLE] " + s);
			}
		} catch (IOException e) {
			e.printStackTrace();
		}
	}
	
	private static String transformToConsoleColors(String message) { // Replaces ChatColor codes efficiently
		char[] transformedMessage = new char[1024 * 8];
		char[] msg = message.toCharArray();
		
		int j = 0;
		for(int i = 0; i < message.length(); i++) {
			if(msg[i] == '&') {
				if(i + 1 < msg.length) {
					char myChar = msg[i+1];
					int listLength = ChatColor.values().length;
					
					int k = 0;
					while(k < listLength && ChatColor.values()[k].toString().charAt(1) != myChar)
						k++;
					
					if(k < listLength) {
						char[] color = ConsoleColor.valueOf(ChatColor.values()[k].name()).toString().toCharArray();
						int codeLength = color.length;
						for(int h = 0; h < codeLength; h++) {
							transformedMessage[j] = color[h];
							j++;
						}
							
						i++;
					} else transformedMessage[j] = msg[i];
				}
			} else 
				transformedMessage[j] = msg[i];
			j++;
		}
		
		return new String(transformedMessage) + ConsoleColor.WHITE;
	}
	
	public static void sendMessage(String message) {
		if(message == null)
			sendMessage(ChatColor.RED + "Has intentado enviar un mensaje vacio");
		else if(message.length() > 1024)
			sendMessage(ChatColor.RED + "Has intentado enviar un mensaje demasiado largo");
		else 
			System.out.println(transformToConsoleColors(message));
	}
}
