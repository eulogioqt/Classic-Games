package CGTP.COMMANDS.LOBBY;

import Server.Lobby.Objects.LobbyUser;

public class ON {
	public static String getMessage(LobbyUser newOnUser) {
		return "ON " + newOnUser.getKey() + " " + newOnUser;
	}
}
