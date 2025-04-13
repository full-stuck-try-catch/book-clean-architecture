using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Books;

public enum BookStatus
{
    Available = 1,
    Borrowed = 2,
    Deleted = 3,
}