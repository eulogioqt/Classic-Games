package CGTP.COMMANDS;

public class CHAT {
	private String message;
	
	private CHAT(String message) {
		this.message = message;
	}
	
	public String getMessage() {
		return message;
	}
	
	public static CHAT process(String message) {
		return new CHAT(message.substring(5));
	}
	
	public static String getMessage(String message) {
		return "CHAT " + message;
	}
}
