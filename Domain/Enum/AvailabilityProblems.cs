namespace Domain.Enum
{
    public static class AvailabilityProblems
    {
        public const string EndDateIsGreaterThanStartDate = "End date is greater than start date";
        public const string StartDateIsOnThePast = "Start date is on the past";
        public const string NotPossibleToStart = "Today is not possible to start";
        public const string InternalProblem = "Internal problem";
        public const string MaximumEndReservationDays = "Can’t be reserved more than {0} days in advance.";
        public const string MaximumReservationDays = "The stay can’t be longer than {0} days";
        public const string DatesAreUnavailable = "Dates are unavailable";
        public const string ReservationIsUnavailable = "Reservation is unavailable";
    }
}
