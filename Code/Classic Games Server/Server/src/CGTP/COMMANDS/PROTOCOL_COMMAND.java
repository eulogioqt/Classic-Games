package CGTP.COMMANDS;

import java.net.DatagramPacket;
import java.net.InetAddress;
import java.nio.charset.StandardCharsets;

import CGTP.CommandType;

public class PROTOCOL_COMMAND {
	private String command;
	private InetAddress address;
	private int port;
	
	private CommandType type;
	
	public PROTOCOL_COMMAND(DatagramPacket dp) {
		this.command = new String(dp.getData(), dp.getOffset(), dp.getLength(), StandardCharsets.UTF_8);
		this.address = dp.getAddress();
		this.port = dp.getPort();
		
		this.type = CommandType.getCommandType(command);
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
		return address + ":" + port;
	}
}
