using System.ComponentModel.DataAnnotations;
using Hurudza.Data.Enums.Enums;
using Hurudza.Data.UI.Models.ViewModels.Base;

namespace Hurudza.Data.UI.Models.ViewModels.Core;

public class FarmViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Address is required")]
    public string? Address { get; set; }
    public string? ContactPerson { get; set; }

    [Required(ErrorMessage = "Phone Number is required")]
    [RegularExpression(@"^((00|\+)?(263))?0?7(1|3|7|8)[0-9]{7}$", ErrorMessage = "Enter a valid mobile number")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }
    public string? Description { get; set; }
    public string? Vision { get; set; }
    public string? Mission { get; set; }
    public string? Website { get; set; }
    public int Year { get; set; }
    public string? FoundingMembers { get; set; }
    public string? KeyMilestones { get; set; }
    public float Size { get; set; }
    public float Arable { get; set; }
    public float Cleared { get; set; }
    public Terrain? Terrain { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Elevation { get; set; }
    public string? PastCrops { get; set; }
    public string? LandManagementPractice { get; set; }
    public string? SecurityMeasures { get; set; }
    public string? WaterSource { get; set; }
    public WaterAvailability? WaterAvailability { get; set; }
    public bool Irrigated { get; set; }
    public string? IrrigationSystems { get; set; }
    public string? Equipment { get; set; }
    public string? SoilType { get; set; }
    public RoadAccess? RoadAccess { get; set; }
    public string? Problems { get; set; }
    public string? Personnel { get; set; }
    public string? Recommendations { get; set; }
    public string? WardId { get; set; }
    public string? LocalAuthorityId { get; set; }
    public string? DistrictId { get; set; }
    public string? ProvinceId { get; set; }
    public Region? Region { get; set; }
    public Conference? Conference { get; set; }
    public string? CommunityPrograms { get; set; }
    public string? Partnerships { get; set; }
    public string? Classrooms { get; set; }
    public string? Laboratories { get; set; }
    public string? SportsFacilities { get; set; }
    public string? Library { get; set; }
    public string? AverageExamScores { get; set; }
    public string? ExtraCurricularAchievements { get; set; }
    public int MaleTeachers { get; set; }
    public int FemaleTeachers { get; set; }
    public int MaleStudents { get; set; }
    public int FemaleStudents { get; set; }
    public string? StudentDemographics { get; set; }
    public int DayScholars { get; set; }
    public int BoardingScholars { get; set; }
    public string? NonTeachingStaff { get; set; }
    public string? Buildings { get; set; }

    public string? Ward { get; set; }
    public string? LocalAuthority { get; set; }
    public string? District { get; set; }
    public string? Province { get; set; }
    
    public FarmType FarmType { get; set; } = FarmType.School;
    public string? ParentSchoolId { get; set; }
    public string? ParentSchoolName { get; set; }
    public bool RequiresTillageService { get; set; }
    public string? TillageRequirements { get; set; }
    public DateTime? LastTillageDate { get; set; }
    public string? CropRotationPlan { get; set; }
    
    public List<FarmViewModel> ManagedFarms { get; set; } = new List<FarmViewModel>();
    public List<FarmLocationViewModel> Locations { get; set; } = new List<FarmLocationViewModel>();
}

public class CreateFarmViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Address is required")]
    public string? Address { get; set; }
    public string? ContactPerson { get; set; }

    [Required(ErrorMessage = "Phone Number is required")]
    [RegularExpression(@"^((00|\+)?(263))?0?7(1|3|7|8)[0-9]{7}$", ErrorMessage = "Enter a valid mobile number")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }
    public string? Description { get; set; }
    public string? Vision { get; set; }
    public string? Mission { get; set; }
    public string? Website { get; set; }
    public int Year { get; set; }
    public string? FoundingMembers { get; set; }
    public string? KeyMilestones { get; set; }
    public float Size { get; set; }
    public float Arable { get; set; }
    public float Cleared { get; set; }
    public Terrain? Terrain { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Elevation { get; set; }
    public string? PastCrops { get; set; }
    public string? LandManagementPractice { get; set; }
    public string? SecurityMeasures { get; set; }
    public string? WaterSource { get; set; }
    public WaterAvailability? WaterAvailability { get; set; }
    public bool Irrigated { get; set; }
    public string? IrrigationSystems { get; set; }
    public string? Equipment { get; set; }
    public string? SoilType { get; set; }
    public RoadAccess? RoadAccess { get; set; }
    public string? Problems { get; set; }
    public string? Personnel { get; set; }
    public string? Recommendations { get; set; }
    public string? WardId { get; set; }
    public string? LocalAuthorityId { get; set; }
    public string? DistrictId { get; set; }
    public string? ProvinceId { get; set; }
    public Region? Region { get; set; }
    public Conference? Conference { get; set; }
    public string? CommunityPrograms { get; set; }
    public string? Partnerships { get; set; }
    public string? Classrooms { get; set; }
    public string? Laboratories { get; set; }
    public string? SportsFacilities { get; set; }
    public string? Library { get; set; }
    public string? AverageExamScores { get; set; }
    public string? ExtraCurricularAchievements { get; set; }
    public int MaleTeachers { get; set; }
    public int FemaleTeachers { get; set; }
    public int MaleStudents { get; set; }
    public int FemaleStudents { get; set; }
    public string? StudentDemographics { get; set; }
    public int DayScholars { get; set; }
    public int BoardingScholars { get; set; }
    public string? NonTeachingStaff { get; set; }
    public string? Buildings { get; set; }

    public string? Ward { get; set; }
    public string? LocalAuthority { get; set; }
    public string? District { get; set; }
    public string? Province { get; set; }
}

public class FarmMapViewModel : BaseViewModel
{
    public string Id { get; set; }
    public string? Name { get; set; }
    public float Size { get; set; }
    public float Arable { get; set; }
    public float Cleared { get; set; }
    public Terrain? Terrain { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double[] Center => [Longitude, Latitude];
    public bool IsPolygon => FarmCoordinates.Count > 0;
    public string? SoilType { get; set; }

    public List<List<double>> FarmCoordinates { get; set; } = new List<List<double>>();
    public List<List<List<double>>> FarmPolygon => new List<List<List<double>>>()
    {
        FarmCoordinates
    };

    public List<FieldViewModel> Fields { get; set; } = new List<FieldViewModel>();
}

public class FarmListViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
}