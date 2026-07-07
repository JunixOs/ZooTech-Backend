using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZooTech.Infrastructure.Persistence.Models;

public class VacunoRawRowConfiguration : IEntityTypeConfiguration<VacunoRawRow>
{
    public void Configure(EntityTypeBuilder<VacunoRawRow> builder)
    {
        builder.HasNoKey();
        builder.ToView(null);
    }
}
