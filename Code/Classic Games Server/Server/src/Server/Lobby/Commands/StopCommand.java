package Server.Lobby.Commands;

import Server.Lobby.ServerChat;
import Server.Lobby.Objects.Command;
import Server.Lobby.Objects.CommandExecutor;
import Server.Lobby.Objects.CommandSender;
import Server.Lobby.Objects.UDPLobbyServer;
import Server.Objects.User;
import Server.Objects.Utils;

public class StopCommand implements CommandExecutor {

	@Override
	public boolean onCommand(CommandSender sender, Command cmd, String label, String[] args) {
		ServerChat.broadcastMessage("&d" + sender.getName() + " esta cerrando el servidor. Cerrando en 3...");
		Utils.sleep(1000);
		ServerChat.broadcastMessage("&dCerrando en 2...");
		Utils.sleep(1000);
		ServerChat.broadcastMessage("&dCerrando en 1...");
		Utils.sleep(1000);
		
		for(User user : UDPLobbyServer.users.values())
			UDPLobbyServer.kick(user, "Server closed");
		
		ServerChat.saveChat();
		System.exit(0);
		
		return true;
	}

}
