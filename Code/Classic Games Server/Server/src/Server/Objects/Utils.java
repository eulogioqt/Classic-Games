package Server.Objects;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.SocketException;
import java.nio.charset.StandardCharsets;
import java.util.List;

import CGTP.COMMANDS.ALIVE;
import CGTP.COMMANDS.CHAT;
import CGTP.COMMANDS.INFO;
import CGTP.COMMANDS.MOVE;
import CGTP.COMMANDS.OFF;
import CGTP.COMMANDS.ON;
import CGTP.COMMANDS.PING;
import CGTP.COMMANDS.DISCONNECT;
import CGTP.COMMANDS.STATUS;
import Server.Lobby.LobbyServer;
import Server.Lobby.Objects.UDPLobbyServer;

public class Utils {
	public static DatagramSocket initSocket(int puerto) {
		DatagramSocket s = null;
		
		int i = 0;
		while(s == null) {
			try {
				s = new DatagramSocket(puerto + i);
			} catch (SocketException e) {
				i++;
			}
		}
		return s;
	}
	
	public static DatagramPacket createDatagram(String text, User user) {
		return createDatagram(text, user.getAddress(), user.getPort());
	}
	
	public static DatagramPacket createDatagram(String text, InetAddress address, int port) {
		byte[] bytes = text.getBytes(StandardCharsets.UTF_8);
		DatagramPacket ds = new DatagramPacket(
				bytes,
				bytes.length,
				address,
				port);
		return ds;
	}
	
	public static void sendALIVE(User sendTo) {
		UDPLobbyServer.send(createDatagram(ALIVE.getMessage(), sendTo));
	}
	
	public static void sendCHAT(String message, List<User> users) {
		for(User user : users)
			UDPLobbyServer.send(Utils.createDatagram(CHAT.getMessage(message), user));
	}

	public static void sendDISCONNECT(User offUser, String msg) {
		UDPLobbyServer.send(createDatagram(DISCONNECT.getMessage(msg), offUser));
	}
	
	public static void sendINFO(User sendTo, List<User> restUsers, List<String> chat) {
		UDPLobbyServer.send(Utils.createDatagram(INFO.getMessage(sendTo.getKey(), restUsers, chat), sendTo));
	}
	
	public static void sendMOVE(User movedUser, int x, int y, List<User> restUsers) {
		for(User user : restUsers)
			UDPLobbyServer.send(Utils.createDatagram(MOVE.getMessage(movedUser.getKey(), x, y), user));
	}
	
	public static void sendOFF(User offUser, List<User> restUsers) {
		for(User user : restUsers)
			UDPLobbyServer.send(createDatagram(OFF.getMessage(offUser.getKey()), user));
	}
	
	public static void sendON(User onUser, List<User> restUsers) {
		for(User user : restUsers) // send ON
			UDPLobbyServer.send(createDatagram(ON.getMessage(onUser), user));
	}
	
	public static void sendPING(InetAddress address, int port, int onlinePlayers, String version, String MOTD) {
		UDPLobbyServer.send(createDatagram(PING.getMessage(onlinePlayers, version, MOTD), address, port));
	}
	
	public static void sendSTATUS(User sendTo) {
		UDPLobbyServer.send(createDatagram(STATUS.getMessage(), sendTo));
	}
	
	public static String argsToString(String[] args) {
		String msg = "";
		for(String arg : args)
			msg += arg + " ";
		msg = msg.substring(0, msg.length() - 1);
		return msg;
	}
	
	public static void sleep(long n) {
		try {
			Thread.sleep(n);
		} catch (InterruptedException e) {
			LobbyServer.getServerConsole().sendMessage(ChatColor.RED + "Ha habido algun problema durmiendo a la hebra de los comandos: " + e.getMessage());
		}
	}
}
