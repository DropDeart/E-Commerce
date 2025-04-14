using Ljuss.Core.Domian;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LjussAPI.Persistence.ModelBuilderConfigurations
{
    public class ModelBuilderConfig
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            #region VerificationCode and User Relations

                modelBuilder.Entity<VerificationCode>()
                .HasOne(vc => vc.User)
                .WithMany(u => u.VerificationCodes)
                .HasForeignKey(vc => vc.UserId);

            #endregion
        }
    }
}
