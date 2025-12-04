using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApplication2.Models;
using Xunit;

namespace WebApplication2.Tests
{
    // Enhetstester for ObstacleData-modellen
    public class ObstacleDataTests
    {
        /// <summary>
        /// Hjelpemetode som kjører DataAnnotation-validering på et objekt.
        /// Returnerer en liste med eventuelle valideringsfeil.
        /// </summary>
        private static IList<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, results, validateAllProperties: true);
            return results;
        }

        [Fact]
        public void ObstacleData_WithValidData_IsValid()
        {
            // Arrange: Oppretter et gyldig hinder med alle nødvendige verdier
            var obstacle = new ObstacleData
            {
                ObstacleName = "Testmast",
                ObstacleHeight = 75,
                Latitude = 58.1234,
                Longitude = 8.1234,
                GeometryGeoJson = "{ \"type\": \"Point\", \"coordinates\": [8.1234, 58.1234] }"
            };

            // Act: Validerer modellen
            var results = ValidateModel(obstacle);

            // Assert: Forventer ingen valideringsfeil
            Assert.Empty(results);
        }

        [Fact]
        public void ObstacleData_MissingName_IsInvalid()
        {
            // Arrange: Navn mangler (Required)
            var obstacle = new ObstacleData
            {
                ObstacleHeight = 50
            };

            // Act
            var results = ValidateModel(obstacle);

            // Assert: Skal gi valideringsfeil
            Assert.NotEmpty(results);
        }

        [Fact]
        public void ObstacleData_NegativeHeight_IsInvalid()
        {
            // Arrange: Ugyldig høyde (Range: 0–1000)
            var obstacle = new ObstacleData
            {
                ObstacleName = "Feil mast",
                ObstacleHeight = -10
            };

            // Act
            var results = ValidateModel(obstacle);

            // Assert: Skal gi valideringsfeil
            Assert.NotEmpty(results);
        }

        [Fact]
        public void ObstacleData_HeightAboveLimit_IsInvalid()
        {
            // Arrange: Høyde overskrider maksgrense (1000 meter)
            var obstacle = new ObstacleData
            {
                ObstacleName = "Overlimit mast",
                ObstacleHeight = 2000
            };

            // Act
            var results = ValidateModel(obstacle);

            // Assert: Skal gi valideringsfeil
            Assert.NotEmpty(results);
        }
    }
}


