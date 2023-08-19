using System;
using System.Text;

public class COMMAND {
	private string command;
	private CommandType type;
	
	public COMMAND(byte[] dp) {
		this.command = Encoding.ASCII.GetString(dp);;
		
		this.type = getCommandType(command);
	}
	
	public string getCommand() {
		return command;
	}
	
	public CommandType getType() {
		return type;
	}
	
	private CommandType getCommandType(string command) {
        CommandType t;
        try {
			string str = command.Contains(" ") ? command.Split(" ")[0] : command;
			t = (CommandType) Enum.Parse(typeof(CommandType), str);
		} catch (Exception) {
			t = CommandType.UNKNOWN;
		}
		return t;
	}
}
