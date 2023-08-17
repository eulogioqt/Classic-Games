import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import CGTP.ChatMessage;
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
	
	// CADA CIERTO TIEMPO, ENVIAR UN MENSAJE AL USUARIO Y SI NO ESTA, DESCONECTARLO
	
	// QUE EL SERVIDOR RESPONDA CUANDO SE CONECTA ALGUIEN, PROTOCOLO ETC

	// GUARDAR HISTORIAL DE CHAT
	
	// USUARIOS
	
	public static Map<String, User> users;
	public static List<ChatMessage> chat;
	// reemplazar por un bojeto propio que redefina el add del list con un booleano de updated
	public static List<String> timeoutList;
	
	private static DatagramSocket s;
	
	public static List<User> getRestUsers(String key) {
		Map<String, User> sendTo = new HashMap<>(users);
		sendTo.remove(key);
		return new ArrayList<>(sendTo.values());
	}
	
	public static void saveChat() throws IOException {
		FileOutputStream fos = new FileOutputStream("chat.txt");
		ObjectOutputStream oos = new ObjectOutputStream(fos);
		oos.writeObject(chat);
		oos.close();
		
		System.out.println("- Chat guardado");
	}
	
	@SuppressWarnings("unchecked")
	public static void loadChat () {
		try {
			FileInputStream fis = new FileInputStream("chat.txt");
			ObjectInputStream ois = new ObjectInputStream(fis);
			chat = (ArrayList<ChatMessage>) ois.readObject();
			ois.close();
		} catch (IOException | ClassNotFoundException e) {
			System.err.println("No se ha encontrado historial de chat.");
		}
		
		if(chat == null)
			chat = new ArrayList<>();
	}
	
	public static void main(String[] args) throws IOException {
		loadChat();
		
		ServerTimer st = new ServerTimer();
		st.start();
		
		users = new HashMap<>();
		s = Utils.initSocket(11000);

		byte[] buffer;
		while (true) {
			buffer = new byte[800];

			DatagramPacket dp = new DatagramPacket(buffer, buffer.length);
			s.receive(dp);

			COMMAND cmd = new COMMAND(dp);
			if(cmd.getType() == CommandType.HOLA) {
				HOLA msg = HOLA.process(cmd.getCommand());
				
				User newUser = new User(dp.getAddress(), dp.getPort(), msg.getData(), msg.getPlayerData());
				List<User> restUsers = getRestUsers(cmd.getSenderKey());
				
				users.put(cmd.getSenderKey(), newUser);
				
				Utils.sendON(newUser, restUsers);
				Utils.sendINFO(newUser, restUsers, chat);
				
				chat.add(new ChatMessage(newUser.getData().getName() ,"joined the server"));
			} else if (cmd.getType() == CommandType.ADIOS) {
				User offUser = users.get(cmd.getSenderKey());
				List<User> restUsers = getRestUsers(cmd.getSenderKey());
				
				Utils.sendOFF(offUser, restUsers);
				
				chat.add(new ChatMessage(offUser.getData().getName() ,"left the server"));
				users.remove(cmd.getSenderKey());
			} else if (cmd.getType() == CommandType.MOVE) {
				MOVE msg = MOVE.process(cmd.getCommand());
				
				User movedUser = users.get(cmd.getSenderKey());
				List<User> restUsers = getRestUsers(cmd.getSenderKey());
				
				movedUser.getPlayerData().setPosition(msg.getX(), msg.getY());
				
				Utils.sendMOVE(movedUser, msg.getX(), msg.getY(), restUsers);
			} else if (cmd.getType() == CommandType.CHAT){
				CHAT msg = CHAT.process(cmd.getCommand());
				
				User movedUser = users.get(cmd.getSenderKey());
				List<User> restUsers = getRestUsers(cmd.getSenderKey());
				
				Utils.sendCHAT(movedUser, msg.getMessage(), restUsers);
				
				chat.add(new ChatMessage(users.get(cmd.getSenderKey()).getData().getName(), msg.getMessage()));
			} else if (cmd.getType() == CommandType.ALIVE) {
				timeoutList.remove(cmd.getSenderKey());
			}
		}
	}
	
	public static void send(DatagramPacket datagram) throws IOException {
		s.send(datagram);
	}
}