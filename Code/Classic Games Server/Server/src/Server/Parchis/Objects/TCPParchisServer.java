package Server.Parchis.Objects;

import java.io.IOException;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

import org.fusesource.jansi.AnsiConsole;

import CGTP.CommandType;
import CGTP.COMMANDS.PARCHIS.CHAT;
import Server.UserData;
import Server.WindowsConsole;
import Server.Parchis.ParchisServer;
import Server.Parchis.ParchisUtils;

public class TCPParchisServer {
	private static ServerSocket s;
	
	public static Map<String, ParchisUser> users = new HashMap<>();
	
	public static void main(String[] args) {
		AnsiConsole.systemInstall();
		
		ParchisServer.onEnable();
		
		s = ParchisUtils.initSocket(11001);
		WindowsConsole.println("&aServidor inicializado correctamente en el puerto " + s.getLocalPort());
		
		try {
			while(true) {
				onNewConnection(s.accept());
			}
		} catch (IOException e) {
			System.out.println(e.getMessage());
		}
		
		WindowsConsole.println("&4&lEl programa ha finalizado");
	}
	
	public static void onNewConnection(Socket s) { // tengo que hacer lo de HOLA nombre
		ParchisUser user = new ParchisUser(s.getInetAddress(), s.getPort(), new UserData("macintosh"), s);
		
		if (users.size() < 4) {
			user.setPlayer(new Player(ParchisUtils.getNewColor(), user));
			
			Thread th = new Thread(user);
			th.start();
			
			addUser(user.getKey(), user);
		} else {
			ParchisUtils.sendDISCONNECT(user, "Ya hay 4 jugadores en la partida de parchis");
			WindowsConsole.println("&cConexion rechazada " + user.getData().getName() + " (" + user.getKey() + "): Ya hay 4 jugadores");
		}
	}

	public static void processCommand(ParchisUser sender, String message) {
		CommandType cmdType = CommandType.getCommandType(message);
		WindowsConsole.println("Recibido de " + sender.getData().getName() + ": " + message);
		if(cmdType == CommandType.CHAT) {
			CHAT msg = CHAT.process(message);
			
			ParchisServer.onPlayerChat(sender.getPlayer(), msg.getMessage());
		} else {
			WindowsConsole.println("&4Algo salio mal: " + cmdType);
		}
	}
	
	public static void addUser(String key, ParchisUser newUser) {
		ParchisUtils.sendINFO(newUser, new ArrayList<>(users.values()));
		
		users.put(newUser.getKey(), newUser);
		ParchisUtils.sendON(newUser);
		
		WindowsConsole.println("Nueva conexion " + newUser.getData().getName() + " (" + newUser.getKey() + ")");
	}
	
	public static void removeUser(String key) {
		ParchisUtils.sendOFF(key);
		
		users.remove(key);
		
		WindowsConsole.println("Usuario desconectado: " + key);
	}
}
