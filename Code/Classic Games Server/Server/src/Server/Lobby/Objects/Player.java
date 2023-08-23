package Server.Lobby.Objects;

import java.util.ArrayList;
import java.util.List;

import Server.Objects.User;
import Server.Objects.Utils;

public class Player implements CommandSender {
	private int x;
	private int y;
	
	private User user;
	
	public Player(int x, int y, User user) {
		this.x = x;
		this.y = y;
		
		this.user = user;
	}
	
	public int getX() {
		return x;
	}
	
	public int getY() {
		return y;
	}
	
	public User getUser() {
		return user;
	}
	
	@Override
	public String toString() {
		return x + ";" + y;
	}
	
	protected void setPosition(int x, int y) {
		this.x = x;
		this.y = y;
	}
	
	public void teleport(int x, int y) {
		setPosition(x, y);
		Utils.sendMOVE(user, x, y, new ArrayList<>(UDPLobbyServer.users.values()));
	}

	@Override
	public String getName() {
		return user.getData().getName();
	}
	
	@Override
	public void sendMessage(String message) {
		List<User> list = new ArrayList<>();
		list.add(user);
		Utils.sendCHAT(message, list);
	}
}
