package CGTP;

public class UserData {
	private String name;
	
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
