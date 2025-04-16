using BookLibrary.Application.Abstractions.Authentication;
using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;
using BookLibrary.Domain.Loans;
using BookLibrary.Domain.Users;

namespace BookLibrary.Application.Loans.CreateLoan;

internal sealed class CreateLoanCommandHandler : ICommandHandler<CreateLoanCommand, Guid>
{
    private readonly ILoanRepository _loanRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLoanCommandHandler(
        ILoanRepository loanRepository,
        IUserRepository userRepository,
        IBookRepository bookRepository,
        IUserContext userContext,
        IUnitOfWork unitOfWork)
    {
        _loanRepository = loanRepository;
        _userRepository = userRepository;
        _bookRepository = bookRepository;
        _userContext = userContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateLoanCommand request, CancellationToken cancellationToken)
    {
        // Get current user ID from context
        Guid userId = _userContext.UserId;

        // Validate user exists
        User? user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<Guid>(UserErrors.NotFound);
        }

        // Validate book exists
        Book? book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null)
        {
            return Result.Failure<Guid>(BookErrors.NotFound);
        }

        // Check if book is available
        if (book.Status != BookStatus.Available)
        {
            return Result.Failure<Guid>(BookErrors.BookNotAvailable);
        }

        // Check if user already has an active loan for this book
        Loan? existingLoan = await _loanRepository.GetActiveLoanByUserAndBookAsync(
            userId, request.BookId, cancellationToken);
        if (existingLoan is not null)
        {
            return Result.Failure<Guid>(LoanErrors.LoanAlreadyExists);
        }

        // Create loan period
        var loanPeriod = new LoanPeriod(request.StartDate, request.EndDate);

        // Create loan
        Result<Loan> loanResult = Loan.Create(Guid.NewGuid(), userId, request.BookId, loanPeriod);

        if (loanResult.IsFailure)
        {
            return Result.Failure<Guid>(loanResult.Error);
        }

        // Mark book as borrowed
        book.MarkAsBorrowed();

        // Add to repository
        _loanRepository.Add(loanResult.Value);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(loanResult.Value.Id);
    }
}
