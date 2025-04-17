using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Domain.Users;
public sealed class RolePermission
{
    public int RoleId { get; set; }

    public int PermissionId { get; set; }
}
