namespace MyAcademy_JWT.Entities
{
    public class UserSongHistory
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;
        public AppUser? User { get; set; }

        public int SongId { get; set; }
        public Song? Song { get; set; }

        public DateTime PlayedAt { get; set; }
    }
}
