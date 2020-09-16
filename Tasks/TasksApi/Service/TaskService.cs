using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TasksApi.Interfaces;

namespace TasksApi.Service
{
    public class TaskService : ICRUDService
    {
        public bool Create(object obj)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetAll()
        {
            throw new NotImplementedException();
        }

        public object GetOne(int id)
        {
            throw new NotImplementedException();
        }

        public object Search(string key, string value)
        {
            throw new NotImplementedException();
        }

        public bool Update(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
