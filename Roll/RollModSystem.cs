using System.Text.RegularExpressions;
using Roll.Randomizer;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace Roll;

public class RollModSystem : ModSystem
{
    private const int MaxRollCount = 10;
    private const int MaxDiceSides = 100;
    
    public static readonly Random Random = new Random();
    
    private ICoreServerAPI _sapi;
    
    public override bool ShouldLoad(EnumAppSide forSide) => forSide == EnumAppSide.Server;
    
    public override void StartServerSide(ICoreServerAPI api)
    {
        _sapi = api;
        var parsers = api.ChatCommands.Parsers;

        _sapi.ChatCommands.Create("roll")
            .WithDescription(
                "/roll - returns (1-100)\n" +
                "/roll [MAX] - returns (1-MAX)\n" +
                "/roll [MIN] [MAX] - returns (MIN-MAX)\n" +
                "Dices: d4, d6, d8, d10, d12, d20\n" +
                "/roll d4 - returns (1-4)")
            .RequiresPrivilege(Privilege.chat)
            .WithArgs( parsers.OptionalWord("string"), parsers.OptionalInt("int1"), parsers.OptionalInt("int2"))
            .HandleWith(OnCommandDice);
    }
    
    private TextCommandResult OnCommandDice(TextCommandCallingArgs args)
    {
        var player = args.Caller.Player as IServerPlayer;
        if (player == null)
            return TextCommandResult.Error("Player is null");

        if (args.Caller.FromChatGroupId == GlobalConstants.GeneralChatGroup)
            return TextCommandResult.Error("You cannot use /roll into global chat. Only groups");
        
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
        
        _sapi.SendMessageToGroup(args.Caller.FromChatGroupId, formattedMessage, EnumChatType.AllGroups);
        
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
            if (int.TryParse(args[0].ToString(), out int result))
            {
                if (result <= 1 || result > MaxDiceSides)
                    throw new Exception($"Invalid argument. Value can be 2-{MaxDiceSides}");
                
                return new MinMaxRandomizer(max: result);
            }

            var arg = args[0].ToString().ToLower();
            var match = Regex.Match(arg, @"^(\d+)?d(\d+)$");
            if (!match.Success)
                throw new Exception("Invalid argument");
            
            string countStr = match.Groups[1].Value;
            int count = string.IsNullOrEmpty(countStr) ? 1 : int.Parse(countStr);
            
            int sides = int.Parse(match.Groups[2].Value);

            if (count > MaxRollCount || count < 1)
                throw new Exception($"Invalid argument. Value can be 1-{MaxRollCount}");
            if (sides > MaxDiceSides || sides <= 1)
                throw new Exception($"Invalid argument. Value can be 1-{MaxDiceSides}");
            
            return new DiceRandomizer(sides, count);
        }

        if (arguments == 2)
        {
            var min = int.Parse(args[0].ToString());
            var max = int.Parse(args[1].ToString());
            
            if (min >= max)
                throw new Exception($"Invalid argument. Min value must be greater than max value");
            
            if (min < 1 || max < 1)
                throw new Exception($"Invalid argument. Value can be 1-{MaxDiceSides}");
            
            return new MinMaxRandomizer(min, max);
        }
        
        if (arguments == 3)
            throw new Exception("Too many arguments");
        
        throw new Exception("Wrong command");
    }
}
