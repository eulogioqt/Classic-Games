package Server;

import java.time.LocalDateTime;

public class WindowsConsole {
	public static void println(String message) {
		LocalDateTime now = LocalDateTime.now();
		String hour =  String.format("%02d", now.getHour());
		String minute = String.format("%02d", now.getMinute());
		String second =  String.format("%02d", now.getSecond());
		
		System.out.println("\033[0m" + ConsoleColor.WHITE + "[" + hour + ":" + minute + ":" + second + "] " + ConsoleColor.transformToConsoleColors(message));
	}
}
