using Exadel_task.Models;
using Exadel_task.Services;
using Microsoft.AspNetCore.Mvc;

namespace Exadel_task.Controllers
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

        // GET: api/books
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var books = await _bookService.GetAllAsync();
            return Ok(books);
        }

        // GET: api/books/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var book = await _bookService.GetByIdAsync(id);
            return book == null ? NotFound() : Ok(book);
        }

        // POST: api/books
        [HttpPost]
        public async Task<IActionResult> Create(Book book)
        {
            await _bookService.CreateAsync(book);
            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }

        // PUT: api/books/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Book updatedBook)
        {
            var existingBook = await _bookService.GetByIdAsync(id);
            if (existingBook == null)
                return NotFound();

            updatedBook.Id = id; // Ensure the correct ID is used
            await _bookService.UpdateAsync(id, updatedBook);
            return NoContent();
        }

        // DELETE: api/books/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null)
                return NotFound();

            await _bookService.DeleteAsync(id);
            return NoContent();
        }
    }
}