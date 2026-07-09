namespace UmaFestHub.Application.Messaging;

/// <summary>Culture-neutral message reference resolved at display time via i18n key + args.</summary>
public sealed record UserMessage(string Key, params object[] Args);

public static class UserMessageKeys
{
	public const string Cart_DuplicateItem = "Cart_DuplicateItem";
	public const string Cart_ProductNotFound = "Cart_ProductNotFound";
	public const string Cart_QuantityInvalid = "Cart_QuantityInvalid";

	public const string Purchase_CheckoutEmpty = "Purchase_CheckoutEmpty";
	public const string Purchase_PaymentFailed = "Purchase_PaymentFailed";
	public const string Purchase_SessionNotFound = "Purchase_SessionNotFound";
	public const string Purchase_SessionAlreadyStarted = "Purchase_SessionAlreadyStarted";
	public const string Purchase_AlreadyOwnRental = "Purchase_AlreadyOwnRental";
	public const string Purchase_AlreadyOwnDailyPass = "Purchase_AlreadyOwnDailyPass";
	public const string Purchase_AlreadyOwnCompletePass = "Purchase_AlreadyOwnCompletePass";
	public const string Purchase_FestivalNotFound = "Purchase_FestivalNotFound";
	public const string Purchase_FestivalEnded = "Purchase_FestivalEnded";

	public const string SessionAccess_Denied = "SessionAccess_Denied";
	public const string SessionAccess_SessionNotFound = "SessionAccess_SessionNotFound";
	public const string SessionAccess_RentalWindow = "SessionAccess_RentalWindow";
	public const string SessionAccess_NoRentalProduct = "SessionAccess_NoRentalProduct";
	public const string SessionAccess_PassRequired = "SessionAccess_PassRequired";
	public const string SessionAccess_NoValidTicket = "SessionAccess_NoValidTicket";
	public const string SessionAccess_NotStartedYet = "SessionAccess_NotStartedYet";
	public const string SessionAccess_SessionEnded = "SessionAccess_SessionEnded";

	public const string Session_FestivalFilmRequired = "Session_FestivalFilmRequired";
	public const string Session_UnknownType = "Session_UnknownType";
	public const string Session_DuplicateStartTime = "Session_DuplicateStartTime";
	public const string Session_InvalidSchedule = "Session_InvalidSchedule";

	public const string Vote_NominationNotFound = "Vote_NominationNotFound";
	public const string Vote_AwardClosed = "Vote_AwardClosed";
	public const string Vote_AlreadyVotedAward = "Vote_AlreadyVotedAward";
	public const string Vote_AlreadyVotedNominee = "Vote_AlreadyVotedNominee";

	public const string Review_InvalidFestivalFilm = "Review_InvalidFestivalFilm";
	public const string Review_InvalidReviewId = "Review_InvalidReviewId";
	public const string Review_NotFoundForTitle = "Review_NotFoundForTitle";
}
