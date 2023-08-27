package CGTP.COMMANDS.LOBBY;

import java.util.List;

import Server.Lobby.Objects.LobbyUser;

public class INFO {
	public static String getMessage(List<LobbyUser> users, List<String> chat) {
		String INFO = "INFO\n";
		for(LobbyUser user : users)
			INFO += user.getKey() + " " + user + "\n";
		INFO += ".\n";
		
		for(String message : chat)
			INFO += message + "\n";
		INFO += ".";
		
		return INFO;
	}
}
