using CSharp9Overview.Interfaces;
using static System.Console;

namespace CSharp9Overview.Implementations
{
    public class Car: ICar
    {
        public Car(IEngine engine, IGearBox gearBox, IOnBoardComputer computer, IDriveTrain driveTrain)
        {
            Engine = engine;
            GearBox = gearBox;
            Computer = computer;
            DriveTrain = driveTrain;
        }

        public IEngine Engine { get; }
        public IGearBox GearBox { get; }
        public IOnBoardComputer Computer { get; }
        public IDriveTrain DriveTrain { get; }

        public void CheckAllSystems()
        {
            WriteLine("======Car system check======");
            WriteLine($"Engine: {Engine.IsOperational()}");
            WriteLine($"Gear box: {GearBox.IsOperational()}");
            WriteLine($"Computer: {Computer.IsOperational()}");
            WriteLine($"Drive train: {DriveTrain.IsOperational()}");
            WriteLine("============================");
        }
    }

    public class Engine : IEngine
    {
        public bool IsOperational() => true;
    }

    public class GearBox : IGearBox
    {
        public bool IsOperational() => true;
    }

    public class Computer : IOnBoardComputer
    {
        public bool IsOperational() => true;
    }

    public class DriveTrain : IDriveTrain
    {
        public DriveTrain(ITirePressureSensor tirePressureSensor)
        {
            TirePressureSensor = tirePressureSensor;
        }

        public ITirePressureSensor TirePressureSensor { get; }

        public bool IsOperational() => TirePressureSensor.IsOperational();
    }

    public class TirePressureSensor : ITirePressureSensor
    {
        public bool IsOperational() => true;
    }
}
