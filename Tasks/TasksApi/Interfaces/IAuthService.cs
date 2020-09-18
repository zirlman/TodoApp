using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TasksApi.Model.Auth;

namespace TasksApi.Interfaces
{
    public interface IAuthService: ICRUDService
    {
        object Authenticate(AuthRequest req);
    }
}
