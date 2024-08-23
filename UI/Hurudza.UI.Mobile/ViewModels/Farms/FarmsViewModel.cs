using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hurudza.Common.Utils.Extensions;
using Hurudza.Common.Utils.Helpers;
using Hurudza.Common.Utils.Models;
using Hurudza.Data.Enums.Enums;
using Hurudza.UI.Mobile.Helpers;
using Hurudza.UI.Mobile.Models;
using Hurudza.UI.Mobile.Pages.Farms;
using Hurudza.UI.Mobile.Services;
using Hurudza.UI.Mobile.Services.Interfaces;
using Microsoft.Datasync.Client;
using System.Collections.ObjectModel;
using Region = Hurudza.Data.Enums.Enums.Region;
using Sensors = Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.ApplicationModel;

namespace Hurudza.UI.Mobile.ViewModels.Farms
{
    public partial class FarmsViewModel : ObservableObject, IMVVMHelper
    {
        private readonly LocationService _locationService;
        private readonly IProvinceService _provinceService;
        private readonly IDistrictService _districtService;
        private readonly ILocalAuthorityService _localAuthorityService;
        private readonly IWardService _wardService;
        private readonly IFarmService _farmService;

        public FarmsViewModel(
            LocationService locationService,
            IProvinceService provinceService,
            IDistrictService districtService,
            ILocalAuthorityService localAuthorityService,
            IWardService wardService,
            IFarmService farmService)
        {
            _locationService = locationService;
            _provinceService = provinceService;
            _districtService = districtService;
            _localAuthorityService = localAuthorityService;
            _wardService = wardService;
            _farmService = farmService;
            Regions = new ObservableCollection<EnumItem>(typeof(Region).ToList());
            Conferences = new ObservableCollection<EnumItem>(typeof(Conference).ToList());
            Terrains = new ObservableCollection<EnumItem>(typeof(Terrain).ToList());
            WaterAvailabilities = new ObservableCollection<EnumItem>(typeof(WaterAvailability).ToList());
            Roads = new ObservableCollection<EnumItem>(typeof(RoadAccess).ToList());
        }

        public CreateFarmSheet CreateFarmSheet { get; set; }

        public ConcurrentObservableCollection<Farm> AllFarms { get; set; } = new ConcurrentObservableCollection<Farm>();
        public ConcurrentObservableCollection<Farm> Farms { get; set; } = new ConcurrentObservableCollection<Farm>();
        public ConcurrentObservableCollection<Province> Provinces { get; set; } = new ConcurrentObservableCollection<Province>();
        public ConcurrentObservableCollection<Province> AllProvinces { get; set; } = new ConcurrentObservableCollection<Province>();
        public ConcurrentObservableCollection<District> Districts { get; set; } = new ConcurrentObservableCollection<District>();
        public ConcurrentObservableCollection<District> AllDistricts { get; set; } = new ConcurrentObservableCollection<District>();
        public ConcurrentObservableCollection<LocalAuthority> LocalAuthorities { get; set; } = new ConcurrentObservableCollection<LocalAuthority>();
        public ConcurrentObservableCollection<LocalAuthority> AllLocalAuthorities { get; set; } = new ConcurrentObservableCollection<LocalAuthority>();
        public ConcurrentObservableCollection<Ward> Wards { get; set; } = new ConcurrentObservableCollection<Ward>();
        public ConcurrentObservableCollection<Ward> AllWards { get; set; } = new ConcurrentObservableCollection<Ward>();
        public ObservableCollection<EnumItem> Regions { get; set; }
        public ObservableCollection<EnumItem> Conferences { get; set; }
        public ObservableCollection<EnumItem> Terrains { get; set; }
        public ObservableCollection<EnumItem> WaterAvailabilities { get; set; }
        public ObservableCollection<EnumItem> Roads { get; set; }

        [ObservableProperty] Farm _selectedFarm;

        [ObservableProperty] string _searchText;

        [ObservableProperty]
        Sensors.Location _currentLocation;
        [ObservableProperty] double _latitude;
        [ObservableProperty] double _longitude;

