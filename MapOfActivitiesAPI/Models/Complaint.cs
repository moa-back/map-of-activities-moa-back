namespace MapOfActivitiesAPI.Models
{
    public class Complaint
    {
        public int Id { get; set; }
        public int? AuthorId { get; set; }

        public string Header { get; set; }

        public string? Description { get; set; }

        public int? EventId { get; set; }
        public Event? Event { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}
