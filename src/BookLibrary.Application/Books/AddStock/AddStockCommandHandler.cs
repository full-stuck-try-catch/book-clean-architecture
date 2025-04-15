using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;

namespace BookLibrary.Application.Books.AddStock;

internal sealed class AddStockCommandHandler : ICommandHandler<AddStockCommand>
{
    private readonly IBookRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddStockCommandHandler(
        IBookRepository bookRepository,
        IUnitOfWork unitOfWork)
    {
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AddStockCommand request, CancellationToken cancellationToken)
    {
        // Find book
        Book? book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null)
        {
            return Result.Failure(BookErrors.NotFound);
        }

        // Add stock using domain logic
        Result result = book.AddStock(request.Count);
        if (result.IsFailure)
        {
            return result;
        }

        // Update repository
        _bookRepository.Update(book);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
