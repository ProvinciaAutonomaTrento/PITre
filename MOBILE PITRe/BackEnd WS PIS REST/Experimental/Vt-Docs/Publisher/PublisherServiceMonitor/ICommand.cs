namespace PublisherServiceMonitor
{
    public interface ICommand
    {
        void Execute(CommandArgs args);
    }
}
