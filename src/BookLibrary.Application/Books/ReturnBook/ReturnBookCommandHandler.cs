using BookLibrary.Application.Abstractions.Authentication;
using BookLibrary.Application.Abstractions.Clock;
using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;
using BookLibrary.Domain.Users;

namespace BookLibrary.Application.Books.ReturnBook;

internal sealed class ReturnBookCommandHandler : ICommandHandler<ReturnBookCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUserContext _userContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public ReturnBookCommandHandler(
        IUserRepository userRepository,
        IBookRepository bookRepository,
        IUserContext userContext,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _bookRepository = bookRepository;
        _userContext = userContext;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
    {
        // Get the current user
        User? user = await _userRepository.GetByIdAsync(_userContext.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }

        // Get the book
        Book? book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null)
        {
            return Result.Failure(BookErrors.NotFound);
        }

        // Return the book using domain logic
        Result returnResult = user.ReturnBook(book, _dateTimeProvider.UtcNow);
        if (returnResult.IsFailure)
        {
            return returnResult;
        }

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
