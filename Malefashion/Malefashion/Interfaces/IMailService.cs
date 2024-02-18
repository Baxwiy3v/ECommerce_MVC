using Malefashion.Models.ViewModels.Mailsender;

namespace Malefashion.Interfaces
{
	public interface IMailService
	{
		Task SendEmailAsync(MailRequestVM mailRequest);
	}
}
