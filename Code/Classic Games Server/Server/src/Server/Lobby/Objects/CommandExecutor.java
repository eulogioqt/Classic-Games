package Server.Lobby.Objects;

public interface CommandExecutor {
	public boolean onCommand(CommandSender sender, Command cmd, String label, String[] args);
}
