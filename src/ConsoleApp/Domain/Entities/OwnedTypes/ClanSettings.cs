using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Domain.Entities.OwnedTypes
{
    public class ClanSettings
    {
        public required DiscordGuild Owner { get; set; }

        public int MaxMembers { get; set; }
    }
}
