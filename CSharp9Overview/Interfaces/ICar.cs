namespace CSharp9Overview.Interfaces
{
    public interface ICar
    {
        void CheckAllSystems();
    }

    public interface IEngine : IComponent
    {
    }

    public interface IGearBox: IComponent
    {
    }

    public interface IOnBoardComputer : IComponent
    {
    }

    public interface IDriveTrain : IComponent
    {
    }

    public interface ITirePressureSensor : IComponent
    {
    }

    public interface IComponent
    {
        bool IsOperational();
    }
}
