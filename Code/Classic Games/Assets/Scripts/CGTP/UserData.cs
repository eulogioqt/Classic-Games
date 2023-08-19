public class UserData {
	private string name;
	
	public UserData(string name) {
		this.name = name;
	}
	
	public string getName() {
		return name;
	}

	public override string ToString() {
		return name;
	}
}
