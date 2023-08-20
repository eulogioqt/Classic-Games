package CGTP.COMMANDS;

import Server.Objects.User;

public class ON {
	public static String getMessage(User newOnUser) {
		return "ON " + newOnUser.getKey() + " " + newOnUser;
	}
}
