using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TasksApi.Interfaces
{
    interface ICRUDService
    {
        bool Create(Object obj);
        bool Update(Object obj);
        bool Delete(int id);
        Object GetOne(int id);
        IEnumerable<Object> GetAll();
        Object Search(string key, string value);
    }
}
