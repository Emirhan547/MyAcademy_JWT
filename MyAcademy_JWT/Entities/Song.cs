namespace MyAcademy_JWT.Entities
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;

        public int ArtistId { get; set; }
        public Artist? Artist { get; set; }

        public int AlbumId { get; set; }
        public Album? Album { get; set; }

        public int ContentLevel { get; set; } // 1-6

        public string Mp3FileName { get; set; } = null!;
        public TimeSpan Duration { get; set; }
        public int PlayCount { get; set; }
    }
}
