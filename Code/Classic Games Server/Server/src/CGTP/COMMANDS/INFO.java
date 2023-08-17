package CGTP.COMMANDS;

import java.util.List;

import CGTP.ChatMessage;
import CGTP.User;

public class INFO {
	public static String getMessage(String sendToKey, List<User> users, List<ChatMessage> chat) {
		String INFO = "INFO\n";
		for(User user : users) //s.send(createDatagram(ON, user));
			INFO += sendToKey + " " + user + "\n";
		INFO += ".\n";
		
		for(ChatMessage message : chat)
			INFO += message.getSender() + ";" + message.getMessage() + "\n";
		INFO += ".";
		
		return INFO;
	}
}
