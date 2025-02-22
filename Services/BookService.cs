using Exadel_task.Models;
using Exadel_task.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Exadel_task.Services
{
    public class BookService
    {
        private readonly IMongoCollection<Book> _booksCollection;

        public BookService(IOptions<MongoDBSettings> mongoSettings)
        {
            var mongoClient = new MongoClient(mongoSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoSettings.Value.DatabaseName);
            _booksCollection = mongoDatabase.GetCollection<Book>(mongoSettings.Value.CollectionName);
        }

        //  Retrieve book details by ID (also increments views count)
        public async Task<Book?> GetByIdAsync(string id)
        {
            var book = await _booksCollection.Find(b => b.Id == id && !b.IsDeleted).FirstOrDefaultAsync();

            if (book != null)
            {
                // Increase view count each time a book is accessed
                book.Views += 1;
                await _booksCollection.ReplaceOneAsync(b => b.Id == id, book);
            }

            return book;
        }

        //  Retrieve all book titles with views count, sorted by popularity
        public async Task<List<Dictionary<string, object>>> GetTitlesSortedByPopularityAsync()
        {
            var books = await _booksCollection
                .Find(b => !b.IsDeleted)
                .SortByDescending(b => b.Views)
                .Project(b => new { b.Title, b.Views })
                .ToListAsync();

            return books.Select(book => new Dictionary<string, object>
            {
                { "title", book.Title },
                { "views", book.Views }
            }).ToList();
        }

        //  Validate Book Data (Checks Title, Author, Year, Views)
        private bool IsValidBook(Book book, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(book.Title))
            {
                errorMessage = "Title is required.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(book.Author))
            {
                errorMessage = "Author is required.";
                return false;
            }
            if (book.PublicationYear < 1900 || book.PublicationYear > 2026)
            {
                errorMessage = "Publication year must be between 1900 and 2026.";
                return false;
            }
            if (book.Views < 0)
            {
                errorMessage = "Views count cannot be negative.";
                return false;
            }

            errorMessage = "";
            return true;
        }

        //  Add a single book with validation
        public async Task<(bool success, string message)> CreateAsync(Book book)
        {
            if (!IsValidBook(book, out string validationError))
                return (false, validationError);

            var existingBook = await _booksCollection.Find(b => b.Title == book.Title && !b.IsDeleted).FirstOrDefaultAsync();
            if (existingBook != null)
                return (false, "A book with the same title already exists.");

            await _booksCollection.InsertOneAsync(book);
            return (true, "Book added successfully.");
        }

        //  Bulk add books with validation
        public async Task<(List<string> addedBooks, List<string> errors)> BulkCreateAsync(List<Book> books)
        {
            var addedBooks = new List<string>();
            var errors = new List<string>();

            foreach (var book in books)
            {
                if (!IsValidBook(book, out string validationError))
                {
                    errors.Add($"Error in '{book.Title}': {validationError}");
                    continue;
                }

                var existingBook = await _booksCollection.Find(b => b.Title == book.Title && !b.IsDeleted).FirstOrDefaultAsync();
                if (existingBook != null)
                {
                    errors.Add($"'{book.Title}' already exists.");
                    continue;
                }

                addedBooks.Add(book.Title);
                await _booksCollection.InsertOneAsync(book);
            }

            return (addedBooks, errors);
        }

        //  Update an existing book by ID
        public async Task<bool> UpdateAsync(string id, Book updatedBook)
        {
            var existingBook = await _booksCollection.Find(b => b.Id == id && !b.IsDeleted).FirstOrDefaultAsync();
            if (existingBook == null)
                return false;

            if (!IsValidBook(updatedBook, out _))
                return false;

            updatedBook.Id = id;
            await _booksCollection.ReplaceOneAsync(b => b.Id == id, updatedBook);
            return true;
        }

        //  Soft delete a single book
        public async Task<bool> SoftDeleteAsync(string id)
        {
            var update = Builders<Book>.Update.Set(b => b.IsDeleted, true);
            var result = await _booksCollection.UpdateOneAsync(b => b.Id == id, update);
            return result.ModifiedCount > 0;
        }

        //  Soft delete multiple books at once
        public async Task<long> BulkSoftDeleteAsync(List<string> ids)
        {
            var update = Builders<Book>.Update.Set(b => b.IsDeleted, true);
            var result = await _booksCollection.UpdateManyAsync(b => ids.Contains(b.Id), update);
            return result.ModifiedCount;
        }
    }
}
