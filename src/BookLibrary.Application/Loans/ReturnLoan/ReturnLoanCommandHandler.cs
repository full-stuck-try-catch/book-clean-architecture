using BookLibrary.Application.Abstractions.Authentication;
using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;
using BookLibrary.Domain.Loans;

namespace BookLibrary.Application.Loans.ReturnLoan;

internal sealed class ReturnLoanCommandHandler : ICommandHandler<ReturnLoanCommand>
{
    private readonly ILoanRepository _loanRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;

    public ReturnLoanCommandHandler(
        ILoanRepository loanRepository,
        IBookRepository bookRepository,
        IUserContext userContext,
        IUnitOfWork unitOfWork)
    {
        _loanRepository = loanRepository;
        _bookRepository = bookRepository;
        _userContext = userContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ReturnLoanCommand request, CancellationToken cancellationToken)
    {
        // Get current user ID from context
        Guid userId = _userContext.UserId;

        // Get loan
        Loan? loan = await _loanRepository.GetByIdAsync(request.LoanId, cancellationToken);
        if (loan is null)
        {
            return Result.Failure(LoanErrors.NotFound);
        }

        // Ensure the loan belongs to the current user
        if (loan.UserId != userId)
        {
            return Result.Failure(LoanErrors.NotFound); // Return NotFound for security reasons
        }

        // Mark loan as returned
        Result returnResult = loan.MarkAsReturned(request.ReturnedAt);
        if (returnResult.IsFailure)
        {
            return returnResult;
        }

        // Get book and mark as available
        Book? book = await _bookRepository.GetByIdAsync(loan.BookId, cancellationToken);

        if (book is  null)
        {
           return Result.Failure(BookErrors.NotFound);
        }

        book.MarkAsReturned();

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
