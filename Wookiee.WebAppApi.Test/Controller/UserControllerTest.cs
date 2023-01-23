using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Wookiee.Service.Interface;
using Wookiee.Service.Model.User;
using Wookiee.WebAppApi.Controllers;
using Wookiee.WebAppApi.Test.Model.User;

namespace Wookiee.WebAppApi.Test.Controller;

[TestFixture]
public class UserControllerTest
{
    private Mock<IUserService> _userServiceMock;
    private UserController _userController;

    [SetUp]
    public void Setup()
    {
        _userServiceMock = new Mock<IUserService>();
        
        _userController = new UserController(_userServiceMock.Object);
    }

    [Test]
    public async Task RegisterCheck_ModelStateInvalid_ReturnBadRequest()
    {
        _userController.ModelState.AddModelError("Error", "Something went wrong");

        var result = (BadRequestResult)await _userController.Register(Mapper.ToRegisterUser());
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(result.StatusCode, 400);
    }

    [Test]
    public async Task LoginCheck_ModelStateInvalid_ReturnBadRequest()
    {
        _userController.ModelState.AddModelError("Error", "Something went wrong");

        var result = (BadRequestResult)await _userController.Login(Mapper.ToLoginUser());
        Assert.That(result, Is.Not.Null);
        Assert.IsInstanceOf<BadRequestResult>(result);
        Assert.AreEqual(result.StatusCode, 400);
    }

    

    [Test]
    public async Task RegisterCheck_RegisterUser_Success()
    {
        // Arrange
        var mockUserService = new Mock<IUserService>();
        mockUserService
            .Setup(service => service.Register(It.IsAny<RegisterDto>()))
            .ReturnsAsync(Mapper.ToResponseObject);
        var userController = new UserController(mockUserService.Object);

        // Act

        var result = await userController.Register(Mapper.ToRegisterUser());
        

        // Assert
        Assert.IsInstanceOf<JsonResult>(result);
        mockUserService.Verify(service => service.Register(It.IsAny<RegisterDto>()), Times.Once);
    }


    [Test]
    public async Task LoginCheck_LoginUser_Success()
    {
        // Arrange
        var mockUserService = new Mock<IUserService>();
        mockUserService
            .Setup(service => service.Login(It.IsAny<LoginDto>()))
            .ReturnsAsync(Mapper.ToResponseObject);
        var userController = new UserController(mockUserService.Object);

        // Act

        var result = await userController.Login(Mapper.ToLoginUser());


        // Assert
        Assert.IsInstanceOf<JsonResult>(result);
        mockUserService.Verify(service => service.Login(It.IsAny<LoginDto>()), Times.Once);
        mockUserService.Verify(service => service.Login(It.IsAny<LoginDto>()), Times.AtLeast(1));
    }

}