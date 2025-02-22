using Exadel_task.Models;
using Exadel_task.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookManagementAPI.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly BookService _bookService;

        public BooksController(BookService bookService)
        {
            _bookService = bookService;
        }

        //  Get book titles with views count, sorted by popularity
        [HttpGet("titles")]
        public async Task<IActionResult> GetTitlesSortedByPopularity()
        {
            var books = await _bookService.GetTitlesSortedByPopularityAsync();
            return Ok(books);
        }

        //  Get full details of a book by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var book = await _bookService.GetByIdAsync(id);
            return book == null ? NotFound("Book not found.") : Ok(book);
        }

        //  Add a single book
        [HttpPost]
        public async Task<IActionResult> Create(Book book)
        {
            var (success, message) = await _bookService.CreateAsync(book);
            return success ? CreatedAtAction(nameof(GetById), new { id = book.Id }, book) : BadRequest(new { Error = message });
        }

        //  Add multiple books
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkCreate(List<Book> books)
        {
            var (addedBooks, errors) = await _bookService.BulkCreateAsync(books);
            return Ok(new { AddedBooks = addedBooks, Errors = errors });
        }

        //  Update an existing book
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Book updatedBook)
        {
            var success = await _bookService.UpdateAsync(id, updatedBook);
            return success ? NoContent() : NotFound("Book not found or validation failed.");
        }

        //  Soft delete a single book
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(string id)
        {
            var success = await _bookService.SoftDeleteAsync(id);
            return success ? NoContent() : NotFound("Book not found.");
        }

        //  Soft delete multiple books
        [HttpDelete("bulk")]
        public async Task<IActionResult> BulkSoftDelete(List<string> ids)
        {
            var deletedCount = await _bookService.BulkSoftDeleteAsync(ids);
            return Ok(new { DeletedCount = deletedCount });
        }
    }
}
