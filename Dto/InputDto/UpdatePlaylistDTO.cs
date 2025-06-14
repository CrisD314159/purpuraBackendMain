namespace purpuraMain.Dto.InputDto;

    public class UpdatePlaylistDTO
    {
        public required Guid Id { get; set; }

        public string? ImageUrl { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
