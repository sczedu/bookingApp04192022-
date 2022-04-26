namespace Domain.Entities.Responses
{
    public class ReservationResponse
    {
        public string Id { get; set; }
        public DateTime Starts { get; set; }
        public DateTime Ends { get; set; }
        public List<Error> Errors { get; set; }

        public ReservationResponse(string id, DateTime starts, DateTime ends)
        {
            Id = id;
            Starts = starts;
            Ends = ends;
            Errors = new List<Error>();
        }

        public ReservationResponse(Error error)
        {
            Errors = Errors ?? new List<Error>();
            Errors.Add(error);
        }

        public ReservationResponse(List<Error> errors, DateTime starts, DateTime ends)
        {
            Errors = Errors ?? new List<Error>();
            Errors.AddRange(errors);
            Starts = starts;
            Ends = ends;
        }
    }
}
