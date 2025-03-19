namespace Nemeio.Core.Injection
{
    public interface IDependencyRegister
    {
        TService Resolve<TService>() where TService : class;
        void ConstructAndRegisterSingleton<TInterface, TType>() where TInterface : class where TType : TInterface;
        void LazyConstructAndRegisterSingleton<TInterface, TType>() where TInterface : class where TType : TInterface;
        void RegisterSingleton<TInterface>(TInterface service) where TInterface : class;
        void RegisterType<TInterface, TType>() where TInterface : class where TType : class, TInterface;
    }
}
