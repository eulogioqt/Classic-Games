package Server.Lobby;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import Server.Lobby.Commands.KickCommand;
import Server.Lobby.Commands.StopCommand;
import Server.Lobby.Objects.Command;
import Server.Lobby.Objects.CommandExecutor;
import Server.Lobby.Objects.CommandSender;
import Server.Lobby.Objects.Player;

public class LobbyCommandManager implements CommandExecutor {
	
	private static Map<String, CommandExecutor> internalCommands = Map.of(
			"stop", new StopCommand(),
			"kick", new KickCommand()
	);
	private static Map<String, CommandExecutor> externalCommands = new HashMap<>();
	private static CommandExecutor defaultExecutor = new LobbyCommandManager();
	
	public static void setExecutor(String command, CommandExecutor cmd) {
		externalCommands.put(command, cmd);
	}
	
	public static void executeCommand(CommandSender sender, String msg) {
		String label = (msg.contains(" ") ? msg.split(" ")[0] : msg);
		Command cmd = new Command(label);
		String[] args = new String[0];
		
		if(msg.contains(" ")) {
			String restMsg = msg.substring(label.length() + 1);
			args = restMsg.contains(" ") ? restMsg.split(" ") : new String[] { restMsg };
		}

		CommandExecutor exec = externalCommands.get(cmd.getName());
		exec = exec != null ? exec : internalCommands.get(cmd.getName());
		exec = exec != null ? exec : defaultExecutor;
		
		if(!exec.onCommand(sender, cmd, label, args))
			sender.sendMessage("&fEl comando que has introducido no existe.");
	}
	
	
	
	@Override
	public boolean onCommand(CommandSender sender, Command cmd, String label, String[] args) {
		if(cmd.getName().equalsIgnoreCase("list")) {
			List<Player> onlinePlayers = LobbyServer.getOnlinePlayers();
			sender.sendMessage("&bJugadores conectados (&3" + onlinePlayers.size() + "&b):");
			for(Player player : onlinePlayers)
				sender.sendMessage("- &e" + player.getName() + " &f(" + player.getX() + ", " + player.getY() +")");
			
			return true;
		} else if (cmd.getName().equalsIgnoreCase("say")) {
			if(args.length == 0)
				sender.sendMessage("&4Uso correcto: &c/say <mensaje>");
			else if(args.length > 0) {
				LobbyChat.broadcastMessage("[" + sender.getName() + "] " + LobbyUtils.argsToString(args));
			}
			return true;
		} else if (cmd.getName().equalsIgnoreCase("tp")) {
			if(args.length <= 2)
				sender.sendMessage("&4Uso correcto: &c/tp <nombre> <x> <y>");
			else {
				Player player = LobbyServer.getPlayer(args[0]);
				if(player != null) {
					try {
						player.teleport(Integer.parseInt(args[1]), Integer.parseInt(args[2]));
						sender.sendMessage("&e&l>> &fHas teletransportado a &b" + player.getName() + " &f a las coordenadas &b" + args[1] + "&f, &b" + args[2]);
					} catch (Exception e) {
						sender.sendMessage("&cHas introducido coordenadas no numericas");
					}
				} else
					sender.sendMessage("&cEl jugador &4&n" + args[0] + "&c no esta conectado");
			}
			return true;
		}
		
		return false;
	}
}
