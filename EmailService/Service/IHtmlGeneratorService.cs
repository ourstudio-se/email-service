using System.Threading.Tasks;
using EmailService.ViewModels;

namespace EmailService.Service
{
	public interface IHtmlGeneratorService
	{
		Task<string> GetRawHtmlAsync(string view, EmailViewModel viewModel);
	}
}