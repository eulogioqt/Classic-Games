package CGTP;

public enum CommandType {
	ADIOS,
	ALIVE,
	CHAT,
	COMMAND,
	HOLA,
	INFO,
	MOVE,
	OFF,
	ON,
	PING,
	STATUS,
	UNKNOWN;
	
	public static CommandType getCommandType(String command) {
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
