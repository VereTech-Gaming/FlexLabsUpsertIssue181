using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Domain.Entities.Configuration
{
    public class DiscordGuildConfiguration : IEntityTypeConfiguration<DiscordGuild>
    {
        public void Configure(EntityTypeBuilder<DiscordGuild> entity)
        {
            entity.HasKey(x => x.Id);

            entity.OwnsOne(x => x.ClanSettings, clanSettingsBuilder =>
            {
                clanSettingsBuilder.Property(x => x.MaxMembers).HasDefaultValue(5);
            });
        }
    }
}
