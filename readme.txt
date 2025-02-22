This is a Book Management API built with ASP.NET Core (.NET 8) and MongoDB. It allows users to add, update, delete, and retrieve books.

#Features:

1) Add Books (Single & Bulk)
2) Update Books
3) Soft Delete Books (Single & Bulk)
4) Retrieve Book Titles with Views Count (Sorted by popularity)
5) Retrieve Full Book Details (Increases view count)



#API Endpoints

1) Retrieve Book Titles with Views Count

GET /api/books/titlesReturns a list of book titles along with their views count, sorted by popularity.

2) Retrieve Full Book Details

GET /api/books/{id}Fetches full details of a book and increases its views count.

3) Add a Book

POST /api/booksAdds a book to the collection. Ensures duplicate titles are not allowed.

4) Add Multiple Books

POST /api/books/bulkAllows bulk book additions while skipping duplicates.

5) Update a Book

PUT /api/books/{id}Updates an existing book's details.

6) Soft Delete a Book

DELETE /api/books/{id}Marks a book as deleted without removing it from the database.

7) Soft Delete Multiple Books

DELETE /api/books/bulkMarks multiple books as deleted.



#Could not finish:

1) JWT-based authentication
2) When retrieving book details, calculate its popularity score on the fly
