namespace MyAcademy_JWT.Models
{
    public class SongCardViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string ArtistName { get; set; } = null!;
        public string? CoverImageUrl { get; set; }
        public int PlayCount { get; set; }

        public bool CanAccess { get; set; }
    }
}
