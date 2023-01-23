using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Wookiee.Service.Interface;
using Wookiee.Service.Model.Book;
using Wookiee.Utility;
using Wookiee.WebAppApi.Controllers;
using Wookiee.WebAppApi.Test.Model.Book;

namespace Wookiee.WebAppApi.Test.Controller;

[TestFixture]
public class BookControllerTest
{
    private Mock<IBookService> _bookServiceMock;
    private Mock<IHelper> _helperMock;
    private BookController _bookController;

    [SetUp]
    public void Setup()
    {
        _bookServiceMock = new Mock<IBookService>();
        _helperMock = new Mock<IHelper>();
        _bookController = new BookController(_bookServiceMock.Object, _helperMock.Object);
    }

    [Test]
    public async Task CheckCreate_InvalidInput_ReturnBadRequest()
    {
        _bookController.ModelState.AddModelError("Error", "Something wrong");

        var result = (BadRequestResult) await _bookController.Create(Mapper.ToCreate());
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(result.StatusCode, 400);
    }

    [Test]
    public async Task CheckUpdate_InvalidInput_ReturnBadRequest()
    {
        _bookController.ModelState.AddModelError("Error", "Something wrong");

        var result = (BadRequestResult)await _bookController.Update(Mapper.ToUpdate());
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(result.StatusCode, 400);
    }

    [Test]
    public async Task CheckDelete_InvalidInput_ReturnBadRequest()
    {
        // Arrange
        _bookController.ModelState.AddModelError("Error", "Something wrong");

        //Act
        var result = (BadRequestResult)await _bookController.Delete(1);

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(result.StatusCode, 400);
        Assert.ThrowsAsync<FormatException>(async () => await _bookController.Delete(Int32.Parse("Test")));
    }

    [Test]
    public async Task CheckCreate_ValidInput_ReturnCorrectResult()
    {
        // Arrange
        _bookServiceMock.Setup(service => service.Create(It.IsAny<AddBookDto>()))
            .ReturnsAsync(Mapper.ToResponseObject);
        _helperMock.Setup(method => method.ImageValidation(null)).Returns((true, null));

        // Act
        var result = await _bookController.Create(Mapper.ToCreate());

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.IsInstanceOf<JsonResult>(result);
        _bookServiceMock.Verify(service => service.Create(It.IsAny<AddBookDto>()), Times.Once);
    }

    [Test]
    public async Task CheckUpdate_ValidInput_ReturnCorrectResult()
    {
        // Arrange
        _bookServiceMock.Setup(service => service.Update(It.IsAny<UpdateBookDto>()))
            .ReturnsAsync(Mapper.ToResponseObject);
        _helperMock.Setup(method => method.ImageValidation(null)).Returns((true, null));

        // Act
        var result = await _bookController.Update(Mapper.ToUpdate());

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.IsInstanceOf<JsonResult>(result);
        _bookServiceMock.Verify(service => service.Update(It.IsAny<UpdateBookDto>()), Times.Once);
    }

    [Test]
    public async Task CheckGetList_WithoutInput_ReturnCorrectResult()
    {
        // Arrange
        _bookServiceMock.Setup(service => service.ReadList())!
            .ReturnsAsync(Mapper.ToListResponseObject);

        // Act
        var result = await _bookController.GetList();

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.IsInstanceOf<JsonResult>(result);
        _bookServiceMock.Verify(service => service.ReadList(),Times.Once);
    }

    [Test]
    public async Task CheckGetBook_WithValidInput_ReturnCorrectResult()
    {
        // Arrange
        _bookServiceMock.Setup(service => service.Read(1))
            .ReturnsAsync(Mapper.ToResponseObject);

        // Act
        var result = await _bookController.GetDetail(1);

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.IsInstanceOf<JsonResult>(result);
        _bookServiceMock.Verify(service => service.Read(1), Times.Once);
    }

    [Test]
    public void CheckGetBook_WithInvalidInput_ReturnCorrectResult()
    {
        // Assert
        Assert.ThrowsAsync<FormatException>(async () => await _bookController.GetDetail(Int32.Parse("Test")));
    }

