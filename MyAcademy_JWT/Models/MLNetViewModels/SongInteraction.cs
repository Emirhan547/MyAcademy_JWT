namespace MyAcademy_JWT.Models.MLNetViewModels
{
    public class SongInteraction
    {
        public string UserId { get; set; } = null!;
        public string SongId { get; set; } = null!;
        public float Label { get; set; } = 1f;
    }
}
