using System.Collections.Generic;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Abstract
{
    public interface IProductsRepository
    {
        // interface uses IEnumerable<T> to allow a caller to obtain a 
        // sequence of Product objects, without saying how or where the
        // data is stored or retrieved
        IEnumerable<Product> Products { get; }
    }
}
