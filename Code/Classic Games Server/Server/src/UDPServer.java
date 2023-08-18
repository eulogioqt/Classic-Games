import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.fusesource.jansi.AnsiConsole;

import CGTP.CommandType;
import CGTP.User;
import CGTP.COMMANDS.CHAT;
import CGTP.COMMANDS.COMMAND;
import CGTP.COMMANDS.HOLA;
import CGTP.COMMANDS.MOVE;

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

	private static void onEnable() { // &
		AnsiConsole.systemInstall();
		
		ServerChat.loadChat();
		ServerConsole.sendMessage(ChatColor.DARK_GREEN + "Cargando chat...");
		
		ServerTimer st = new ServerTimer();
		st.start();
		ServerConsole.sendMessage(ChatColor.DARK_GREEN + "Activando timer...");
		
		s = Utils.initSocket(11000);
		ServerConsole.sendMessage(ChatColor.DARK_GREEN + "Inicializando socket...");
		
		ServerConsole sc = new ServerConsole();
		sc.start();
		ServerConsole.sendMessage("Colores: &11&22&33&44&55&66&77&88&99&00&aa&bb&cc&dd&ee&ff");
		ServerConsole.sendMessage(ChatColor.GREEN + "Servidor inicializado correctamente");
	}
	
	public static void main(String[] args) throws IOException {
		onEnable();
		
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
				
				User newUser = new User(dp.getAddress(), dp.getPort(), msg.getData(), msg.getPlayerData());
				List<User> restUsers = getRestUsers(cmd.getSenderKey());
				
				users.put(cmd.getSenderKey(), newUser);
				
				Utils.sendON(newUser, restUsers);
				Utils.sendINFO(newUser, restUsers, ServerChat.getChat());
				
				ServerChat.broadcastMessage("&e" + newUser.getData().getName()  + " joined the game.");
			} else if (cmd.getType() == CommandType.ADIOS) {
				User offUser = users.get(cmd.getSenderKey());
				List<User> restUsers = getRestUsers(cmd.getSenderKey());
				
				Utils.sendOFF(offUser, restUsers);
				
				ServerChat.broadcastMessage("&e" + offUser.getData().getName() + " left the game.");
				users.remove(cmd.getSenderKey());
			} else if (cmd.getType() == CommandType.MOVE) {
				MOVE msg = MOVE.process(cmd.getCommand());
				
				User movedUser = users.get(cmd.getSenderKey());
				List<User> restUsers = getRestUsers(cmd.getSenderKey());
				
				movedUser.getPlayerData().setPosition(msg.getX(), msg.getY());
				
				Utils.sendMOVE(movedUser, msg.getX(), msg.getY(), restUsers);
			} else if (cmd.getType() == CommandType.CHAT){ // Cuando haya cuentas, cambiar el sistema de envio a
				// chatmessage guarda el key y el mensaje y como en minecraft el chat, copiarlo literalmente
				CHAT msg = CHAT.process(cmd.getCommand());

				ServerChat.broadcastMessage("<" + users.get(cmd.getSenderKey()).getData().getName() + "> " + msg.getMessage());
			} else if (cmd.getType() == CommandType.ALIVE) {
				timeoutList.remove(cmd.getSenderKey());
			} else if (cmd.getType() == CommandType.UNKNOWN) {
				ServerConsole.sendMessage(ChatColor.RED + "Algo salio mal: " + cmd.getCommand());
			}
		}
	}
	
	public static void send(DatagramPacket datagram) throws IOException {
		s.send(datagram);
	}
}