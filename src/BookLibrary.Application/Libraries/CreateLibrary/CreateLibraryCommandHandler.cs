using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Libraries;

namespace BookLibrary.Application.Libraries.CreateLibrary;

public sealed class CreateLibraryCommandHandler : ICommandHandler<CreateLibraryCommand, Guid>
{
    private readonly ILibraryRepository _libraryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLibraryCommandHandler(
        ILibraryRepository libraryRepository,
        IUnitOfWork unitOfWork)
    {
        _libraryRepository = libraryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateLibraryCommand request, CancellationToken cancellationToken)
    {
        // Validate library name
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result.Failure<Guid>(LibraryErrors.InvalidName);
        }

        // Check if library name already exists
        bool libraryExists = await _libraryRepository.ExistsAsync(request.Name, cancellationToken);
        if (libraryExists)
        {
            return Result.Failure<Guid>(LibraryErrors.NameAlreadyExists);
        }

        // Create library
        var libraryName = new LibraryName(request.Name);
        var library = Library.Create(Guid.NewGuid(), libraryName);

        // Add to repository
        _libraryRepository.Add(library);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(library.Id);
    }
}
