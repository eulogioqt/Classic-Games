import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.SocketException;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.Collection;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class UDPServer {
	
	// QUE CADA VEZ QUE SE CONECTE ALGUIEN, SE CREE UN THREAD ESCUCHANDO AL PUERTO DEL QUE SE HA CONECTADO
	// DE ESTA FORMA, SE MEJORA EL RENDIMIENTO
	// TAMBIEN QUE CUANDO VAYA A ENVIAR UN MENSAJE
	
	// CADA CIERTO TIEMPO, ENVIAR UN MENSAJE AL USUARIO Y SI NO ESTA, DESCONECTARLO
	
	// QUE EL SERVIDOR RESPONDA CUANDO SE CONECTA ALGUIEN, PROTOCOLO ETC

	// GUARDAR HISTORIAL DE CHAT
	
	// USUARIOS
	
	private static Map<String, User> users;
	private static List<ChatMessage> chat;
	// reemplazar por un bojeto propio que redefina el add del list con un booleano de updated
	
	private static DatagramSocket initSocket(int puerto) {
		DatagramSocket s = null;
		try {
			s = new DatagramSocket(puerto);
		} catch (SocketException e) {
			System.err.println(e.getMessage());
		}
		return s;
	}
	
	private static DatagramPacket createDatagram(String text, User user) {
		DatagramPacket ds = new DatagramPacket(text.getBytes(StandardCharsets.UTF_8),
				text.getBytes(StandardCharsets.UTF_8).length,
				user.getAddress(),
				user.getPort());
		return ds;
	}
	
	private static Collection<User> getRestUsers(String key) {
		Map<String, User> sendTo = new HashMap<>(users);
		sendTo.remove(key);
		return sendTo.values();
	}
	
	private static void saveTimer() {
		Thread timer = new Thread() {
			@Override
			public void run() {
				while(true) {
					try {
						Thread.sleep(10000);
						
						saveChat();
						System.out.println("- Chat guardado");
					} catch (InterruptedException | IOException e) {
						e.printStackTrace();
					}
				}
			}
		};
		
		timer.start();
	}
	
	private static void saveChat() throws IOException {
		FileOutputStream fos = new FileOutputStream("chat.txt");
		ObjectOutputStream oos = new ObjectOutputStream(fos);
		oos.writeObject(chat);
		oos.close();
	}
	
	@SuppressWarnings("unchecked")
	private static void loadChat () {
		try {
			FileInputStream fis = new FileInputStream("chat.txt");
			ObjectInputStream ois = new ObjectInputStream(fis);
			chat = (ArrayList<ChatMessage>) ois.readObject();
			ois.close();
		} catch (IOException | ClassNotFoundException e) {
			System.err.println("No se ha encontrado historial de chat");
		}
		
		if(chat == null)
			chat = new ArrayList<>();
	}
	
	public static void main(String[] args) throws IOException {
		loadChat();
		saveTimer();
		
		users = new HashMap<>();
		
		DatagramSocket s = initSocket(11000);

		byte[] buffer;
		while (true) {
			buffer = new byte[800];

			DatagramPacket dp = new DatagramPacket(buffer, buffer.length);

			System.out.println("- Esperando recibir algun mensaje");
			s.receive(dp);

			String texto = new String(dp.getData(), dp.getOffset(), dp.getLength(), StandardCharsets.UTF_8);
			
			String key = dp.getAddress() + ":" + dp.getPort();
			
			if(texto.startsWith("HOLA")) {
				texto = texto.substring(4);
				
				users.put(key, new User(dp.getAddress(), dp.getPort(), texto));
				
				String ON = "ON " + key + " " + texto; // añadir datos de usuario, nombre, color cosas asi
				String INFO = "INFO\n";
				for(User user : getRestUsers(key)) {
					s.send(createDatagram(ON, user)); // Envia el ON al resto
					
					INFO += user.getAddress() + ":" + user.getPort() + " " + user.getName() + ";" + user.getX() + ";" + user.getY() + "\n";
				}
				INFO += ".\n";
				
				for(ChatMessage message : chat) { // añade el chat al cuerpo del mensaje INFO
					INFO += message.getSender() + ";" + message.getMessage() + "\n";
				}
				INFO += ".";
				
				s.send(createDatagram(INFO, users.get(key)));
				
				chat.add(new ChatMessage(users.get(key).getName() ,"joined the server"));
				System.out.println("- Nuevo usuario conectado: " + dp.getAddress() + ":" + dp.getPort() + " - Online: " + users.size());
			} else if (texto.startsWith("ADIOS")) {
				chat.add(new ChatMessage(users.get(key).getName() ,"left the server"));
				users.remove(key);
				
				String OFF = "OFF" + key;
				for(User user : getRestUsers(key))
					s.send(createDatagram(OFF, user));

				System.out.println("- Usuario se desconecto: " + dp.getAddress() + ":" + dp.getPort() + " - Online: " + users.size());
			} else if (texto.startsWith("GAME")) {
				System.out.println("Mensaje " + texto + " (recibido desde: " + dp.getAddress() + ":" + dp.getPort() + ")");
				
				String data[] = texto.substring(4).split(";");
				users.get(key).setPosition(Integer.parseInt(data[0]), Integer.parseInt(data[1]));

				texto += "@" + key;
				
				for(User user : getRestUsers(key))
					s.send(createDatagram(texto, user));
				
				System.out.println("- Enviando a todos los usuarios");
			} else if (texto.startsWith("CHAT")){
				System.out.println("Mensaje " + texto + " (recibido desde: " + dp.getAddress() + ":" + dp.getPort() + ")");

				chat.add(new ChatMessage(users.get(key).getName(), texto.substring(5)));
				texto = "CHAT " + key + " " + texto.substring(5);
				
				for(User user : getRestUsers(key))
					s.send(createDatagram(texto, user));
				
				System.out.println("- Enviando a todos los usuarios");
			}
		}
	}
}