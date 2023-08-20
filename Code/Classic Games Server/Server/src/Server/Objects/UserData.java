package Server.Objects;

public class UserData {
	public String name;
	
	public UserData(String name) {
		this.name = name;
	}
	
	public String getName() {
		return name;
	}
	
	@Override
	public String toString() {
		return name;
	}
}
