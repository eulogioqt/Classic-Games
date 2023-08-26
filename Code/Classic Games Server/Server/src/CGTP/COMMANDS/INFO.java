package CGTP.COMMANDS;

import java.util.List;

import Server.Objects.User;

public class INFO {
	public static String getMessage(String sendToKey, List<User> users, List<String> chat) {
		String INFO = "INFO\n";
		for(User user : users)
			INFO += user.getKey() + " " + user + "\n";
		INFO += ".\n";
		
		for(String message : chat)
			INFO += message + "\n";
		INFO += ".";
		
		return INFO;
	}
}
