using TiVerse.WebUI.ViewModels;

namespace TiVerse.Application.Interfaces.IAccountServiceInterface
{
    public interface IAccountService
    {
        void UpdateAccountInfo(PersonalInfoViewModel model);
    }
}
