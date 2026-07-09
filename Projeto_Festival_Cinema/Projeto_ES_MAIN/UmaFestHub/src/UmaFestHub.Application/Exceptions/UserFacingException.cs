using UmaFestHub.Application.Messaging;

namespace UmaFestHub.Application.Exceptions;

/// <summary>Exception carrying i18n message keys for user-visible warnings and modals.</summary>
public sealed class UserFacingException : InvalidOperationException
{
	public IReadOnlyList<UserMessage> Messages { get; }

	public UserFacingException(UserMessage message)
		: base(message.Key)
	{
		Messages = new[] { message };
	}

	public UserFacingException(IEnumerable<UserMessage> messages)
		: base(string.Join("; ", messages.Select(m => m.Key)))
	{
		Messages = messages.ToList();
	}
}
