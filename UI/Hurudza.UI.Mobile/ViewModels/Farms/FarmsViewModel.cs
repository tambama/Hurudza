using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hurudza.Common.Utils.Extensions;
using Hurudza.Common.Utils.Models;
using Hurudza.Data.Enums.Enums;
using Hurudza.Data.UI.Models.ViewModels.Core;
using Hurudza.UI.Mobile.Pages.Farms;
using Hurudza.UI.Mobile.Services;
using System.Collections.ObjectModel;
using The49.Maui.BottomSheet;
using Region = Hurudza.Data.Enums.Enums.Region;
using Sensors = Microsoft.Maui.Devices.Sensors;

namespace Hurudza.UI.Mobile.ViewModels.Farms
{
    public partial class FarmsViewModel : ObservableObject
    {
        private readonly LocationService _locationService;

        public FarmsViewModel(LocationService locationService)
        {
            _locationService = locationService;

            Regions = new ObservableCollection<EnumItem>(typeof(Region).ToList());
            Conferences = new ObservableCollection<EnumItem>(typeof(Conference).ToList());

            CreateFarmSheet = new CreateFarmSheet();
            CreateFarmSheet.Detents = new DetentsCollection()
            {
                new RatioDetent(){ Ratio = 0.93F },
                new ContentDetent(),
            };
            CreateFarmSheet.BindingContext = this;
        }

        public CreateFarmSheet CreateFarmSheet { get; set; }

        public ObservableCollection<FarmViewModel> Farms { get; set; }
        public ObservableCollection<EnumItem> Regions { get; set; }
        public ObservableCollection<EnumItem> Conferences { get; set; }

        [ObservableProperty]
        Sensors.Location _currentLocation;

        [ObservableProperty] string _name;
        [ObservableProperty] string _description;
        [ObservableProperty] string _address;
        [ObservableProperty] string _contactPerson;
        [ObservableProperty] string _contactNumber;
        [ObservableProperty] string _contactEmail;
        [ObservableProperty] double _farmSize;
        [ObservableProperty] double _arable;
        [ObservableProperty] double _cleared;
        [ObservableProperty] string _waterSource;
        [ObservableProperty] bool _isIrrigated;
        [ObservableProperty] string _equipment;
        [ObservableProperty] EnumItem _soilType;
        [ObservableProperty] string _personnel;
        [ObservableProperty] string _problems;
        [ObservableProperty] EnumItem _region;
        [ObservableProperty] EnumItem _conference;
        [ObservableProperty] EnumItem _province;
        [ObservableProperty] EnumItem _district;
        [ObservableProperty] EnumItem _localAuthority;
        [ObservableProperty] EnumItem _ward;

        [RelayCommand]
        async Task InitAsync()
        {
            CurrentLocation = await _locationService.GetCurrentLocation();
        }

        [RelayCommand]
        async Task GoToCreateFarmAsync()
        {
            await Shell.Current.Navigation.PushModalAsync(new CreateFarmPage(), true);
        }

        [RelayCommand]
        async Task CancelEdit()
        {
            Name = null;
            Description = null;
            Address = null;
            ContactPerson = null;
            ContactNumber = null;
            ContactEmail = null;
            FarmSize = 0;
            Arable = 0;
            Cleared = 0;
            WaterSource = null;
            IsIrrigated = false;
            Equipment = null;
            SoilType = null;
            Personnel = null;
            Problems = null;
            Region = null;
            Conference = null;
            Province = null;
            District = null;
            LocalAuthority = null;
            Ward = null;

            await CreateFarmSheet.DismissAsync();
        }
    }
}
