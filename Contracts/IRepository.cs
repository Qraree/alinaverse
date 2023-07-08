using System.Linq.Expressions;

namespace Contracts;

public interface IRepository<T>
{
    IQueryable<T> FindAll(Expression<Func<T, bool>> expression);
    T FindById(int id);
    void Create(T entity);
    void Update(T entity);
    void Delete(T entity);
}