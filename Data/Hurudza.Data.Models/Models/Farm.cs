using System.ComponentModel.DataAnnotations;
using Hurudza.Data.Enums.Enums;
using Hurudza.Data.Models.Base;

namespace Hurudza.Data.Models.Models;

public class Farm : BaseEntity
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Address { get; set; }
    public string? Description { get; set; }
    public string? Vision { get; set; }
    public string? Mission { get; set; }
    [Required]
    public string ContactPerson { get; set; }
    [Required]
    public string PhoneNumber { get; set; }
    [Required]
    public string Email { get; set; }
    public string? Website { get; set; }
    public int Year { get; set; }
    public string? FoundingMembers { get; set; }
    public string? KeyMilestones { get; set; }
    public float Size { get; set; }
    public float Arable { get; set; }
    public float Cleared { get; set; }
    public string? PastCrops { get; set; }
    public string? LandManagementPractice { get; set; }
    public string? SecurityMeasures { get; set; }
    public string? WaterSource { get; set; }
    public WaterAvailability WaterAvailability { get; set; }
    public bool Irrigated { get; set; }
    public string? Equipment { get; set; }
    public string? SoilType { get; set; }
    public RoadAccess RoadAccess { get; set; }
    public string? Problems { get; set; }
    public string? Personnel { get; set; }
    public string? Recommendations { get; set; }
    public string? WardId { get; set; }
    public string? LocalAuthorityId { get; set; }
    public string? DistrictId { get; set; }
    public string? ProvinceId { get; set; }
    public Region Region { get; set; }
    public Conference Conference { get; set; }
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
    public Terrain Terrain { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Elevation { get; set; }
    public string? Buildings { get; set; }

    public virtual Ward? Ward { get; set; }
    public virtual LocalAuthority? LocalAuthority { get; set; }
    public virtual District? District { get; set; }
    public virtual Province? Province { get; set; }
    public virtual ICollection<Field> Fields { get; set; }
    public virtual ICollection<FarmLocation> Locations { get; set; }
    public virtual ICollection<FieldLocation> FieldLocations { get; set; }
    
    public virtual ICollection<FarmOwner> Owners { get; set; }
}