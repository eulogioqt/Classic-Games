package Server.Lobby.Commands.External;

import Server.Lobby.LobbyChat;
import Server.Lobby.LobbyUtils;
import Server.Lobby.Objects.Command;
import Server.Lobby.Objects.CommandExecutor;
import Server.Lobby.Objects.CommandSender;

public class BroadcastCommand implements CommandExecutor {

	@Override
	public boolean onCommand(CommandSender sender, Command cmd, String label, String[] args) {
		if(args.length == 0)
			sender.sendMessage("&4Uso correcto: &c/broadcast <mensaje>");
		else if(args.length > 0)
			LobbyChat.broadcastMessage(LobbyUtils.argsToString(args));
		return true;
	}

}
