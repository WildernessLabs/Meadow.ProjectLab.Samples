using Meadow.Cloud;

namespace Meadow.Cloud_Command.Commands
{
    public class ToggleRelayCommand : IMeadowCommand
    {
        public int Relay { get; set; }

        public bool IsOn { get; set; }
    }
}