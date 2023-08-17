import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.SocketException;
import java.nio.charset.StandardCharsets;
import java.util.List;

import CGTP.ChatMessage;
import CGTP.User;
import CGTP.COMMANDS.CHAT;
import CGTP.COMMANDS.INFO;
import CGTP.COMMANDS.MOVE;
import CGTP.COMMANDS.OFF;
import CGTP.COMMANDS.ON;
import CGTP.COMMANDS.TIMEOUT;

public class Utils {
	public static DatagramSocket initSocket(int puerto) {
		DatagramSocket s = null;
		try {
			s = new DatagramSocket(puerto);
		} catch (SocketException e) {
			System.err.println(e.getMessage());
		}
		return s;
	}
	
	public static DatagramPacket createDatagram(String text, User user) {
		DatagramPacket ds = new DatagramPacket(text.getBytes(StandardCharsets.UTF_8),
				text.getBytes(StandardCharsets.UTF_8).length,
				user.getAddress(),
				user.getPort());
		return ds;
	}
	
	public static void sendCHAT(User sender, String message, List<User> restUsers) throws IOException {
		for(User user : restUsers)
			UDPServer.send(Utils.createDatagram(CHAT.getMessage(sender.getKey(), message), user));
	}
	
	public static void sendINFO(User sendTo, List<User> restUsers, List<ChatMessage> chat) throws IOException {
		UDPServer.send(Utils.createDatagram(INFO.getMessage(sendTo.getKey(), restUsers, chat), sendTo)); // send INFO
	}
	
	public static void sendMOVE(User movedUser, int x, int y, List<User> restUsers) throws IOException {
		for(User user : restUsers)
			UDPServer.send(Utils.createDatagram(MOVE.getMessage(movedUser.getKey(), x, y), user));
	}
	
	public static void sendOFF(User offUser, List<User> restUsers) throws IOException {
		for(User user : restUsers)
			UDPServer.send(createDatagram(OFF.getMessage(offUser.getKey()), user));
	}
	
	public static void sendON(User onUser, List<User> restUsers) throws IOException {
		for(User user : restUsers) // send ON
			UDPServer.send(createDatagram(ON.getMessage(onUser), user));
	}
	
	public static void sendTIMEOUT(User sendTo) throws IOException {
		UDPServer.send(createDatagram(TIMEOUT.getMessage(), sendTo));
	}
}
