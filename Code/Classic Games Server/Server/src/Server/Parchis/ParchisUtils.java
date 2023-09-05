package Server.Parchis;

import java.io.IOException;
import java.net.ServerSocket;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Random;

import CGTP.COMMANDS.DISCONNECT;
import CGTP.COMMANDS.OFF;
import CGTP.COMMANDS.PARCHIS.ON;
import CGTP.COMMANDS.PARCHIS.CHAT;
import CGTP.COMMANDS.PARCHIS.INFO;
import Server.Parchis.Objects.ParchisUser;
import Server.Parchis.Objects.TCPParchisServer;
import Server.Parchis.Objects.TeamColor;

public class ParchisUtils {
	public static ServerSocket initSocket(int port) {
		int i = 0;
		ServerSocket s = null;
		while(s == null) {
			try {
				s = new ServerSocket(11001 + i, 1);
			} catch (IOException e) {
				i++;
			}
		}
		return s;
	}
	
	public static List<ParchisUser> getRestUsers(String key) {
		Map<String, ParchisUser> sendTo = new HashMap<>(TCPParchisServer.users);
		sendTo.remove(key);
		return new ArrayList<>(sendTo.values());
	}
	
	public static TeamColor getNewColor() {
		List<TeamColor> actualColors = new ArrayList<>();
		for(ParchisUser user : TCPParchisServer.users.values())
			actualColors.add(user.getPlayer().getColor());
		
		Random rnd = new Random();
		TeamColor color;
		do {
			color = TeamColor.values()[rnd.nextInt(4)];
		} while (actualColors.contains(color));
		
		return color;
	}
	
	
	
	public static void sendINFO(ParchisUser sendTo, List<ParchisUser> users) {
		sendTo.send(INFO.getMessage(sendTo.getPlayer().getColor(), users));
	}
	
	public static void sendON(ParchisUser newUser) {
		for(ParchisUser user : ParchisUtils.getRestUsers(newUser.getKey()))
			user.send(ON.getMessage(newUser));
	}
	
	public static void sendOFF(String newUserKey) {
		for(ParchisUser user : ParchisUtils.getRestUsers(newUserKey))
			user.send(OFF.getMessage(newUserKey));
	}
	
	public static void sendCHAT(ParchisUser sender, String message) {
		for(ParchisUser user : TCPParchisServer.users.values())
			user.send(CHAT.getMessage(sender.getKey(), message));
	}
	
	public static void sendDISCONNECT(ParchisUser sendTo, String message) {
		sendTo.send(DISCONNECT.getMessage(message));
	}
}
