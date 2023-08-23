package Server.Lobby.Commands.External;

import Server.ServerChat;
import Server.Lobby.Objects.Command;
import Server.Lobby.Objects.CommandExecutor;
import Server.Lobby.Objects.CommandSender;
import Server.Objects.Utils;

public class BroadcastCommand implements CommandExecutor {

	@Override
	public boolean onCommand(CommandSender sender, Command cmd, String label, String[] args) {
		if(args.length == 0)
			sender.sendMessage("&4Uso correcto: &c/broadcast <mensaje>");
		else if(args.length > 0)
			ServerChat.broadcastMessage(Utils.argsToString(args));
		return true;
	}

}
