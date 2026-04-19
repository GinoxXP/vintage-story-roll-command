using Roll.Randomizer;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Roll;

public class RollModSystem : ModSystem
{
    private ICoreServerAPI sapi;
    
    public override bool ShouldLoad(EnumAppSide forSide) => forSide == EnumAppSide.Server;
    
    public override void StartServerSide(ICoreServerAPI api)
    {
        sapi = api;
        var parsers = api.ChatCommands.Parsers;

        sapi.ChatCommands.Create("roll")
            .WithDescription("/roll - returns (1-100)\n/roll [MAX] - returns (1-MAX)\n/roll [MIN] [MAX] - returns (MIN-MAX)")
            .RequiresPrivilege(Privilege.chat)
            .WithArgs(parsers.OptionalInt("arg1"), parsers.OptionalInt("arg2"))
            .HandleWith(OnCommandDice);
    }
    
    private TextCommandResult OnCommandDice(TextCommandCallingArgs args)
    {
        var player = args.Caller.Player as IServerPlayer;
        if (player == null)
            return TextCommandResult.Error("Player is null");

        IRandomizer randomizer;
        try
        {
            randomizer = GetRandomizer(args);
        }
        catch (Exception ex)
        {
            return TextCommandResult.Error(ex.Message);
        }

        var formattedMessage =
            $"<font color=#ffaa00><strong>{player.PlayerName} rolls ({randomizer.Name}): {randomizer.GetRandomValue()}</strong></font>";
        
        sapi.SendMessageToGroup(args.Caller.FromChatGroupId, formattedMessage, EnumChatType.AllGroups);
        
        return TextCommandResult.Success();
    }

    private IRandomizer GetRandomizer(TextCommandCallingArgs args)
    {
        var arguments = args.Parsers.Count(a => !a.IsMissing);
        
        if (arguments == 0)
        {
            return new MinMaxRandomizer();
        }
        
        if (arguments == 1)
        {
            return new MinMaxRandomizer(max: (int)args[0]);
        }

        if (arguments == 2)
        {
            return new MinMaxRandomizer((int)args[0], (int)args[1]);
        }
        
        if (arguments == 3)
            throw new Exception("Too many arguments");
        
        throw new Exception("Wrong command");
    }
}
