using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Application.Users.GetUserLogged;
public sealed record UserResponse(Guid Id , string Email , string FirstName , string LastName);
