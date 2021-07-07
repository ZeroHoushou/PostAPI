using SocialMedia.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia.Core.Interfaces
{
    public interface IRepository<T> where T: BaseEntity
    {
        IEnumerable<T> GetAll();
        Task<T> GetByid(int id);
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(int id);
       
    }
}
