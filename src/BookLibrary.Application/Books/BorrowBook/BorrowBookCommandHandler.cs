using BookLibrary.Application.Abstractions.Authentication;
using BookLibrary.Application.Abstractions.Clock;
using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;
using BookLibrary.Domain.Loans;
using BookLibrary.Domain.Users;

namespace BookLibrary.Application.Books.BorrowBook;

public sealed class BorrowBookCommandHandler : ICommandHandler<BorrowBookCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUserContext _userContext;
    private readonly ILoanRepository _loanRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public BorrowBookCommandHandler(
        IUserRepository userRepository,
        IBookRepository bookRepository,
        IUserContext userContext,
        ILoanRepository loanRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _bookRepository = bookRepository;
        _userContext = userContext;
        _loanRepository = loanRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(BorrowBookCommand request, CancellationToken cancellationToken)
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

        // Create loan period
        Result<LoanPeriod> loanPeriodResult = LoanPeriod.Create(request.StartDate, request.EndDate);
        if (loanPeriodResult.IsFailure)
        {
            return loanPeriodResult;
        }

        Result<Loan> loan = Loan.Create(
           Guid.NewGuid(),
           user.Id,
           book.Id,
           loanPeriodResult.Value);


        // Borrow the book using domain logic
        Result borrowResult = user.BorrowBook(book, loan.Value);

        if (borrowResult.IsFailure)
        {
            return borrowResult;
        }

        // Attach the loan to the book
        _loanRepository.Add(loan.Value);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
