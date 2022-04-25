namespace Web.ApiModels.Request
{
    public class ReservationRequest
    {
        public string UserIdentification { get; set; }
        public DateTime Starts { get; set; }
        public DateTime Ends { get; set; }
    }
}
