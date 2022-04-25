namespace Domain.Entities
{
    public class Configuration
    {
        public TimeSpan ReservationStartsAt { get; set; }
        public TimeSpan ReservationEndsAt { get; set; }
        public int MaximumReservationDays { get; set; }
        public int MaximumEndReservationDays { get; set; }
    }
}
