using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using CommonServiceLocator;

namespace PESTApply
{
    public class ViewModelLocator
    {
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
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<HomeViewModel>();
            SimpleIoc.Default.Register<DictionaryViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public LoginViewModel Login
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LoginViewModel>();
            }
        }

        public HomeViewModel Home
        {
            get
            {
                return ServiceLocator.Current.GetInstance<HomeViewModel>();
            }
        }

        public DictionaryViewModel Dictionary
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DictionaryViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}