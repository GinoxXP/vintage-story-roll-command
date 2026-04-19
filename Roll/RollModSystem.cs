using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Roll;

public class RollModSystem : ModSystem
{
    private ICoreServerAPI sapi;
    private Random rand = new Random();
    
    public override bool ShouldLoad(EnumAppSide forSide) => forSide == EnumAppSide.Server;
    
    public override void StartServerSide(ICoreServerAPI api)
    {
        sapi = api;

        sapi.ChatCommands.Create("roll")
            .WithDescription("Get random value")
            .RequiresPrivilege(Privilege.chat)
            .HandleWith(OnCommandDice);
    }
    
    private TextCommandResult OnCommandDice(TextCommandCallingArgs args)
    {
        var player = args.Caller.Player as IServerPlayer;
        if (player == null)
            return TextCommandResult.Error("Player is null");
        
        var randomNumber = rand.Next(1, 101);
        var message = $"{player.PlayerName} Random number: {randomNumber}";
        player.SendMessage(args.Caller.FromChatGroupId, message, EnumChatType.Notification);

        return TextCommandResult.Success();
    }
}
