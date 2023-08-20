package Server.Objects;
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
import CGTP.COMMANDS.COMMAND;
import CGTP.COMMANDS.HOLA;
import CGTP.COMMANDS.MOVE;
import Server.ServerChat;
import Server.ServerConsole;
import Server.Lobby.LobbyServer;

public class UDPServer {
	
	// QUE CADA VEZ QUE SE CONECTE ALGUIEN, SE CREE UN THREAD ESCUCHANDO AL PUERTO DEL QUE SE HA CONECTADO
	// DE ESTA FORMA, SE MEJORA EL RENDIMIENTO
	// TAMBIEN QUE CUANDO VAYA A ENVIAR UN MENSAJE
	
	// QUE EL SERVIDOR RESPONDA CUANDO SE CONECTA ALGUIEN, PROTOCOLO ETC

	public static Map<String, User> users = new HashMap<>();
	public static List<String> timeoutList; // reemplazar por un bojeto propio que redefina el add del list con un booleano de updated
	
	private static DatagramSocket s;
	
	public static List<User> getRestUsers(String key) {
		Map<String, User> sendTo = new HashMap<>(users);
		sendTo.remove(key);
		return new ArrayList<>(sendTo.values());
	}
	// cuando llega un mensaje, guardo en un hashmao user, int, el systemcurrentmillis de cuando ha llegao el mensaje
	// si cuando toque el timeout, ese systemcurrentmillis es mayor que 30 segundos con el actual, envia timeout y a los 10s
	// vuelve a comprobar ese systemtimemillis, si es menor que antes, no pasa na

	// cunado llega un mensaje ,guardar el systemcurrentseconds, si en algun momento el systemcurrentseconds con el catual se diferencia en 30
	// envia un mensaje timeout al servidor, y este debera responder con alive, si pasados 10s el systemcurrentseconds sigue siendo el mismo que antes
	// da de baja, si es menor, no pasa na
	
	// el servidor cuando dices hola tendra que responder con welcome, durante ese rato e cliente estara
	// conectando
	
	// cuando el servidor mate a alguien le envie un mensje de shutdown para si llega a procesarlo vea que ha muerto
	
	
	public static void main(String[] args) throws IOException {
		AnsiConsole.systemInstall();
		
		LobbyServer.onEnable();
		
		s = Utils.initSocket(11000);
		ServerConsole.sendMessage(ChatColor.DARK_GREEN + "Inicializando socket...");
		ServerConsole.sendMessage(ChatColor.GREEN + "Servidor inicializado correctamente en el puerto " + s.getLocalPort());
		
		byte[] buffer;
		while (true) {
			buffer = new byte[800];

			DatagramPacket dp = new DatagramPacket(buffer, buffer.length);
			s.receive(dp);

			COMMAND cmd = new COMMAND(dp);
			if(cmd.getType() != CommandType.HOLA && users.get(cmd.getSenderKey()) == null) // por si envia mensaje alguien que no esta conectado
				continue;
			
			if(cmd.getType() == CommandType.HOLA) {
				HOLA msg = HOLA.process(cmd.getCommand());

				User newUser = new User(cmd.getAddress(), cmd.getPort(), msg.getData());
				Player player = new Player(msg.getX(), msg.getY(), newUser);
				newUser.setPlayer(player);
				
				List<User> restUsers = getRestUsers(cmd.getSenderKey());
				
				users.put(cmd.getSenderKey(), newUser);
				
				Utils.sendON(newUser, restUsers);
				Utils.sendINFO(newUser, restUsers, ServerChat.getChat());

				LobbyServer.onPlayerJoin(newUser.getPlayer());
			} else if (cmd.getType() == CommandType.ADIOS) {
				User offUser = users.get(cmd.getSenderKey());
				List<User> restUsers = getRestUsers(cmd.getSenderKey());
				
				Utils.sendOFF(offUser, restUsers);
				
				users.remove(cmd.getSenderKey());
				
				LobbyServer.onPlayerLeave(offUser.getPlayer());
			} else if (cmd.getType() == CommandType.MOVE) {
				MOVE msg = MOVE.process(cmd.getCommand());
				
				User movedUser = users.get(cmd.getSenderKey());
				List<User> restUsers = getRestUsers(cmd.getSenderKey());
				
				movedUser.getPlayer().setPosition(msg.getX(), msg.getY());
				
				Utils.sendMOVE(movedUser, msg.getX(), msg.getY(), restUsers);
				
				LobbyServer.onPlayerMove(movedUser.getPlayer());
			} else if (cmd.getType() == CommandType.CHAT){ 
				CHAT msg = CHAT.process(cmd.getCommand());

				LobbyServer.onPlayerChat(users.get(cmd.getSenderKey()).getPlayer(), msg.getMessage());
			} else if (cmd.getType() == CommandType.ALIVE) {
				timeoutList.remove(cmd.getSenderKey());
			} else {
				ServerConsole.sendMessage(ChatColor.RED + "Algo salio mal: " + cmd.getCommand());
			}
		}
	}
	
	public static List<User> getPlayers() {
		return new ArrayList<>(users.values());
	}
	
	public static void send(DatagramPacket datagram) {
		try {
			s.send(datagram); 
		} catch (Exception e) {
			ServerConsole.sendMessage(ChatColor.RED + e.getMessage());
		}
	}
}