using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class ObstacleData
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Obstacle name is required.")]
        [StringLength(100, ErrorMessage = "Obstacle name cannot be longer than 100 characters.")]
        public string ObstacleName { get; set; } = string.Empty;

        [Range(0, 1000, ErrorMessage = "Height must be between 0 and 1000 meters.")]
        [Display(Name = "Obstacle Height (meters)")]
        public double ObstacleHeight { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string ObstacleDescription { get; set; } = string.Empty;

        // 📍 Enkeltpunkt (for klikk i kart)
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
        public double? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
        public double? Longitude { get; set; }

        // NEW: GeoJSON (FeatureCollection) for tegnet geometri i kartet
        [Required(ErrorMessage = "Please draw the location/area on the map.")]
        public string? GeometryGeoJson { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Status { get; set; } = "Pending";



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
