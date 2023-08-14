import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.SocketException;
import java.nio.charset.StandardCharsets;
import java.util.HashSet;
import java.util.Iterator;
import java.util.Set;

public class UDPServer {
	
	// QUE CADA VEZ QUE SE CONECTE ALGUIEN, SE CREE UN THREAD ESCUCHANDO AL PUERTO DEL QUE SE HA CONECTADO
	// DE ESTA FORMA, SE MEJORA EL RENDIMIENTO
	// TAMBIEN QUE CUANDO VAYA A ENVIAR UN MENSAJE
	
	// CADA CIERTO TIEMPO, ENVIAR UN MENSAJE AL USUARIO Y SI NO ESTA, DESCONECTARLO
	
	// QUE EL SERVIDOR RESPONDA CUANDO SE CONECTA ALGUIEN, PROTOCOLO ETC

	private static Set<User> users;
	
	private static User getUser(InetAddress address, int port) {
		User user = new User(address, port);
		Iterator<User> iter = users.iterator();
		
		boolean found = false;
		while(iter.hasNext() && !found) {
			User u = iter.next();
			if(user.equals(u)) {
				found = true;
				user = u;
			}
		}
		return user;
	}
	
	public static void main(String[] args) throws IOException {
		users = new HashSet<User>();
		
		DatagramSocket s = null;
		try {
			s = new DatagramSocket(11000);
		} catch (SocketException e) {
			System.err.println(e.getMessage());
		}

		byte[] buffer;
		while (true) {
			buffer = new byte[800];

			DatagramPacket dp = new DatagramPacket(buffer, buffer.length);

			System.out.println("- Esperando recibir algun mensaje");
			s.receive(dp);

			String texto = new String(dp.getData(), dp.getOffset(), dp.getLength(), StandardCharsets.UTF_8);
			User thisUser = getUser(dp.getAddress(), dp.getPort());
			
			if(texto.equals("HOLA")) {
				users.add(thisUser);
				System.out.println("- Nuevo usuario conectado: " + thisUser.getAddress() + ":" + thisUser.getPort() + " - Online: " + users.size());
			} else if (texto.equals("ADIOS")) {
				users.remove(thisUser);
				System.out.println("- Usuario se desconecto: " + thisUser.getAddress() + ":" + thisUser.getPort() + " - Online: " + users.size());
			} else {
				System.out.println("Mensaje " + texto + " (recibido desde: " + dp.getAddress() + ":" + dp.getPort() + ")");
				
				Set<User> sendTo = new HashSet<User>(users);
				sendTo.remove(thisUser);
				for(User user : sendTo) {
					DatagramPacket ds = new DatagramPacket(texto.getBytes(StandardCharsets.UTF_8), // Datos
							texto.getBytes(StandardCharsets.UTF_8).length, // Longitud de los datos
							user.getAddress(), // IP del servidor
							user.getPort() // puerto del servidor
					);
					
					s.send(ds);
				}
				
				System.out.println("- Enviando a todos los usuarios");
			}
		}
	}
}