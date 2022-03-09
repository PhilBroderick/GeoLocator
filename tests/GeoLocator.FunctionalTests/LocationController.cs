using GeoLocator.Web.ApiModels;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace GeoLocator.FunctionalTests
{
    public class LocationController : IClassFixture<WebTestFixture>
    {
        private readonly HttpClient _httpClient;

        public LocationController(WebTestFixture factory)
        {
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task GetLocationByIpAddress_ValidIpAddress_ReturnsLocationInformation()
        {
            // Arrange
            var ipFromUK = "81.170.32.169";

            // Act
            var response = await _httpClient.GetAsync($"/api/location?ipAddress={ipFromUK}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var locationDto = JsonConvert.DeserializeObject<LocationDTO>(content);

            // Assert
            Assert.Equal("United Kingdom", locationDto.Country);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetLocationByIpAddress_InvalidIpAddress_Returns404()
        {
            // Arrange
            var ipFromUK = "invalid-ip";

            // Act
            var response = await _httpClient.GetAsync($"/api/location?ipAddress={ipFromUK}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetLocationByIpAddress_NoIpAddressAndRunningLocally_Returns400()
        {
            // Act
            var response = await _httpClient.GetAsync("/api/location");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}