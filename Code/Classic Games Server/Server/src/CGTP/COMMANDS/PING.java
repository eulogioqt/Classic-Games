package CGTP.COMMANDS;

public class PING {
	public static String getMessage(int onlinePlayers, String version, String MOTD) {
		return "PING " + onlinePlayers + " " + version + " " + MOTD;
	}
}
