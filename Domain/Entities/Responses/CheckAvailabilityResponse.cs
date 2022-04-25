namespace Domain.Entities.Responses
{
    public class CheckAvailabilityResponse
    {
        public bool IsAvailable { get; set; }
        public DateTime Starts { get; set; }
        public DateTime Ends { get; set; }
        public List<Error> Errors { get; set; }
        public CheckAvailabilityResponse()
        {
            IsAvailable = true;
            Errors = new List<Error>();
        }
    }
}