        [ObservableProperty] string _id;
        [ObservableProperty] string _name;
        [ObservableProperty] string _description;
        [ObservableProperty] string _address;
        [ObservableProperty] string _vision;
        [ObservableProperty] string _mission;
        [ObservableProperty] string _website;
        [ObservableProperty] int _year;
        [ObservableProperty] string _foundingMembers;
        [ObservableProperty] string _keyMilestones;
        [ObservableProperty] string _contactPerson;
        [ObservableProperty] string _contactNumber;
        [ObservableProperty] string _contactEmail;
        [ObservableProperty] double _farmSize;
        [ObservableProperty] double _arable;
        [ObservableProperty] double _cleared;
        [ObservableProperty] string _waterSource;
        [ObservableProperty] EnumItem _waterAvailability;
        [ObservableProperty] bool _isIrrigated;
        [ObservableProperty] string _equipment;
        [ObservableProperty] string _soilType;
        [ObservableProperty] string _buildings;
        [ObservableProperty] EnumItem _roadAccess;
        [ObservableProperty] string _personnel;
        [ObservableProperty] string _recommendations;
        [ObservableProperty] string _problems;
        [ObservableProperty] EnumItem _region;
        [ObservableProperty] EnumItem _terrain;
        [ObservableProperty] double _elevation;
        [ObservableProperty] string _pastCrops;
        [ObservableProperty] string _landManagementPractice;
        [ObservableProperty] string _securityMeasures;
        [ObservableProperty] string _communityPrograms;
        [ObservableProperty] string _partnerships;
        [ObservableProperty] string _classrooms;
        [ObservableProperty] string _laboratories;
        [ObservableProperty] string _sportsFacilities;
        [ObservableProperty] string _library;
        [ObservableProperty] string _averageExamScores;
        [ObservableProperty] string _extraCurricularAchievements;
        [ObservableProperty] int _maleTeachers;
        [ObservableProperty] EnumItem _conference;
        [ObservableProperty] Province _province;
        [ObservableProperty] District _district;
        [ObservableProperty] LocalAuthority _localAuthority;
        [ObservableProperty] Ward _ward;
        [ObservableProperty] int _femaleTeachers;
        [ObservableProperty] int _maleStudents;
        [ObservableProperty] int _femaleStudents;
        [ObservableProperty] string _studentDemographics;
        [ObservableProperty] int _dayScholars;
        [ObservableProperty] int _boardingScholars;
        [ObservableProperty] string _nonTeachingStaff;
        [ObservableProperty] bool _isRefreshing;

        [ObservableProperty] string _createFarmTitle;

