using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPP_Dapper_WebAPI.Controllers;
using CPP_Dapper_WebAPI.JobPosting_Repository;
using CPP_Dapper_WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace XCPP_Dapper_Test
{
    public class JobPostingsTest
    {
        private readonly Mock<IJobPostings> _mockJobPostingsRepository;
        private readonly Mock<ILogger<JobPostingsController>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly JobPostingsController _controller;

        public JobPostingsTest()
        {
            _mockJobPostingsRepository = new Mock<IJobPostings>();
            _mockLogger = new Mock<ILogger<JobPostingsController>>();
            _mockConfig = new Mock<IConfiguration>();
            _controller = new JobPostingsController(_mockJobPostingsRepository.Object, _mockLogger.Object, _mockConfig.Object);
        }

        //[Fact]
        //public async Task GetJobPostingById_ReturnsNotFound_WhenNoJobPostingsExist()
        //{
        //    // Arrange
        //    string recruiterEmail = "contact@techwizards.com";

        //    // Mock the repository method to return an empty collection
        //    _mockJobPostingsRepository.Setup(repo => repo.GetJobPostingsAsync(recruiterEmail))
        //             .ReturnsAsync(Enumerable.Empty<JobPostings>());

        //    // Act
        //    var result = await _controller.GetJobPostingById(recruiterEmail);

        //    // Assert
        //    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        //    Assert.Equal($"Job Posting with recruiteremail {recruiterEmail} not found.", ((dynamic)notFoundResult.Value).message);
        //}


        //[Fact]
        //public async Task GetJobPostingById_ReturnsOk_WhenJobPostingExists()
        //{
        //    // Arrange
        //    string recruiterEmail = "contact@techwizards.com";
        //    var jobPostings = new List<JobPostings>
        //    {
        //        new JobPostings { job_title = "Software Developer", remail = recruiterEmail }
        //    };

        //    // Mock the repository method
        //    _mockJobPostingsRepository.Setup(repo => repo.GetJobPostingsAsync(recruiterEmail))
        //             .ReturnsAsync(jobPostings);

        //    // Act
        //    var result = await _controller.GetJobPostingById(recruiterEmail);

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result.Result);
        //    var returnValue = Assert.IsType<JobPostings>(okResult.Value);
        //    Assert.Equal("Software Developer", returnValue.job_title);
        //}


        [Fact]
        public async Task AddJobPosting_ReturnsOkResult_WhenJobPostingIsInsertedSuccessfully()
        {
            // Arrange
            var newJobPosting = new JobPostings { job_title = "Data Analyst", remail = "support@quantumsoft.com" };
            _mockJobPostingsRepository.Setup(repo => repo.InsertJobPostingAsync(newJobPosting))
                                      .ReturnsAsync(true);

            // Act
            var result = await _controller.AddJobPosting(newJobPosting);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Job Posting inserted successfully.", okResult.Value);
        }

        [Fact]
        public async Task AddJobPosting_ReturnsBadRequest_WhenJobPostingInsertionFails()
        {
            // Arrange
            var newJobPosting = new JobPostings { job_title = "Data Analyst", remail = "support@quantumsoft.com" };
            _mockJobPostingsRepository.Setup(repo => repo.InsertJobPostingAsync(newJobPosting))
                                      .ReturnsAsync(false);

            // Act
            var result = await _controller.AddJobPosting(newJobPosting);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to insert job posting.", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteJobPosting_ReturnsOkResult_WhenJobPostingIsDeletedSuccessfully()
        {
            // Arrange
            string job_title = "Software Developer";
            string remail = "contact@techwizards.com";
            _mockJobPostingsRepository.Setup(repo => repo.DeleteJobPostingAsync(job_title, remail))
                                      .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteJobPosting(job_title, remail);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Job Posting deleted successfully.", okResult.Value);
        }

        [Fact]
        public async Task DeleteJobPosting_ReturnsBadRequest_WhenDeletionFails()
        {
            // Arrange
            string job_title = "Software Developer";
            string remail = "contact@techwizards.com";
            _mockJobPostingsRepository.Setup(repo => repo.DeleteJobPostingAsync(job_title, remail))
                                      .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteJobPosting(job_title, remail);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to deleted job posting.", badRequestResult.Value);
        }
    }
}
