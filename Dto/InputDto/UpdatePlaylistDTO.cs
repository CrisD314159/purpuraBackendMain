namespace purpuraMain.Dto.InputDto;

    public class UpdatePlaylistDTO
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
