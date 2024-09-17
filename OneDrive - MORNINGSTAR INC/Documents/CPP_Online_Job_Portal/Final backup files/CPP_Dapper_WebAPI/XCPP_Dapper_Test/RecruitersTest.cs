using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPP_Dapper_WebAPI.Controllers;
using CPP_Dapper_WebAPI.Models;
using CPP_Dapper_WebAPI.Recruiters_Repository;
using JobPortalWebAPI.LoginRequest;
using JobPortalWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace XCPP_Dapper_Test
{
    public class RecruitersTest
    {
        private readonly Mock<IRecruiters> _mockRecruitersRepo;
        private readonly Mock<ILoginRequest> _mockLoginRequest;
        private readonly Mock<ILogger<RecruitersController>> _mockLogger;
        private readonly RecruitersController _controller;

        public RecruitersTest()
        {
            _mockRecruitersRepo = new Mock<IRecruiters>();
            _mockLoginRequest = new Mock<ILoginRequest>();
            _mockLogger = new Mock<ILogger<RecruitersController>>();

            _controller = new RecruitersController(_mockRecruitersRepo.Object, _mockLoginRequest.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetRecruitersByEmail_ReturnsOk_WhenRecruiterExists()
        {
            // Arrange
            var recruiterEmail = "contact@techwizards.com";
            var recruiter = new Recruiters
            {
                email = recruiterEmail,
                company_name = "TechWizards Pvt Ltd"
            };

            _mockRecruitersRepo.Setup(repo => repo.GetRecruitersAsync(recruiterEmail)).ReturnsAsync(recruiter);

            // Act
            var result = await _controller.GetRecruitersByEmail(recruiterEmail);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRecruiter = Assert.IsType<Recruiters>(okResult.Value);
            Assert.Equal(recruiterEmail, returnedRecruiter.email);
        }

        //[Fact]
        //public async Task GetRecruitersByEmail_ReturnsNotFound_WhenRecruiterDoesNotExist()
        //{
        //    // Arrange
        //    var recruiterEmail = "nonexistent@company.com";

        //    _mockRecruitersRepo.Setup(repo => repo.GetRecruitersAsync(recruiterEmail))
        //        .ReturnsAsync((Recruiters)null); // Return null to simulate non-existent recruiter

        //    // Act
        //    var result = await _controller.GetRecruitersByEmail(recruiterEmail);

        //    // Assert
        //    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result); // Check if result is NotFound
        //    var response = Assert.IsType<dynamic>(notFoundResult.Value); // Explicitly assert the type of response object
        //    Assert.Equal($"Recruiter with ID {recruiterEmail} not found.", response.message); // Assert on the returned message
        //}

        [Fact]
        public async Task AddRecruiters_ReturnsOk_WhenRecruiterIsInserted()
        {
            // Arrange
            var recruiter = new Recruiters
            {
                company_name = "TechWizards Pvt Ltd",
                email = "contact@techwizards.com"
            };

            _mockRecruitersRepo.Setup(repo => repo.InsertRecruitersAsync(recruiter)).ReturnsAsync(true);

            // Act
            var result = await _controller.AddRecruiters(recruiter);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Recruiters inserted successfully.", okResult.Value);
        }

        [Fact]
        public async Task AddRecruiters_ReturnsBadRequest_WhenInsertFails()
        {
            // Arrange
            var recruiter = new Recruiters
            {
                company_name = "TechWizards Pvt Ltd",
                email = "contact@techwizards.com"
            };

            _mockRecruitersRepo.Setup(repo => repo.InsertRecruitersAsync(recruiter)).ReturnsAsync(false);

            // Act
            var result = await _controller.AddRecruiters(recruiter);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to insert Recruiters.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateRecruiters_ReturnsOk_WhenRecruiterIsUpdated()
        {
            // Arrange
            var recruiterEmail = "contact@techwizards.com";
            var recruiter = new Recruiters
            {
                company_name = "TechWizards Pvt Ltd",
                email = recruiterEmail
            };

            _mockRecruitersRepo.Setup(repo => repo.UpdateRecruitersAsync(recruiterEmail, recruiter)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateRecruiters(recruiterEmail, recruiter);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Recruiters updated successfully.", okResult.Value);
        }

        [Fact]
        public async Task UpdateRecruiters_ReturnsBadRequest_WhenUpdateFails()
        {
            // Arrange
            var recruiterEmail = "contact@techwizards.com";
            var recruiter = new Recruiters
            {
                company_name = "TechWizards Pvt Ltd",
                email = recruiterEmail
            };

            _mockRecruitersRepo.Setup(repo => repo.UpdateRecruitersAsync(recruiterEmail, recruiter)).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateRecruiters(recruiterEmail, recruiter);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to update Recruiters.", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteRecruiters_ReturnsOk_WhenRecruiterIsDeleted()
        {
            // Arrange
            var recruiterEmail = "contact@techwizards.com";

            _mockRecruitersRepo.Setup(repo => repo.DeleteRecruitersAsync(recruiterEmail)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteRecruiters(recruiterEmail);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Recruiter Deleted successfully.", okResult.Value);
        }

        [Fact]
        public async Task DeleteRecruiters_ReturnsBadRequest_WhenDeleteFails()
        {
            // Arrange
            var recruiterEmail = "contact@techwizards.com";

            _mockRecruitersRepo.Setup(repo => repo.DeleteRecruitersAsync(recruiterEmail)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteRecruiters(recruiterEmail);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to update Recruiters", badRequestResult.Value);
        }

        //[Fact]
        //public async Task Login_ReturnsOk_WhenLoginSuccessful()
        //{
        //    // Arrange
        //    var loginRequest = new LoginReq
        //    {
        //        Email = "contact@techwizards.com",
        //        Password = "Tech@123"
        //    };

        //    var recruiter = new Recruiters
        //    {
        //        email = "contact@techwizards.com",
        //        password = "Tech@123"
        //    };

        //    _mockLoginRequest.Setup(repo => repo.GetRecruitersAsync(loginRequest.Email)).ReturnsAsync(recruiter);

        //    // Act
        //    var result = await _controller.Login(loginRequest);

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result);
        //    var resultMessage = ((dynamic)okResult.Value).message;
        //    Assert.Equal("Login successful", resultMessage);
        //}

        [Fact]
        public async Task Login_ReturnsNotFound_WhenRecruiterNotFound()
        {
            // Arrange
            var loginRequest = new LoginReq
            {
                Email = "nonexistent@company.com",
                Password = "wrongpassword"
            };

            _mockLoginRequest.Setup(repo => repo.GetRecruitersAsync(loginRequest.Email)).ReturnsAsync((Recruiters)null);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Recruiters not found with the provided email", notFoundResult.Value);
        }
    }
}
