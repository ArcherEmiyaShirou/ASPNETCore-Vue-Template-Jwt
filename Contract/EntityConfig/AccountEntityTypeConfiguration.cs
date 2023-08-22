using Backend.Contract.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Backend.Common.Utills;
using Backend.Common.Utills.Contract;

namespace Backend.Contract.EntityConfig
{
    internal class AccountEntityTypeConfiguration:IEntityTypeConfiguration<Account>
    {
        private readonly IPasswordHasher hasher = new SHA256PasswordHasher();

        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable(nameof(Account));
            builder.HasKey(x => x.Id);

            builder.Property(ac => ac.Created)
                .HasDefaultValueSql("getdate()");

            builder.HasData(
                new Account { Id = 1, Email = "2811542944@qq.com", Password = hasher.HashPassword("Hzxnnez160749"), Role = Account.ROLE_USER });
        }
    }
}
