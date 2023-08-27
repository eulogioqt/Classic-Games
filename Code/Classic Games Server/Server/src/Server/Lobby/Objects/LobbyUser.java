package Server.Lobby.Objects;

import java.net.InetAddress;

import Server.User;
import Server.UserData;

public class LobbyUser extends User {

	private Player player;
	
	public LobbyUser(InetAddress address, int port, UserData data) {
		super(address, port, data);
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
