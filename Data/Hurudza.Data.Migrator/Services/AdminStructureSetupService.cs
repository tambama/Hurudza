using CsvHelper;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Migrator.Models;
using Hurudza.Data.Models.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;

namespace Hurudza.Data.Migrator.Services
{
    public class AdminStructureSetupService
    {
        protected HurudzaDbContext context;
        public string ROOT_PATH;

        public AdminStructureSetupService(HurudzaDbContext context)
        {
            this.context = context;
            ROOT_PATH = OperatingSystem.IsMacOS() ? "/Users/peniel/Code/Party/Roll" : "D:\\Documents\\Musangano\\Voters";
        }

        public async Task ImportBulkVoters()
        {
            var party_path = !OperatingSystem.IsWindows()
                ? $"{ROOT_PATH}".Replace("\\", "/")
                : $"{ROOT_PATH}";

            var allFiles = Directory.GetFiles(party_path, "*.csv", SearchOption.AllDirectories);

            foreach (var file in allFiles)
            {
                var stopWatch = Stopwatch.StartNew();
                stopWatch.Start();
                Console.WriteLine($"File: {file}");
                try
                {
                    List<Zec> voterRows = new();

                    await using var stream = new MemoryStream(File.ReadAllBytes(file));
                    stream.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(stream))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        voterRows = csv.GetRecords<Zec>().ToList();
                    }

                    var uniqueRows = voterRows.DistinctBy(v => v.StationCode).ToList();

                    var _provinceSet = context.Provinces.ToHashSet();
                    var _adminDistrictSet = context.Districts.ToHashSet();
                    var _localAuthoritiesSet = context.LocalAuthorities.ToHashSet();
                    var _wardSet = context.Wards.ToHashSet();
                    var currentProvince = uniqueRows[0].Province.Trim();
                    var provinceId = (await context.Provinces.FirstOrDefaultAsync(p => p.Name == currentProvince).ConfigureAwait(false))?.Id ?? string.Empty;

                    foreach (var voterRow in uniqueRows)
                    {
                        var trimmedProvince = voterRow.Province.Trim();
                        var province = _provinceSet.FirstOrDefault(c => c.Name == trimmedProvince);
                        if (province == null)
                        {
                            province = new Province
                            {
                                Name = trimmedProvince
                            };

                            await context.Provinces.AddAsync(province);
                            await context.SaveChangesAsync();

                            _provinceSet.Add(province);
                        }

                        var trimmedDistrict = voterRow.District.Trim();
                        var district = _adminDistrictSet.FirstOrDefault(c => c.Name == trimmedDistrict && c.Province.Name == trimmedProvince);
                        if (district == null)
                        {
                            district = new District
                            {
                                Name = trimmedDistrict,
                                ProvinceId = province.Id
                            };

                            await context.Districts.AddAsync(district);
                            await context.SaveChangesAsync();

                            _adminDistrictSet.Add(district);
                        }

                        var trimmedAuthority = voterRow.LocalAuthority.Trim();
                        var localAuthority = _localAuthoritiesSet.FirstOrDefault(c => c.Name == trimmedAuthority && c.District.Name == trimmedDistrict);
                        if (localAuthority == null)
                        {
                            localAuthority = new LocalAuthority
                            {
                                Name = trimmedAuthority,
                                DistrictId = district.Id
                            };

                            await context.LocalAuthorities.AddAsync(localAuthority);
                            await context.SaveChangesAsync();

                            _localAuthoritiesSet.Add(localAuthority);
                        }

                        var trimmedWard = voterRow.Ward.Trim();
                        var ward = _wardSet.FirstOrDefault(c => c.Name == trimmedWard && c.District.Name == trimmedDistrict);
                        if (ward == null)
                        {
                            ward = new Ward
                            {
                                Name = trimmedWard,
                                ProvinceId = province.Id,
                                DistrictId = district.Id,
                                LocalAuthorityId = localAuthority.Id
                            };

                            await context.Wards.AddAsync(ward);
                            await context.SaveChangesAsync();

                            _wardSet.Add(ward);
                        }
                    }

                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }
    }
}
