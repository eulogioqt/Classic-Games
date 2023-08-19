using System;
using System.Diagnostics;
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
            str = str.Contains('\n') ? str.Split('\n')[0] : str;

			Enum.TryParse(str, out t);
        } catch (Exception) {
            t = CommandType.UNKNOWN;
		}
		return t;
	}
}