    [Test]
    [TestCase("")]
    [TestCase(" ")]
    [TestCase(null)]
    public async Task CheckSearchAuthor_EmptyInput_ReturnBadRequest(string authorName, int expectedResult = 400)
    {
        // Act
        var result = (BadRequestObjectResult)await _bookController.SearchByAuthor(authorName);

        // Arrange
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(result.StatusCode, expectedResult);
        Assert.AreEqual(result.Value, "Author is empty");
        Assert.That(result.Value, Does.Contain("author").IgnoreCase);
    }

    [Test]
    [TestCase("")]
    [TestCase(" ")]
    [TestCase(null)]
    public async Task CheckSearchTitle_EmptyInput_ReturnBadRequest(string title, int expectedResult = 400)
    {
        var result = (BadRequestObjectResult)await _bookController.SearchByTitle(title);
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(result.StatusCode, expectedResult);
        Assert.AreEqual(result.Value, "Title is empty");
        Assert.That(result.Value, Does.Contain("title").IgnoreCase);
    }

    [Test]
    [TestCase("")]
    [TestCase(" ")]
    [TestCase(null)]
    public async Task CheckSearchDescription_EmptyInput_ReturnBadRequest(string description, int expectedResult = 400)
    {
        // Act
        var result = (BadRequestObjectResult)await _bookController.SearchByDescription(description);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(result.StatusCode, expectedResult);
        Assert.AreEqual(result.Value, "Description is empty");
        Assert.That(result.Value, Does.Contain("description").IgnoreCase);
    }

    [Test]
    public async Task CheckSearchBookByDescription_WithValidInput_ReturnCorrectResult()
    {
        // Arrange
        _bookServiceMock.Setup(service => service.SearchDescription(It.IsAny<string>()))!
            .ReturnsAsync(Mapper.ToListResponseObject());

        // Act
        var result = await _bookController.SearchByDescription("Test");

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.IsInstanceOf<JsonResult>(result);
        _bookServiceMock.Verify(service => service.SearchDescription(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task CheckSearchBookByAuthor_WithValidInput_ReturnCorrectResult()
    {
        // Arrange
        _bookServiceMock.Setup(service => service.SearchAuthor(It.IsAny<string>()))!
            .ReturnsAsync(Mapper.ToListResponseObject());

        // Act
        var result = await _bookController.SearchByAuthor("Test");

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.IsInstanceOf<JsonResult>(result);
        _bookServiceMock.Verify(service => service.SearchAuthor(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task CheckSearchBookByTitle_WithValidInput_ReturnCorrectResult()
    {
        // Arrange
        _bookServiceMock.Setup(service => service.SearchTitle(It.IsAny<string>()))!
            .ReturnsAsync(Mapper.ToListResponseObject());

        // Act
        var result = await _bookController.SearchByTitle("Test");

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.IsInstanceOf<JsonResult>(result);
        _bookServiceMock.Verify(service => service.SearchTitle(It.IsAny<string>()), Times.Once);

    }

    [Test]
    public async Task CheckUploadFile_InvalidExtension_ReturnBadRequest()
    {
        _helperMock.Setup(x => x.ImageValidation(It.IsAny<IFormFile>()))
            .Returns((false, "Invalid image extension"));

        var result = (BadRequestObjectResult) await _bookController.Create(Mapper.ToInvalidUploadFile());
        Assert.AreEqual(400, result.StatusCode);
        _bookServiceMock.Verify(method => method.Create(It.IsAny<AddBookDto>()), Times.Never);
    }

    [Test]
    public async Task CheckUploadFile_UpLimitOfFile_ReturnBadRequest()
    {
        _helperMock.Setup(x => x.ImageValidation(Mapper.InvalidImageLimit()))
            .Returns((false, "File is too big, limit is 200kb"));

        var result = (BadRequestObjectResult)await _bookController.Create(Mapper.ToInvalidFileLimit());
        Assert.AreEqual(400, result.StatusCode);
        _bookServiceMock.Verify(method => method.Create(It.IsAny<AddBookDto>()), Times.Never);
    }
}