        [RelayCommand]
        async Task InitAsync()
        {
            await SetRefreshing(true);

            try
            {
                //await _farmService.RefreshItemsAsync();

                var provinces = await _provinceService.GetItemsAsync();
                var farms = await _farmService.GetItemsAsync();

                await RunOnUiThreadAsync(() =>
                {
                    AllProvinces.ReplaceAll(provinces);
                    Provinces.ReplaceAll(provinces);
                    AllFarms.ReplaceAll(farms);
                    Farms.ReplaceAll(farms);
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Refresh", ex.Message, "OK");
            }
            finally
            {
                _farmService.ItemsUpdated += OnFarmsUpdated;
                await SetRefreshing(false);
            }
        }

        public async void FilterDistricts(string provinceId)
        {
            if (AllDistricts.Count == 0)
            {
                var districts = await _districtService.GetItemsAsync();
                await RunOnUiThreadAsync(() => AllDistricts.ReplaceAll(districts));
            }

            if (string.IsNullOrEmpty(provinceId))
            {
                await RunOnUiThreadAsync(() => Districts.ReplaceAll(AllDistricts));
                return;
            }

            await RunOnUiThreadAsync(() => Districts.ReplaceAll(AllDistricts.Where(d => d.ProvinceId == provinceId)));
        }

        public async void FilterLocalAuthorities(string districtId)
        {
            if (AllLocalAuthorities.Count == 0)
            {
                var localAuthorities = await _localAuthorityService.GetItemsAsync();
                await RunOnUiThreadAsync(() => AllLocalAuthorities.ReplaceAll(localAuthorities));
            }

            if (AllWards.Count == 0)
            {
                var wards = await _wardService.GetItemsAsync();
                await RunOnUiThreadAsync(() => AllWards.ReplaceAll(wards));
            }

            if (string.IsNullOrEmpty(districtId))
            {
                await RunOnUiThreadAsync(() =>
                {
                    LocalAuthorities.ReplaceAll(AllLocalAuthorities);
                    Wards.ReplaceAll(AllWards);
                });
                return;
            }

            await RunOnUiThreadAsync(() =>
            {
                LocalAuthorities.ReplaceAll(AllLocalAuthorities.Where(la => la.DistrictId == districtId));
                Wards.ReplaceAll(AllWards.Where(w => w.DistrictId == districtId));
            });
        }

        public async void FilterWards(string localAuthorityId)
        {
            if (AllWards.Count == 0)
            {
                var wards = await _wardService.GetItemsAsync();
                await RunOnUiThreadAsync(() => AllWards.ReplaceAll(wards));
            }

            if (string.IsNullOrEmpty(localAuthorityId))
            {
                await RunOnUiThreadAsync(() => Wards.ReplaceAll(AllWards));
                return;
            }

            await RunOnUiThreadAsync(() => Wards.ReplaceAll(AllWards.Where(w => w.LocalAuthorityId == localAuthorityId)));
        }

        private async void OnFarmsUpdated(object sender, ServiceEventArgs<Farm> e)
        {
            await RunOnUiThreadAsync(() =>
            {
                switch (e.Action)
                {
                    case ServiceEventArgs<Farm>.ListAction.Add:
                        AllFarms.AddIfMissing(m => m.Id == e.Item.Id, e.Item);
                        break;
                    case ServiceEventArgs<Farm>.ListAction.Delete:
                        AllFarms.RemoveIf(m => m.Id == e.Item.Id);
                        break;
                    case ServiceEventArgs<Farm>.ListAction.Update:
                        AllFarms.ReplaceIf(m => m.Id == e.Item.Id, e.Item);
                        break;
                }

                Farms.ReplaceAll(AllFarms);
            });
        }

        private Task SetRefreshing(bool value)
        => RunOnUiThreadAsync(() => IsRefreshing = value);

        public Task RunOnUiThreadAsync(Action func) => MainThread.InvokeOnMainThreadAsync(func);

        [RelayCommand]
        async Task AddPointAsync()
        {
            CurrentLocation = await _locationService.GetCurrentLocation();
            Latitude = CurrentLocation?.Latitude ?? 0;
            Longitude = CurrentLocation?.Longitude ?? 0;
            Elevation = CurrentLocation?.Altitude ?? 0;
        }

        [RelayCommand]
        async Task GoToCreateFarmAsync()
        {
            await CancelEditAsync();
            CreateFarmTitle = "Create Farm";
            await Shell.Current.Navigation.PushModalAsync(new CreateFarmPage(), true);
        }

        [RelayCommand]
        async Task CancelEditAsync()
        {
            Id = null;
            Name = null;
            Description = null;
            Address = null;
            ContactPerson = null;
            ContactNumber = null;
            ContactEmail = null;
            Vision = null;
            Mission = null;
            Website = null;
            Year = 0;
            FoundingMembers = null;
            KeyMilestones = null;
            FarmSize = 0;
            Arable = 0;
            Cleared = 0;
            Terrain = null;
            Latitude = 0;
            Longitude = 0;
            Elevation = 0;
            PastCrops = null;
            LandManagementPractice = null;
            SecurityMeasures = null;
            WaterSource = null;
            WaterAvailability = null;
            IsIrrigated = false;
            Equipment = null;
            SoilType = null;
            Buildings = null;
            RoadAccess = null;
            Personnel = null;
            Recommendations = null;
            Problems = null;
            Region = null;
            Conference = null;
            Province = null;
            District = null;
            LocalAuthority = null;
            Ward = null;
            CommunityPrograms = null;
            Partnerships = null;
            Classrooms = null;
            Laboratories = null;
            SportsFacilities = null;
            Library = null;
            AverageExamScores = null;
            ExtraCurricularAchievements = null;
            FemaleTeachers = 0;
            MaleTeachers = 0;
            FemaleStudents = 0;
            MaleStudents = 0;
            StudentDemographics = null;
            DayScholars = 0;
            BoardingScholars = 0;
            NonTeachingStaff = null;
        }

        [RelayCommand]
        async Task AddFarmAsync()
        {
            await SetRefreshing(true);

            if (string.IsNullOrEmpty(Name) ||
                string.IsNullOrEmpty(Address) ||
                string.IsNullOrEmpty(ContactPerson) ||
                string.IsNullOrEmpty(ContactNumber))
            {
                await Shell.Current.DisplayAlert("Error!", "Please fill in all required fields", "OK");
                await SetRefreshing(false);
                return;
            }

            try
            {
                Farm farm = new()
                {
                    Name = Name,
                    Description = Description,
                    Address = Address,
                    Vision = Vision,
                    Mission = Mission,
                    Website = Website,
                    Year = Year,
                    FoundingMembers = FoundingMembers,
                    KeyMilestones = KeyMilestones,
                    ContactPerson = ContactPerson,
                    PhoneNumber = ContactNumber,
                    Email = ContactEmail,
                    Size = FarmSize,
                    Arable = Arable,
                    Cleared = Cleared,
                    Terrain = Terrain != null ? (Terrain)Terrain?.Id : null,
                    Latitude = Latitude,
                    Longitude = Longitude,
                    Elevation = Elevation,
                    PastCrops = PastCrops,
                    LandManagementPractice = LandManagementPractice,
                    SecurityMeasures = SecurityMeasures,
                    WaterSource = WaterSource,
                    WaterAvailability = WaterAvailability != null ? (WaterAvailability)WaterAvailability?.Id : null,
                    Irrigated = IsIrrigated,
                    Equipment = Equipment,
                    SoilType = SoilType,
                    Buildings = Buildings,
                    RoadAccess = RoadAccess != null ? (RoadAccess)RoadAccess?.Id : null,
                    Personnel = Personnel,
                    Problems = Problems,
                    Region = Region != null ? (Region)Region?.Id : null,
                    Conference = Conference != null ? (Conference)Conference?.Id : null,
                    ProvinceId = Province?.Id,
                    DistrictId = District?.Id,
                    LocalAuthorityId = LocalAuthority?.Id,
                    WardId = Ward?.Id,
                    CommunityPrograms = CommunityPrograms,
                    Partnerships = Partnerships,
                    Classrooms = Classrooms,
                    Laboratories = Laboratories,
                    SportsFacilities = SportsFacilities,
                    Library = Library,
                    AverageExamScores = AverageExamScores,
                    ExtraCurricularAchievements = ExtraCurricularAchievements,
                    MaleTeachers = MaleTeachers,
                    FemaleTeachers = FemaleTeachers,
                    MaleStudents = MaleStudents,
                    FemaleStudents = FemaleStudents,
                    StudentDemographics = StudentDemographics,
                    DayScholars = DayScholars,
                    BoardingScholars = BoardingScholars,
                    NonTeachingStaff = NonTeachingStaff
                };

                if (!string.IsNullOrEmpty(Id))
                {
                    farm.Id = Id;
                }

                await _farmService.SaveItemAsync(farm);

                await Shell.Current.Navigation.PopModalAsync(true);
            }
            catch (NullReferenceException)
            {
                await Shell.Current.DisplayAlert("Error!", "Please fill in all required fields", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                await SetRefreshing(false);
            }
        }

        [RelayCommand]
        async Task CancelAsync()
        {
            await Shell.Current.Navigation.PopModalAsync(true);
        }

        [RelayCommand]
        async Task DeleteAsync(Farm farm)
        {
            await SetRefreshing(true);
            await _farmService.RemoveItemAsync(farm);
            await SetRefreshing(false);
            return;
        }

        [RelayCommand]
        async Task SelectedFarmAsync()
        {
            await Shell.Current.DisplayActionSheet("What do you want to do?", "Cancel", null, "Edit", "Delete");
        }

        [RelayCommand]
        async Task EditAsync(Farm farm)
        {
            if (farm == null) return;

            Id = farm.Id;
            Name = farm.Name;
            Description = farm.Description;
            Address = farm.Address;
            Vision = farm.Vision;
            Mission = farm.Mission;
            Website = farm.Website;
            Year = farm.Year;
            FoundingMembers = farm.FoundingMembers;
            KeyMilestones = farm.KeyMilestones;
            ContactPerson = farm.ContactPerson;
            ContactNumber = farm.PhoneNumber;
            ContactEmail = farm.Email;
            FarmSize = farm.Size;
            Arable = farm.Arable;
            Cleared = farm.Cleared;
            Latitude = farm.Latitude;
            Longitude = farm.Longitude;
            Elevation = farm.Elevation;
            PastCrops = farm.PastCrops;
            LandManagementPractice = farm.LandManagementPractice;
            SecurityMeasures = farm.SecurityMeasures;
            WaterSource = farm.WaterSource;
            IsIrrigated = farm.Irrigated;
            Equipment = farm.Equipment;
            SoilType = farm.SoilType;
            Buildings = farm.Buildings;
            Personnel = farm.Personnel;
            Problems = farm.Problems;
            CommunityPrograms = farm.CommunityPrograms;
            Partnerships = farm.Partnerships;
            Classrooms = farm.Classrooms;
            Laboratories = farm.Laboratories;
            SportsFacilities = farm.SportsFacilities;
            Library = farm.Library;
            AverageExamScores = farm.AverageExamScores;
            ExtraCurricularAchievements = farm.ExtraCurricularAchievements;
            MaleTeachers = farm.MaleTeachers;
            FemaleTeachers = farm.FemaleTeachers;
            MaleStudents = farm.MaleStudents;
            FemaleStudents = farm.FemaleStudents;
            StudentDemographics = farm.StudentDemographics;
            DayScholars = farm.DayScholars;
            BoardingScholars = farm.BoardingScholars;
            NonTeachingStaff = farm.NonTeachingStaff;

            var page = new CreateFarmPage();
            page.BindingContext = this;
            CreateFarmTitle = "Edit Farm";
            await Shell.Current.Navigation.PushModalAsync(page, true);
        }

        [RelayCommand]
        async Task SearchFarm()
        {
            await SetRefreshing(true);

            await Task.Delay(TimeSpan.FromSeconds(3));

            if (string.IsNullOrEmpty(SearchText))
            {
                Farms.ReplaceAll(AllFarms);
            }
            else
            {
                Farms.ReplaceAll(AllFarms.Where(f => f.Name.ToLower().Trim().Contains(SearchText.ToLower().Trim())));
            }

            await SetRefreshing(false);
        }
    }
}
