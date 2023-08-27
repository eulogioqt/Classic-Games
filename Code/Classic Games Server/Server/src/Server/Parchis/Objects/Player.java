package Server.Parchis.Objects;

public class Player {

	private TeamColor color;
	
	private ParchisUser user;
	
	public Player(TeamColor color, ParchisUser user) {
		this.color = color;
		this.user = user;
	}
	
	public ParchisUser getUser() {
		return user;
	}
	
	public String getName() {
		return user.getData().getName();
	}
	
	public TeamColor getColor() {
		return color;
	}
	
	@Override
	public String toString() {
		return color.name();
	}
}
