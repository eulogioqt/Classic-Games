package CGTP.COMMANDS;

import java.net.DatagramPacket;
import java.net.InetAddress;
import java.nio.charset.StandardCharsets;

import CGTP.CommandType;

public class COMMAND {
	private String command;
	private InetAddress address;
	private int port;
	
	private CommandType type;
	
	public COMMAND(DatagramPacket dp) {
		this.command = new String(dp.getData(), dp.getOffset(), dp.getLength(), StandardCharsets.UTF_8);
		this.address = dp.getAddress();
		this.port = dp.getPort();
		
		this.type = getCommandType(command);
	}
	
	public String getCommand() {
		return command;
	}
	
	public InetAddress getAddress() {
		return address;
	}
	
	public int getPort() {
		return port;
	}
	
	public CommandType getType() {
		return type;
	}
	
	public String getSenderKey() {
		return address + ";" + port;
	}
	
	private CommandType getCommandType(String command) {
		CommandType t;
		try {
			String str = command.contains(" ") ? command.split(" ")[0] : command;
			t = CommandType.valueOf(str);
		} catch (Exception e) {
			t = CommandType.UNKNOWN;
		}
		return t;
	}
}
