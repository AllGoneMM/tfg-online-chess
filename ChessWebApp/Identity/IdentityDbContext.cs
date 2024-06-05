using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChessWebApp.Identity
{
    public class ChessIdentityDbContext(DbContextOptions<ChessIdentityDbContext> options)
        : IdentityDbContext<ChessUser, ChessRole, string>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ChessUser>(entity =>
            {
                entity.ToTable("ChessUser");
            });
            builder.Entity<ChessRole>(entity =>
            {
                entity.ToTable("ChessRole");
            });
            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("ChessRoleClaims");
            });
            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("ChessUserClaims");
            });
            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("ChessUserLogins");
            });
            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("ChessUserTokens");
            });
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("ChessUserRoles");
            });

        }
    }

}
