namespace Web.ApiModels.Request
{
    public class ModifyReservationRequest
    {
        public string ReservationId { get; set; }
        public DateTime Starts { get; set; }
        public DateTime Ends { get; set; }
    }
}
