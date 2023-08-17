package CGTP.COMMANDS;

import CGTP.User;

public class ON {
	public static String getMessage(User newOnUser) {
		return "ON " + newOnUser.getKey() + " " + newOnUser;
	}
}
