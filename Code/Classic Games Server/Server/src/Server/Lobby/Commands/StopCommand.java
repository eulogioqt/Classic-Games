package Server.Lobby.Commands;

import Server.Lobby.LobbyChat;
import Server.Lobby.LobbyUtils;
import Server.Lobby.Objects.Command;
import Server.Lobby.Objects.CommandExecutor;
import Server.Lobby.Objects.CommandSender;
import Server.Lobby.Objects.LobbyUser;
import Server.Lobby.Objects.UDPLobbyServer;

public class StopCommand implements CommandExecutor {

	@Override
	public boolean onCommand(CommandSender sender, Command cmd, String label, String[] args) {
		LobbyChat.broadcastMessage("&d" + sender.getName() + " esta cerrando el servidor. Cerrando en 3...");
		LobbyUtils.sleep(1000);
		LobbyChat.broadcastMessage("&dCerrando en 2...");
		LobbyUtils.sleep(1000);
		LobbyChat.broadcastMessage("&dCerrando en 1...");
		LobbyUtils.sleep(1000);
		
		for(LobbyUser user : UDPLobbyServer.users.values())
			UDPLobbyServer.kick(user, "Server closed");
		
		LobbyChat.saveChat();
		System.exit(0);
		
		return true;
	}

}
