using BookLibrary.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLibrary.Infrastructure.Configurations;
public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions");
        builder.HasKey(r => new { r.RoleId, r.PermissionId } );

        builder.HasData(
            new RolePermission
            {
                RoleId = Role.User.Id,
                PermissionId = Permission.UserRead.Id
            },
            new RolePermission
            {
                RoleId = Role.Admin.Id,
                PermissionId = Permission.Admin.Id
            });
    }
}
