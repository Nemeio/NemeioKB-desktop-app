using System.Linq;
using Microsoft.Extensions.Configuration;
using Nemeio.UpdateInquiry.Dto.Out;
using Nemeio.UpdateInquiry.Models;
using Nemeio.UpdateInquiry.Parser;
using Nemeio.UpdateInquiry.Repositories;

namespace Nemeio.UpdateInquiry.Builders
{
    public class BinaryListBuilder
    {
        private UpdateEnvironment _environment;
        private Component _component;
        private Platform _platform;
        private IUpdateRepository _repository;

        public BinaryListBuilder(IConfigurationRoot configurationRoot)
        {
            _environment = UpdateEnvironment.Master;
            _component = Component.WindowsInstaller;
            _platform = Platform.x64;
            _repository = new BlobStorageUpdateRepository(configurationRoot);
        }

        public BinaryListBuilder SetEnvironment(string environment)
        {
            var parser = new EnvironmentParser();
            var parsedValue = parser.Parse(environment);

            return SetEnvironment(parsedValue);
        }

        public BinaryListBuilder SetEnvironment(UpdateEnvironment environment)
        {
            _environment = environment;

            return this;
        }

        public BinaryListBuilder SetComponent(string component)
        {
            var parser = new ComponentParser();
            var parsedCompoment = parser.Parse(component);

            return SetComponent(parsedCompoment);
        }

        public BinaryListBuilder SetComponent(Component component)
        {
            _component = component;

            return this;
        }

        public BinaryListBuilder SetPlatform(string platform)
        {
            var parser = new PlatformParser();
            var parsedValue = parser.Parse(platform);

            return SetPlatform(parsedValue);
        }

        public BinaryListBuilder SetPlatform(Platform platform)
        {
            _platform = platform;

            return this;
        }

        public BinaryListOutDto Build()
        {
            var container = _repository.GetContainerByEnvironment(_environment);
            var binaries = _component == Component.WindowsInstaller ? container.Win : container.Osx;
            var binariesForPLatform = binaries.Where(x => x.Platform == _platform).ToList();

            var outDto = new BinaryListOutDto()
            {
                Softwares = binariesForPLatform
                                .Select(binary => SoftwareOutDto.FromBinary(binary))
                                .ToList()
            };

            return outDto;
        }
    }
}
