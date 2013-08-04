/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:ColoritWPF"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using System.ComponentModel;
using ColoritWPF.ViewModel.Products;
using ColoritWPF.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace ColoritWPF.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainViewModel>();
        }


        public ColorsMainViewModel ColorsMainPage
        {
            get
            {
                return new ColorsMainViewModel();
            }
        }

        public AddClientViewMode AddClientPage
        {
            get
            {
                return new AddClientViewMode();
            }
        }

        public AddNewCarViewModel AddCarModelPage
        {
            get
            {
                return new AddNewCarViewModel();
            }
        }

        public ClientEditorViewModel ClientEditorPage
        {
            get
            {
                return new ClientEditorViewModel();
            }
        }

        public PaintEditorViewModel PaintsEdtiorPage
        {
            get
            {
                return new PaintEditorViewModel();
            }
        }

        public SettingsViewModel SettingsPage
        {
            get
            {
                return new SettingsViewModel();
            }
        }

        public PaintsSalesWatcherViewModel PaintsWatcherPage
        {
            get
            {
                return new PaintsSalesWatcherViewModel();
            }
        }

        public DensityViewModel DensityPage
        {
            get
            {
                return new DensityViewModel();
            }
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public AddNewDensityItemViewModel AddNewDensityItemPage
        {
            get
            {
                return new AddNewDensityItemViewModel();
            }
        }

        public MenuItemsViewModel MenuItemsViewModelPage
        {
            get
            {
                return new MenuItemsViewModel();
            }
        }

        public ProductsViewModel ProductsViewModelPage
        {
            get
            {
                return new ProductsViewModel();
            }
        }

        public ProductsSelectorViewModel ProductsSelectorViewModelPage
        {
            get
            {
                return new ProductsSelectorViewModel();
            }
        }

        public TransferProductViewModel TransferProductViewModelPage
        {
            get
            {
                return new TransferProductViewModel();
            }
        }
        public TransferProductsSelectorViewModel TransferProductsSelectorViewModelPage
        {
            get
            {
                return new TransferProductsSelectorViewModel();
            }
        }
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}