namespace Domain.Entities
{
    public class Reservation
    {
        public string Id { get; set; }
        public string UserIdentification { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool Active { get; set; }
        public DateTime Starts { get; set; }
        public DateTime Ends { get; set; }

        public Reservation(string userIdentification, DateTime starts, DateTime ends)
        {
            Id = Guid.NewGuid().ToString();
            Active = true;
            ReservationDate = ModifiedDate = DateTime.Now;
            UserIdentification = userIdentification;
            Starts = starts;
            Ends = ends;
        }

        public void ReservationUpdate(DateTime starts, DateTime ends)
        {
            ModifiedDate = DateTime.Now;
            Starts = starts;
            Ends = ends;
        }

        public void ReservationDelete()
        {
            Active = false;
            ModifiedDate = DateTime.Now;
        }
    }
}
