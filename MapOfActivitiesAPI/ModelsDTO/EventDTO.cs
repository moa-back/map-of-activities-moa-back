namespace MapOfActivitiesAPI.ModelsDTO
{
    public class EventDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public string TypeIcon { get; set; }
        public string TypeName { get; set; }
        public string Coordinates { get; set; }
    }
}
