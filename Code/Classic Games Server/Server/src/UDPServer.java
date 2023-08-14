import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.SocketException;
import java.nio.charset.StandardCharsets;
import java.util.Collection;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Iterator;
import java.util.Map;
import java.util.Set;

public class UDPServer {
	
	// QUE CADA VEZ QUE SE CONECTE ALGUIEN, SE CREE UN THREAD ESCUCHANDO AL PUERTO DEL QUE SE HA CONECTADO
	// DE ESTA FORMA, SE MEJORA EL RENDIMIENTO
	// TAMBIEN QUE CUANDO VAYA A ENVIAR UN MENSAJE
	
	// CADA CIERTO TIEMPO, ENVIAR UN MENSAJE AL USUARIO Y SI NO ESTA, DESCONECTARLO
	
	// QUE EL SERVIDOR RESPONDA CUANDO SE CONECTA ALGUIEN, PROTOCOLO ETC

	// GUARDAR HISTORIAL DE CHAT
	
	// USUARIOS
	
	private static Map<String, User> users;
	
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
	
	public static void main(String[] args) throws IOException {
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
			
			if(texto.equals("HOLA")) {
				users.put(key, new User(dp.getAddress(), dp.getPort()));
				
				String ON = "ON" + key; // añadir datos de usuario, nombre, color cosas asi
				String INFO = "INFO";
				for(User user : getRestUsers(key)) {
					s.send(createDatagram(ON, user));
					INFO += user.getAddress() + ":" + user.getPort() + ";" + user.getX() + ";" + user.getY() + "@";
				}
				INFO = INFO.substring(0, INFO.length() - 1);
				s.send(createDatagram(INFO, users.get(key)));
				
				System.out.println("- Nuevo usuario conectado: " + dp.getAddress() + ":" + dp.getPort() + " - Online: " + users.size());
			} else if (texto.equals("ADIOS")) {
				users.remove(key);
				
				String OFF = "OFF" + key;
				for(User user : getRestUsers(key))
					s.send(createDatagram(OFF, user));
				
				System.out.println("- Usuario se desconecto: " + dp.getAddress() + ":" + dp.getPort() + " - Online: " + users.size());
			} else if (texto.startsWith("CHAT") || texto.startsWith("GAME")){
				System.out.println("Mensaje " + texto + " (recibido desde: " + dp.getAddress() + ":" + dp.getPort() + ")");
				
				if(texto.startsWith("GAME")) {
					String data[] = texto.substring(4).split(";");
					users.get(key).setPosition(Integer.parseInt(data[0]), Integer.parseInt(data[1]));
				}
				
				texto += "@" + key;
				
				for(User user : getRestUsers(key))
					s.send(createDatagram(texto, user));
				
				System.out.println("- Enviando a todos los usuarios");
			}
		}
	}
}