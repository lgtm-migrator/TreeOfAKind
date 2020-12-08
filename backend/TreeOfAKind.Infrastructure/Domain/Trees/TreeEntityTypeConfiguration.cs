﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TreeOfAKind.Application.Configuration;
using TreeOfAKind.Domain.Trees;
using TreeOfAKind.Infrastructure.Database;

namespace TreeOfAKind.Infrastructure.Domain.Trees
{
    public class TreeEntityTypeConfiguration : IEntityTypeConfiguration<Tree>
    {
        public void Configure(EntityTypeBuilder<Tree> builder)
        {
            builder.ToTable("Trees", SchemaNames.Trees);

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .HasMaxLength(StringLengths.Short);

            builder.Property(t => t.Photo)
                .HasConversion<string?>()
                .HasMaxLength(StringLengths.Short);

            builder.OwnsMany(t => t.TreeOwners, b =>
            {
                b.ToTable("TreeUserProfile", SchemaNames.Trees);
                b.HasKey(to => new {to.TreeId, to.UserId});
            });

            builder.OwnsMany(t => t.People, b =>
            {
                b.ToTable("People", SchemaNames.Trees);

                b.HasKey(p => p.Id);

                b.WithOwner(p => p.Tree);

                b.Property(p => p.Name)
                    .HasMaxLength(StringLengths.VeryShort);

                b.Property(p => p.LastName)
                    .HasMaxLength(StringLengths.VeryShort);

                b.Property(p => p.Description)
                    .HasMaxLength(StringLengths.Short);

                b.Property(p => p.Biography)
                    .HasMaxLength(StringLengths.VeryLong);

                b.Property(p => p.BirthDate)
                    .HasColumnType("date");

                b.Property(p => p.DeathDate)
                    .HasColumnType("date");

                b.Property(p => p.Gender)
                    .HasConversion<string>()
                    .HasMaxLength(StringLengths.VeryShort);
            });

            builder.OwnsOne(t => t.TreeRelations, b =>
            {
                b.OwnsMany(r => r.Relations, b =>
                {
                    b.ToTable("TreeRelations", SchemaNames.Trees);

                    b.WithOwner()
                        .HasForeignKey(r => r.TreeId);

                    b.HasKey(r => new {r.From, r.To, r.RelationType});

                    b.Property(r => r.RelationType)
                        .HasConversion<string>()
                        .HasMaxLength(StringLengths.VeryShort);
                });
            });
        }
    }
}
