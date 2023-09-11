using System.Globalization;
using CsvHelper;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.Models;

namespace Hurudza.Data.Context.Seed;

public static class SeedAdministrativeStructureData
{
    public static async Task SeedAdministrativeStructure(HurudzaDbContext context)
    {
        var provinces = context.Provinces.ToHashSet();
        var districts = context.Districts.ToHashSet();
        var localAuthorities = context.LocalAuthorities.ToHashSet();
        var wards = context.Wards.ToHashSet();

        var allFiles = Directory.GetFiles("/Users/provie17/Documents/BioData", "*.csv", SearchOption.AllDirectories);

        var bioData = new List<BioData>();

        foreach (var file in allFiles)
        {
            var fileInfo = new FileInfo(file);
            
            using (var reader = new StreamReader(fileInfo.Open(FileMode.Open)))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                bioData = csv.GetRecords<BioData>().ToList();
            }

            var uniqueRows = bioData.DistinctBy(v => v.StationCode).ToList();
            foreach (var bioRecord in uniqueRows)
            {
                var trimedProvince = bioRecord.Province.Trim();
                var province = provinces.FirstOrDefault(c => c.Name == trimedProvince);
                if (province == null)
                {
                    province = new Province
                    {
                        Name = trimedProvince
                    };

                    provinces.Add(province);
                    await context.AddAsync(province).ConfigureAwait(false);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }

                var trimmedDistrict = bioRecord.District.Trim();
                var district =
                    districts.FirstOrDefault(
                        c => c.Name == trimmedDistrict && c.Province.Name == trimedProvince);
                if (district == null)
                {
                    district = new District
                    {
                        Name = trimmedDistrict,
                        ProvinceId = province.Id
                    };

                    districts.Add(district);
                    await context.AddAsync(district).ConfigureAwait(false);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }

                var trimmedAuthority = bioRecord.LocalAuthority.Trim();
                var localAuthority = localAuthorities.FirstOrDefault(c =>
                    c.Name == trimmedAuthority && c.District.Name == trimmedDistrict);
                if (localAuthority == null)
                {
                    localAuthority = new LocalAuthority
                    {
                        Name = trimmedAuthority,
                        DistrictId = district.Id
                    };

                    localAuthorities.Add(localAuthority);
                    await context.AddAsync(localAuthority).ConfigureAwait(false);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }

                var trimmedWard = bioRecord.Ward.Trim();
                var ward = wards.FirstOrDefault(c =>
                    c.Name == trimmedWard && c.District.Name == trimmedDistrict && c.LocalAuthority.Name == trimmedAuthority);
                if (ward == null)
                {
                    ward = new Ward
                    {
                        Name = trimmedWard,
                        DistrictId = district.Id,
                        LocalAuthorityId = localAuthority.Id,
                        ProvinceId = province.Id
                    };

                    wards.Add(ward);
                    await context.AddAsync(ward).ConfigureAwait(false);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }
    }
}