using System.Linq;

namespace SP.Client.Linq
{
    public interface ISpRepository<TEntity>
        where TEntity : IListItemEntity
    {
        TEntity Find(int itemId);

        IQueryable<TEntity> FindAll(params int[] itemIds);

        /// <summary>
        /// Inserts a new entity into the repository.
        /// </summary>
        void Insert(TEntity entity);

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="entity">Entity to update.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Removes an entity from the respository.
        /// </summary>
        /// <returns>False if the entity does not exist in the repository, or true if successfully deleted.</returns>
        bool Delete(params int[] itemIds);
    }
}
