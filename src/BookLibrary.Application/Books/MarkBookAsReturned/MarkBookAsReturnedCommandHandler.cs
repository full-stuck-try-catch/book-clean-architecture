using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;

namespace BookLibrary.Application.Books.MarkBookAsReturned;
public sealed class MarkBookAsReturnedCommandHandler : ICommandHandler<MarkBookAsReturnedCommand>
{
    private readonly IBookRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkBookAsReturnedCommandHandler(
        IBookRepository bookRepository,
        IUnitOfWork unitOfWork)
    {
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(MarkBookAsReturnedCommand request, CancellationToken cancellationToken)
    {
        // Find book
        Book? book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null)
        {
            return Result.Failure(BookErrors.NotFound);
        }

        // Mark as returned using domain logic
        Result result = book.MarkAsReturned();
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
