package Server.Lobby.Objects;
import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.fusesource.jansi.AnsiConsole;

import CGTP.CommandType;
import CGTP.COMMANDS.CHAT;
import CGTP.COMMANDS.PROTOCOL_COMMAND;
import CGTP.COMMANDS.HOLA;
import CGTP.COMMANDS.MOVE;
import Server.ServerChat;
import Server.Lobby.LobbyCommandManager;
import Server.Lobby.LobbyEvents;
import Server.Lobby.LobbyServer;
import Server.Objects.ChatColor;
import Server.Objects.User;
import Server.Objects.Utils;

public class UDPLobbyServer {
	
	// QUE CADA VEZ QUE SE CONECTE ALGUIEN, SE CREE UN THREAD ESCUCHANDO AL PUERTO DEL QUE SE HA CONECTADO
	// DE ESTA FORMA, SE MEJORA EL RENDIMIENTO
	// TAMBIEN QUE CUANDO VAYA A ENVIAR UN MENSAJE
	
	// lo he visto dias despues y es buena idea, podria meter todo el codigo de atender a los comandos
	// en user si veo que con el tiempo los comandos tienen que procesar mucho y asi una hebra por usuario
	// procesaria solo los comandos de cada usuario idk
	
	public static Map<String, User> users = new HashMap<>();
	
	private static DatagramSocket s;
	
	public static List<User> getRestUsers(String key) {
		Map<String, User> sendTo = new HashMap<>(users);
		sendTo.remove(key);
		return new ArrayList<>(sendTo.values());
	}
	
	public static void main(String[] args) throws IOException {
		AnsiConsole.systemInstall();
		
		LobbyServer.onEnable();
		
		s = Utils.initSocket(11000);
		LobbyServer.getServerConsole().sendMessage(ChatColor.DARK_GREEN + "Inicializando socket...");
		LobbyServer.getServerConsole().sendMessage(ChatColor.GREEN + "Servidor inicializado correctamente en el puerto " + s.getLocalPort());
		
		byte[] buffer;
		while (true) {
			buffer = new byte[800];

			DatagramPacket dp = new DatagramPacket(buffer, buffer.length);
			s.receive(dp);
			
			PROTOCOL_COMMAND cmd = new PROTOCOL_COMMAND(dp);

			if(cmd.getType() == CommandType.PING)
				Utils.sendPING(cmd.getAddress(), cmd.getPort(), users.size());
			
			if(cmd.getType() != CommandType.HOLA && !users.containsKey(cmd.getSenderKey())) { // por si envia mensaje alguien que no esta conectado
				continue;
			}
			
			if(users.containsKey(cmd.getSenderKey())) // actualizar el timeouttime
				users.get(cmd.getSenderKey()).setLastMessageTime(System.currentTimeMillis());
			
			if(cmd.getType() == CommandType.HOLA) {
				HOLA msg = HOLA.process(cmd.getCommand());

				User newUser = new User(cmd.getAddress(), cmd.getPort(), msg.getData());
				Player player = new Player(msg.getX(), msg.getY(), newUser);
				newUser.setPlayer(player);
				
				List<User> restUsers = getRestUsers(cmd.getSenderKey());
				
				newUser.setLastMessageTime(System.currentTimeMillis());
				users.put(cmd.getSenderKey(), newUser);
				
				Utils.sendON(newUser, restUsers);
				Utils.sendINFO(newUser, restUsers, ServerChat.getChat());
				
				LobbyServer.getServerConsole().sendMessage("Nueva conexion " + newUser.getData().getName() + " (" + newUser.getKey() + ")");
				LobbyEvents.onPlayerJoin(newUser.getPlayer());
			} else if (cmd.getType() == CommandType.ADIOS) {
				User offUser = users.get(cmd.getSenderKey());

				kick(offUser, "Adios");
			} else if (cmd.getType() == CommandType.MOVE) {
				MOVE msg = MOVE.process(cmd.getCommand());
				
				User movedUser = users.get(cmd.getSenderKey());
				List<User> restUsers = getRestUsers(cmd.getSenderKey());
				
				movedUser.getPlayer().setPosition(msg.getX(), msg.getY());
				
				Utils.sendMOVE(movedUser, msg.getX(), msg.getY(), restUsers);
				
				LobbyEvents.onPlayerMove(movedUser.getPlayer());
			} else if (cmd.getType() == CommandType.CHAT){ 
				CHAT msg = CHAT.process(cmd.getCommand());
				
				if(msg.getMessage().startsWith("/")) {
					LobbyCommandManager.executeCommand(users.get(cmd.getSenderKey()).getPlayer(), msg.getMessage().substring(1));
				} else
					LobbyEvents.onPlayerChat(users.get(cmd.getSenderKey()).getPlayer(), msg.getMessage());
			} else if (cmd.getType() == CommandType.STATUS) {
				Utils.sendALIVE(users.get(cmd.getSenderKey()));
			} else if (cmd.getType() == CommandType.ALIVE) {

			} else {
				LobbyServer.getServerConsole().sendMessage(ChatColor.RED + "Algo salio mal: " + cmd.getCommand());
			}
		}
	}
	
	public static void kick(User user, String message) {
		List<User> restUsers = getRestUsers(user.getKey());
		
		Utils.sendOFF(user, restUsers);
		Utils.sendDISCONNECT(user, message);

		users.remove(user.getKey());

		LobbyEvents.onPlayerLeave(user.getPlayer());
	}
	
	public static void send(DatagramPacket datagram) {
		try {
			s.send(datagram); 
		} catch (Exception e) {
			LobbyServer.getServerConsole().sendMessage(ChatColor.RED + e.getMessage());
		}
	}
}