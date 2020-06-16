using System.Threading.Tasks;

namespace Comsec.Aws.Commands
{
    public interface ICommand
    {
        Task Execute();
    }

    public interface ICommand<in T>
    {
        Task Execute(T input);
    }
}