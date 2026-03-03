namespace MyAcademy_JWT.Models
{
    public class HomeSongViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ArtistName { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }
        public int PlayCount { get; set; }
    }
}
