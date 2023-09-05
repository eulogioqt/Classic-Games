package Server.Parchis.Objects;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.InetAddress;
import java.net.Socket;
import java.nio.charset.StandardCharsets;

import Server.User;
import Server.UserData;
import Server.WindowsConsole;

public class ParchisUser extends User implements Runnable {
	
	private Socket s;
	private BufferedReader in;
	private PrintWriter out;
	
	private Player player;
	
	public ParchisUser(InetAddress address, int port, UserData data, Socket s) {
		super(address, port, data);
		
		try {
			this.s = s;
	        this.in = new BufferedReader(new InputStreamReader(s.getInputStream(), StandardCharsets.UTF_8));
	        this.out = new PrintWriter(s.getOutputStream(), true, StandardCharsets.UTF_8);
		} catch (IOException e) {
			WindowsConsole.println("&4" + e.getMessage());
		}
	}
	
	@Override
	public void run() {
		try {
			String msg;
			while((msg = in.readLine()) !=null) {
				TCPParchisServer.processCommand(this, msg);
			}
		} catch (IOException e) {
			WindowsConsole.println("&4erro: " + e.getMessage());
		}

		this.close();
	}
	
	private void close() {
		TCPParchisServer.removeUser(super.getKey());
		
		try {
			in.close();
			out.close();
			s.close();
		} catch (IOException e) {
			WindowsConsole.println("&4"+e.getMessage());
		}
	}
	
	public void send(String message) {
		WindowsConsole.println(super.getData().getName() + " sent: " + message);
		out.println(message);
	}
	
	public Player getPlayer() {
		return player;
	}
	
	public void setPlayer(Player player) {
		this.player = player;
	}
	
	@Override
	public String toString() {
		return super.getData() + ";" + player;
	}
}
