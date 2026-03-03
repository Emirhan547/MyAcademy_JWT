namespace MyAcademy_JWT.Models
{
    public class HomeViewModel
    {
        public int TotalSongCount { get; set; }
        public int TotalArtistCount { get; set; }
        public int TotalPlayCount { get; set; }
        public List<HomeSongViewModel> TrendingSongs { get; set; } = new();
        public List<HomeArtistViewModel> PopularArtists { get; set; } = new();
    }
}
