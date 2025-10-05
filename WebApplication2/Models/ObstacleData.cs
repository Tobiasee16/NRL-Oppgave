using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class ObstacleData
    {
        [Required(ErrorMessage = "Obstacle name is required.")]
        [StringLength(100, ErrorMessage = "Obstacle name cannot be longer than 100 characters.")]
        public string ObstacleName { get; set; } = string.Empty;

        [Range(0, 1000, ErrorMessage = "Height must be between 0 and 1000 meters.")]
        [Display(Name = "Obstacle Height (meters)")]
        public double ObstacleHeight { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string ObstacleDescription { get; set; } = string.Empty;

        // NEW: GeoJSON (FeatureCollection) for tegnet geometri i kartet
        public string? GeometryGeoJson { get; set; }
        // Hvis dere vil gjøre kart obligatorisk:
        // [Required(ErrorMessage = "Please draw the location/area on the map.")]
        // public string GeometryGeoJson { get; set; } = string.Empty;
    }

    // Egendefinert attributt for å telle ord (beholdt uendret)
    public class MaxWordsAttribute : ValidationAttribute
    {
        private readonly int _maxWords;
        public MaxWordsAttribute(int maxWords)
        {
            _maxWords = maxWords;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string strValue)
            {
                var wordCount = strValue.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
                if (wordCount > _maxWords)
                {
                    return new ValidationResult(ErrorMessage ?? $"The field {validationContext.DisplayName} cannot exceed {_maxWords} words.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
