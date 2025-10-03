namespace optimisation_developpement_tp7
{
    public class Log
    {
        public long Timestamp { get; set; }
        public required string UserId { get; set; }
        public Guid ActionId { get; set; }
    }
}
