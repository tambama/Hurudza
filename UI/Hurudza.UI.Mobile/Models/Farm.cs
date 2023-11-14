using Hurudza.Data.Enums.Enums;
using Microsoft.Datasync.Client;

namespace Hurudza.UI.Mobile.Models;

public class Farm : DatasyncClientData, IEquatable<Farm>
{
    public required string Name { get; set; }
    public required string Address { get; set; }
    public string Description { get; set; }
    public string Vision { get; set; }
    public string Mission { get; set; }
    public required string ContactPerson { get; set; }
    public required string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Website { get; set; }
    public int Year { get; set; }
    public string FoundingMembers { get; set; }
    public string KeyMilestones { get; set; }
    public double Size { get; set; }
    public double Arable { get; set; }
    public double Cleared { get; set; }
    public Terrain? Terrain { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Elevation { get; set; }
    public string PastCrops { get; set; }
    public string LandManagementPractice { get; set; }
    public string SecurityMeasures { get; set; }
    public string WaterSource { get; set; }
    public WaterAvailability? WaterAvailability { get; set; }
    public bool Irrigated { get; set; }
    public string Equipment { get; set; }
    public string SoilType { get; set; }
    public string Buildings { get; set; }
    public RoadAccess? RoadAccess { get; set; }
    public string Problems { get; set; }
    public string Personnel { get; set; }
    public string Recommendations { get; set; }
    public string WardId { get; set; }
    public string LocalAuthorityId { get; set; }
    public string DistrictId { get; set; }
    public string ProvinceId { get; set; }
    public Hurudza.Data.Enums.Enums.Region? Region { get; set; }
    public Conference? Conference { get; set; }
    public string CommunityPrograms { get; set; }
    public string Partnerships { get; set; }
    public string Classrooms { get; set; }
    public string Laboratories { get; set; }
    public string SportsFacilities { get; set; }
    public string Library { get; set; }
    public string AverageExamScores { get; set; }
    public string ExtraCurricularAchievements { get; set; }
    public int MaleTeachers { get; set; }
    public int FemaleTeachers { get; set; }
    public int MaleStudents { get; set; }
    public int FemaleStudents { get; set; }
    public string StudentDemographics { get; set; }
    public int DayScholars { get; set; }
    public int BoardingScholars { get; set; }
    public string NonTeachingStaff { get; set; }

    public bool Equals(Farm other) =>
        other != null && other.Id == Id &&
        other.Name == Name &&
        other.PhoneNumber == PhoneNumber &&
        other.Email == Email &&
        other.WardId == WardId &&
        other.LocalAuthorityId == LocalAuthorityId &&
        other.DistrictId == DistrictId &&
        other.ProvinceId == ProvinceId &&
        other.Region == Region &&
        other.Conference == Conference;
}