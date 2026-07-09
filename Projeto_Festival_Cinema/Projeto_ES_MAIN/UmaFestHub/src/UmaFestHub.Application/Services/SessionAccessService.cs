using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Handlers;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Application.Messaging;

public sealed class SessionAccessService : ISessionAccessService
{
    private readonly SessionExistsHandler _existsHandler;
    private readonly SessionTimeGateHandler _timeGateHandler;
    private readonly AccessWindowAccessHandler _accessWindowHandler;
    private readonly CompletePassAccessHandler _completePassHandler;
    private readonly DailyPassAccessHandler _dailyPassHandler;
    private readonly UserHasAccessHandler _ticketHandler;

    public SessionAccessService(
        SessionExistsHandler existsHandler,
        SessionTimeGateHandler timeGateHandler,
        AccessWindowAccessHandler accessWindowHandler,
        CompletePassAccessHandler completePassHandler,
        DailyPassAccessHandler dailyPassHandler,
        UserHasAccessHandler ticketHandler)
    {
        _existsHandler = existsHandler;
        _timeGateHandler = timeGateHandler;
        _accessWindowHandler = accessWindowHandler;
        _completePassHandler = completePassHandler;
        _dailyPassHandler = dailyPassHandler;
        _ticketHandler = ticketHandler;

        _existsHandler
            .SetNext(_timeGateHandler)
            .SetNext(_accessWindowHandler)
            .SetNext(_completePassHandler)
            .SetNext(_dailyPassHandler)
            .SetNext(_ticketHandler);
    }

    public Task<(bool Allowed, UserMessage? Error)> ValidateAccessAsync(
        SessionAccessDto sessionAccessDto,
        CancellationToken cancellationToken = default)
        => _existsHandler.HandleAsync(sessionAccessDto, cancellationToken);
}
