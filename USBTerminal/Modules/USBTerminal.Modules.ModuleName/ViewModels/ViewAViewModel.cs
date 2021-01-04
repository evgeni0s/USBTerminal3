using Prism.Regions;
using Serilog;
using USBTerminal.Core.Mvvm;
using USBTerminal.Services.Interfaces;

namespace USBTerminal.Modules.ModuleName.ViewModels
{
    public class ViewAViewModel : RegionViewModelBase
    {
        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public ViewAViewModel(IRegionManager regionManager, ILogger logger, IMessageService messageService) :
            base(regionManager, logger)
        {
            Message = messageService.GetMessage();
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            //do something
        }
    }
}
