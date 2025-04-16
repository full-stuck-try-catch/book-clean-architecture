using BookLibrary.Application.Abstractions.Authentication;
using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Loans;

namespace BookLibrary.Application.Loans.ExtendLoan;

internal sealed class ExtendLoanCommandHandler : ICommandHandler<ExtendLoanCommand>
{
    private readonly ILoanRepository _loanRepository;
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;

    public ExtendLoanCommandHandler(
        ILoanRepository loanRepository,
        IUserContext userContext,
        IUnitOfWork unitOfWork)
    {
        _loanRepository = loanRepository;
        _userContext = userContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ExtendLoanCommand request, CancellationToken cancellationToken)
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

        // Create new loan period for extension

        Result<LoanPeriod> extensionPeriod = LoanPeriod.Create(loan.Period.StartDate , request.NewEndDate);

        // Extend loan
        Result extendResult = loan.Extend(extensionPeriod.Value);

        if (extendResult.IsFailure)
        {
            return extendResult;
        }

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
