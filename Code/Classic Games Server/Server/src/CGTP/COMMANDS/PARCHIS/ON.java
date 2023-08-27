package CGTP.COMMANDS.PARCHIS;

import Server.Parchis.Objects.ParchisUser;

public class ON {
	public static String getMessage(ParchisUser newOnUser) {
		return "ON " + newOnUser.getKey() + " " + newOnUser;
	}
}
