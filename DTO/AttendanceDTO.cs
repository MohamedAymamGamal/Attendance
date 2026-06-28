namespace Job.DTO
{
    public record CheckInDto
    {
        public string UserId { get; set; }
    }

    public record CheckOutDto : CheckInDto
    {

    }

    public record AttendanceDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Department { get; set; }
        public DateOnly Date { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string Status { get; set; }
        public int WorkedMinutes { get; set; }
        public string WorkedFormatted { get; set; }  
    }

    public record AttendanceFilterDto
    {
        public string? UserId { get; set; }
        public DateOnly? From { get; set; }
        public DateOnly? To { get; set; }
        public string? Department { get; set; }
    }
}
