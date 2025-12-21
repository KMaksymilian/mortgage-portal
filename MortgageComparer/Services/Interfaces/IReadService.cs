using System.ComponentModel.DataAnnotations;

namespace MortgageComparer.Services.Interfaces {
    public interface IReadService<T, TKey> where T : BasicEntity {
        List<T> GetAll();
        T GetById(TKey id);
    }

    //To be removed
    public class BasicEntity { }
   
}
