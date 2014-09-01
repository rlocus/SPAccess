namespace SP2013Access.Commands
{
    public class CommandEntity
    {
        public string Name { get; set; }

        public IDelegateCommand Command { get; set; }
    }
}