using System;
using System.Collections.Generic;
using System.Collections;

namespace HTTPResponse.Repository
{
    public interface IRepository <TEntity>
    {
        TEntity FindById(int id);
        IEnumerable FindAll();
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
