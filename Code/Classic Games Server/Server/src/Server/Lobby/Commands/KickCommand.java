package Server.Lobby.Commands;

import java.util.Arrays;

import Server.ServerChat;
import Server.Lobby.LobbyServer;
import Server.Lobby.Objects.Command;
import Server.Lobby.Objects.CommandExecutor;
import Server.Lobby.Objects.CommandSender;
import Server.Lobby.Objects.Player;
import Server.Lobby.Objects.UDPLobbyServer;
import Server.Objects.Utils;

public class KickCommand implements CommandExecutor {

	@Override
	public boolean onCommand(CommandSender sender, Command cmd, String label, String[] args) {
		if(args.length == 0)
			sender.sendMessage("&4Uso correcto: &c/kick <jugador> <mensaje>");
		else if (args.length > 0){
			Player p = LobbyServer.getPlayer(args[0]);
			if(p != null) {
				ServerChat.broadcastMessage("&4" + sender.getName() + "&c ha expulsado a &4" + p.getName());
				UDPLobbyServer.kick(p.getUser(), args.length == 1 ? "Has sido expulsado del servidor" : 
					Utils.argsToString(Arrays.copyOfRange(args, 1, args.length)));
			} else
				sender.sendMessage("&cEl jugador &4&n" + args[0] + "&c no esta conectado");
		}
		
		return true;
	}

}
