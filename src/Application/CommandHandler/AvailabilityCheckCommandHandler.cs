using Application.Command;
using Application.Query;
using Domain.Entities;
using Domain.Entities.Responses;
using Domain.Enum;
using MediatR;

namespace Application.CommandHandler
{
    public class AvailabilityCheckCommandHandler : IRequestHandler<AvailabilityCheckCommand, CheckAvailabilityResponse>
    {
        private readonly IMediator _mediator;
        private CheckAvailabilityResponse _checkAvailabilityResponse;
        private Configuration _configuration;
        private List<Error> _errors;

        public AvailabilityCheckCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<CheckAvailabilityResponse> Handle(AvailabilityCheckCommand request, CancellationToken cancellationToken)
        {
            _checkAvailabilityResponse = new CheckAvailabilityResponse();
            _errors = new List<Error>();

            InputValidations(request);

            if (!_errors.Any())
            {
                _configuration = await _mediator.Send(new GetConfiguration());

                if (_configuration is null)
                    _errors.Add(new Error(AvailabilityProblems.InternalProblem));
                else
                {
                    _checkAvailabilityResponse.Starts = request.Starts.Date.Add(_configuration.ReservationStartsAt);
                    _checkAvailabilityResponse.Ends = request.Ends.Date.Add(_configuration.ReservationEndsAt);
                    await BusinessValidationsAsync(request);
                    await AvailabilityValidationAsync(request);
                }
            }

            if (_errors.Any())
            {
                _checkAvailabilityResponse.Errors = _errors.Distinct().ToList();
                _checkAvailabilityResponse.IsAvailable = false;
            }

            return _checkAvailabilityResponse;
        }

        private void InputValidations(AvailabilityCheckCommand request)
        {
            if (request.Starts.Date > request.Ends.Date)
                _errors.Add(new Error(AvailabilityProblems.EndDateIsGreaterThanStartDate));

            if (request.Starts.Date < DateTime.UtcNow.Date)
                _errors.Add(new Error(AvailabilityProblems.StartDateIsOnThePast));

            if (request.Starts.Date == DateTime.UtcNow.Date)
                _errors.Add(new Error(AvailabilityProblems.NotPossibleToStart));
        }

        private async Task BusinessValidationsAsync(AvailabilityCheckCommand request)
        {
            var differenceEndDate = request.Ends.Date.Subtract(DateTime.UtcNow.Date).Days;
            if (differenceEndDate > 0 && _configuration.MaximumEndReservationDays <= differenceEndDate)
                _errors.Add(new Error(String.Format(AvailabilityProblems.MaximumEndReservationDays, _configuration.MaximumEndReservationDays)));

            var differenceRequestStartEndDates = request.Ends.Date.Subtract(request.Starts.Date).Days;
            if (_configuration.MaximumReservationDays <= differenceRequestStartEndDates)
                _errors.Add(new Error(String.Format(AvailabilityProblems.MaximumReservationDays, _configuration.MaximumReservationDays)));
        }

        private async Task AvailabilityValidationAsync(AvailabilityCheckCommand request)
        {
            if (_configuration is null)
            {
                _errors.Add(new Error(AvailabilityProblems.InternalProblem));
                return;
            }

            var reservations = await _mediator.Send(new GetReservationsBetween(request.Starts, request.Ends));
            reservations.RemoveAll(r => r.Id == request.ReservationDismiss);
            if (reservations != null && reservations.Any())
                _errors.Add(new Error(AvailabilityProblems.DatesAreUnavailable));
        }
    }
}
