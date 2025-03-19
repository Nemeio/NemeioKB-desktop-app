using MvvmCross.Platform;
using Nemeio.Core.Injection;

namespace Nemeio.Presentation.Injection
{
    public class MvxDependencyRegister : IDependencyRegister
    {
        public void ConstructAndRegisterSingleton<TInterface, TType>()
            where TInterface : class
            where TType : TInterface
        {
            Mvx.ConstructAndRegisterSingleton<TInterface, TType>();
        }

        public void LazyConstructAndRegisterSingleton<TInterface, TType>()
            where TInterface : class
            where TType : TInterface
        {
            Mvx.LazyConstructAndRegisterSingleton<TInterface, TType>();
        }

        public void RegisterSingleton<TInterface>(TInterface service) where TInterface : class
        {
            Mvx.RegisterSingleton<TInterface>(service);
        }

        public TService Resolve<TService>() where TService : class
        {
            return Mvx.Resolve<TService>();
        }

        public void RegisterType<TInterface, TType>() where TInterface : class where TType : class, TInterface
        {
            Mvx.RegisterType<TInterface, TType>();
        }
    }
}
