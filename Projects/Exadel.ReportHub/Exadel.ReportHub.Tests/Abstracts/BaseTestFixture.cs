using AutoFixture;
using AutoMapper;
using Exadel.ReportHub.Host;
using Exadel.ReportHub.Host.Mapping.Profiles;
using Microsoft.Extensions.DependencyInjection;

namespace Exadel.ReportHub.Tests.Abstracts;

public abstract class BaseTestFixture
{
    protected static IMapper Mapper { get; }

    protected static Fixture Fixture { get; }

    static BaseTestFixture()
    {
        var services = new ServiceCollection();
        services.AddAutoMapper(typeof(Startup));
        var serviceProvider = services.BuildServiceProvider();
        Mapper = serviceProvider.GetRequiredService<IMapper>();

        Fixture = new Fixture();
    }
}
