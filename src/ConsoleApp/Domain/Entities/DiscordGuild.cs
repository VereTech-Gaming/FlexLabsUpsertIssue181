using ConsoleApp.Domain.Entities.OwnedTypes;

namespace ConsoleApp.Domain.Entities
{
    public class DiscordGuild
    {
        public ulong Id { get; set; }

        public ClanSettings ClanSettings { get; set; } = null!;
    }
}
