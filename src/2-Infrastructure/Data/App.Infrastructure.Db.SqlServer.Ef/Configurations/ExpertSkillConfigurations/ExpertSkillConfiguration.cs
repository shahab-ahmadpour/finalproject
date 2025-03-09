using App.Domain.Core.Skills.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Db.SqlServer.Ef.Configurations.ExpertSkillConfigurations
{
    public class ExpertSkillConfiguration : IEntityTypeConfiguration<ExpertSkill>
    {
        public void Configure(EntityTypeBuilder<ExpertSkill> builder)
        {
            builder.HasKey(es => es.Id);

            builder.ToTable("ExpertSkills");

            builder.HasOne(es => es.Expert)
                .WithMany(e => e.ExpertSkills)
                .HasForeignKey(es => es.ExpertId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(es => es.Skill)
                .WithMany(s => s.ExpertSkills)
                .HasForeignKey(es => es.SkillId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasData(
                new ExpertSkill
                {
                    Id = 1,
                    ExpertId = 1,
                    SkillId = 1
                },

                new ExpertSkill
                {
                    Id = 2,
                    ExpertId = 1,
                    SkillId = 2
                },

                new ExpertSkill
                {
                    Id = 3,
                    ExpertId = 2,
                    SkillId = 3
                }


            );

        }
        
    }

}
