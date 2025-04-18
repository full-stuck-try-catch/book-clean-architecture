namespace BookLibrary.Domain.Reviews;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Review?> GetByBookAndUserAsync(Guid bookId, Guid userId, CancellationToken cancellationToken = default);

    Task<List<Review>> GetByBookIdAsync(Guid bookId, CancellationToken cancellationToken = default);

    Task<List<Review>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    void Add(Review review);

    void Update(Review review);

    void Remove(Review review);
}
