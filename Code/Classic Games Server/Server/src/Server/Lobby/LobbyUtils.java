package Server.Lobby;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.SocketException;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import CGTP.COMMANDS.ALIVE;
import CGTP.COMMANDS.OFF;
import CGTP.COMMANDS.PING;
import CGTP.COMMANDS.DISCONNECT;
import CGTP.COMMANDS.STATUS;
import CGTP.COMMANDS.LOBBY.CHAT;
import CGTP.COMMANDS.LOBBY.INFO;
import CGTP.COMMANDS.LOBBY.MOVE;
import CGTP.COMMANDS.LOBBY.ON;
import Server.ChatColor;
import Server.Lobby.Objects.LobbyUser;
import Server.Lobby.Objects.UDPLobbyServer;

public class LobbyUtils {
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
	
	public static List<LobbyUser> getRestUsers(String key) {
		Map<String, LobbyUser> sendTo = new HashMap<>(UDPLobbyServer.users);
		sendTo.remove(key);
		return new ArrayList<>(sendTo.values());
	}
	
	public static DatagramPacket createDatagram(String text, LobbyUser user) {
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
	
	public static void sendALIVE(LobbyUser sendTo) {
		UDPLobbyServer.send(createDatagram(ALIVE.getMessage(), sendTo));
	}
	
	public static void sendCHAT(String message, List<LobbyUser> users) {
		for(LobbyUser user : users)
			UDPLobbyServer.send(LobbyUtils.createDatagram(CHAT.getMessage(message), user));
	}

	public static void sendDISCONNECT(LobbyUser offUser, String msg) {
		UDPLobbyServer.send(createDatagram(DISCONNECT.getMessage(msg), offUser));
	}
	
	public static void sendINFO(LobbyUser sendTo, List<LobbyUser> restUsers, List<String> chat) {
		UDPLobbyServer.send(LobbyUtils.createDatagram(INFO.getMessage(restUsers, chat), sendTo));
	}
	
	public static void sendMOVE(LobbyUser movedUser, int x, int y, List<LobbyUser> restUsers) {
		for(LobbyUser user : restUsers)
			UDPLobbyServer.send(LobbyUtils.createDatagram(MOVE.getMessage(movedUser.getKey(), x, y), user));
	}
	
	public static void sendOFF(LobbyUser offUser, List<LobbyUser> restUsers) {
		for(LobbyUser user : restUsers)
			UDPLobbyServer.send(createDatagram(OFF.getMessage(offUser.getKey()), user));
	}
	
	public static void sendON(LobbyUser onUser, List<LobbyUser> restUsers) {
		for(LobbyUser user : restUsers) // send ON
			UDPLobbyServer.send(createDatagram(ON.getMessage(onUser), user));
	}
	
	public static void sendPING(InetAddress address, int port, int onlinePlayers, String version, String MOTD) {
		UDPLobbyServer.send(createDatagram(PING.getMessage(onlinePlayers, version, MOTD), address, port));
	}
	
	public static void sendSTATUS(LobbyUser sendTo) {
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
