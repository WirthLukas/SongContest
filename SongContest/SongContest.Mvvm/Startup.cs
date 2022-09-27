using Autofac;
using SongContest.Mvvm.Persistence;
using SongContest.Mvvm.ViewModels;
using SongContest.Mvvm.Views;

namespace SongContest.Mvvm
{
    internal static class Startup
    {
        internal static IContainer ConfigureServices()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<MainWindow>();
            builder.RegisterType<MainWindowViewModel>();

            builder.RegisterType<EditVotesWindow>();
            builder.RegisterType<EditVotesWindowViewModel>();

            builder.RegisterType<MvvmUnitOfWork>().As<IMvvmUnitOfWork>();

            return builder.Build();
        }
    }
}
