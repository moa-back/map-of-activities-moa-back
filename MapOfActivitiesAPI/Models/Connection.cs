namespace MapOfActivitiesAPI.Models
{
    public class Connection
    {
        public int Id { get; set; }
        public string ConnectionId { get; set; }
        public List<Visitings>? VisitingsList { get; set; }

    }
}
