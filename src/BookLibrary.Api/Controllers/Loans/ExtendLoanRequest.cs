namespace BookLibrary.Api.Controllers.Loans;

public sealed record ExtendLoanRequest(
    DateTime NewEndDate);
