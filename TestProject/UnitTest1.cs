using Moq;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Shop.Pages;
using Shop.Pages.Admin.Books;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class BookDetailsModelTests
{
    private readonly Mock<IBookRepository> _mockRepository;
    private readonly BookDetailsModel _bookDetailsModel;

    public BookDetailsModelTests()
    {
        _mockRepository = new Mock<IBookRepository>();
        _bookDetailsModel = new BookDetailsModel(_mockRepository.Object);
    }

    [Fact]
    public void OnGet_BookIdIsNull_ShouldRedirectToErrorPage()
    {
        // Arrange
        var mockRepository = new Mock<IBookRepository>();
        var pageModel = new BookDetailsModel(mockRepository.Object);

        // Act
        var result = pageModel.OnGet(null);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("Error", redirectResult.PageName);
    }

    [Fact]
    public void OnGet_BookIdIsInvalid_ShouldRedirectToErrorPage()
    {
        // Act
        var result = _bookDetailsModel.OnGet(null);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("Error", redirectResult.PageName);
    }

    [Fact]
    public void OnGet_BookNotFound_ShouldRedirectToHomePage()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetBookById(It.IsAny<int>())).Returns((BookInfo)null);

        // Act
        var result = _bookDetailsModel.OnGet(1);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/", redirectResult.PageName);
    }

    [Fact]
    public void OnGet_RepositoryThrowsException_ShouldRedirectToErrorPage()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetBookById(It.IsAny<int>())).Throws(new Exception("Database error"));

        // Act
        var result = _bookDetailsModel.OnGet(1);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("Error", redirectResult.PageName);
    }

    [Fact]
    public void OnGet_BookFound_ShouldReturnPage()
    {
        // Arrange
        var mockRepository = new Mock<IBookRepository>();
        var book = new BookInfo { Id = 1, Title = "Test Book" };
        mockRepository.Setup(repo => repo.GetBookById(1)).Returns(book);
        var pageModel = new BookDetailsModel(mockRepository.Object);

        // Act
        var result = pageModel.OnGet(1);

        // Assert
        var pageResult = Assert.IsType<PageResult>(result);
        Assert.Equal(book, pageModel.BookInfo);
    }

    [Fact]
    public void OnGet_BookIdIsNegative_ShouldRedirectToErrorPage()
    {
        // Act
        var result = _bookDetailsModel.OnGet(-1);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("Error", redirectResult.PageName);
    }

    [Fact]
    public void OnGet_BookIdIsZero_ShouldRedirectToErrorPage()
    {
        // Act
        var result = _bookDetailsModel.OnGet(0);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("Error", redirectResult.PageName);
    }

    [Fact]
    public void OnGet_BookFound_ShouldSetBookInfoCorrectly()
    {
        // Arrange
        var book = new BookInfo { Id = 10, Title = "Test Book", Author = "Test Author", Price = 9.99M, NumPages = 123, Category = "Test Category", Description = "Test Description", ImageFileName = "test.jpg" };
        _mockRepository.Setup(repo => repo.GetBookById(1)).Returns(book);

        // Act
        var result = _bookDetailsModel.OnGet(1);

        // Assert
        var pageResult = Assert.IsType<PageResult>(result);
        Assert.Equal(book, _bookDetailsModel.BookInfo);
        Assert.Equal("Test Book", _bookDetailsModel.BookInfo.Title);
        Assert.Equal("Test Author", _bookDetailsModel.BookInfo.Author);
        Assert.Equal(9.99M, _bookDetailsModel.BookInfo.Price);
        Assert.Equal(123, _bookDetailsModel.BookInfo.NumPages);
        Assert.Equal("Test Category", _bookDetailsModel.BookInfo.Category);
        Assert.Equal("Test Description", _bookDetailsModel.BookInfo.Description);
        Assert.Equal("test.jpg", _bookDetailsModel.BookInfo.ImageFileName);
    }

    [Fact]
    public void OnGet_BookIdIsPositive_ShouldReturnPage()
    {
        // Arrange
        var book = new BookInfo { Id = 1, Title = "Test Book" };
        _mockRepository.Setup(repo => repo.GetBookById(1)).Returns(book);

        // Act
        var result = _bookDetailsModel.OnGet(1);

        // Assert
        var pageResult = Assert.IsType<PageResult>(result);
        Assert.Equal(book, _bookDetailsModel.BookInfo);
    }

    [Fact]
    public void OnGet_BookNotFound_ShouldSetBookInfoToNull()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetBookById(It.IsAny<int>())).Returns((BookInfo)null);

        // Act
        var result = _bookDetailsModel.OnGet(1);

        // Assert
        Assert.Null(_bookDetailsModel.BookInfo);
    }

    [Fact]
    public void OnGet_BookFoundWithDifferentValues_ShouldSetBookInfoCorrectly()
    {
        // Arrange
        var book = new BookInfo { Id = 2, Title = "Another Test Book", Author = "Another Author", Price = 15.99M, NumPages = 456, Category = "Another Category", Description = "Another Description", ImageFileName = "another.jpg" };
        _mockRepository.Setup(repo => repo.GetBookById(2)).Returns(book);

        // Act
        var result = _bookDetailsModel.OnGet(2);

        // Assert
        var pageResult = Assert.IsType<PageResult>(result);
        Assert.Equal(book, _bookDetailsModel.BookInfo);
        Assert.Equal("Another Test Book", _bookDetailsModel.BookInfo.Title);
        Assert.Equal("Another Author", _bookDetailsModel.BookInfo.Author);
        Assert.Equal(15.99M, _bookDetailsModel.BookInfo.Price);
        Assert.Equal(456, _bookDetailsModel.BookInfo.NumPages);
        Assert.Equal("Another Category", _bookDetailsModel.BookInfo.Category);
        Assert.Equal("Another Description", _bookDetailsModel.BookInfo.Description);
        Assert.Equal("another.jpg", _bookDetailsModel.BookInfo.ImageFileName);
    }

    [Fact]
    public void OnGet_BookNotFound_ShouldNotSetBookInfoToDefaultValues()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetBookById(It.IsAny<int>())).Returns((BookInfo)null);

        // Act
        var result = _bookDetailsModel.OnGet(1);

        // Assert
        Assert.Null(_bookDetailsModel.BookInfo);
    }

    [Fact]
    public void OnGet_BookIdIsLargeNumber_ShouldRedirectToErrorPage()
    {
        // Act
        var result = _bookDetailsModel.OnGet(int.MaxValue);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("Error", redirectResult.PageName);
    }

    [Fact]
    public void OnGet_DatabaseReturnsMultipleResults_ShouldRedirectToErrorPage()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetBookById(1)).Throws(new InvalidOperationException("Multiple results"));

        // Act
        var result = _bookDetailsModel.OnGet(1);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("Error", redirectResult.PageName);
    }

    [Fact]
    public void AddToCart_BookIdIsValid_ShouldReturnSuccessMessage()
    {
        // Arrange
        var bookId = 1;

        // Act
        var result = _bookDetailsModel.AddToCart(bookId);

        // Assert
        var contentResult = Assert.IsType<ContentResult>(result);
        Assert.Equal($"Book with ID {bookId} added to cart successfully.", contentResult.Content);
    }

    [Fact]
    public void AddToCart_BookIdIsZero_ShouldReturnErrorMessage()
    {
        // Arrange
        var bookId = 0;

        // Act
        var result = _bookDetailsModel.AddToCart(bookId);

        // Assert
        var contentResult = Assert.IsType<ContentResult>(result);
        Assert.Equal("Invalid book ID.", contentResult.Content);
    }

    [Fact]
    public void AddToCart_BookIdIsNegative_ShouldReturnErrorMessage()
    {
        // Arrange
        var bookId = -1;

        // Act
        var result = _bookDetailsModel.AddToCart(bookId);

        // Assert
        var contentResult = Assert.IsType<ContentResult>(result);
        Assert.Equal("Invalid book ID.", contentResult.Content);
    }

    [Fact]
    public void AddToCart_BookIdIsNull_ShouldReturnErrorMessage()
    {
        // Act
        var result = _bookDetailsModel.AddToCart(null);

        // Assert
        var contentResult = Assert.IsType<ContentResult>(result);
        Assert.Equal("Invalid book ID.", contentResult.Content);
    }

    [Fact]
    public void AddToCart_BookIdIsMaxValue_ShouldReturnErrorMessage()
    {
        // Arrange
        var bookId = int.MaxValue;

        // Act
        var result = _bookDetailsModel.AddToCart(bookId);

        // Assert
        var contentResult = Assert.IsType<ContentResult>(result);
        Assert.Equal("Invalid book ID.", contentResult.Content);
    }

    [Fact]
    public void AddToCart_BookIdIsMinValue_ShouldReturnErrorMessage()
    {
        // Arrange
        var bookId = int.MinValue;

        // Act
        var result = _bookDetailsModel.AddToCart(bookId);

        // Assert
        var contentResult = Assert.IsType<ContentResult>(result);
        Assert.Equal("Invalid book ID.", contentResult.Content);
    }

}
