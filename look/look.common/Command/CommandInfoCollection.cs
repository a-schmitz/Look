namespace look.common.Command
{

    using System.Collections.Concurrent;
    using System.Text;
    using System.Threading;

    public class CommandInfoCollection
    {
        private ConcurrentQueue<CommandInfo> commands = new ConcurrentQueue<CommandInfo>();

        public string SerializeCommandStack()
        {
            var bld = new StringBuilder();

            foreach (var cmd in this.commands)
            {
                bld.Append(cmd);
                bld.Append(";");
            }

            // clear queue
            var newQueue = new ConcurrentQueue<CommandInfo>();
            Interlocked.Exchange(ref this.commands, newQueue);

            return bld.ToString();
        }

        public void DeserializeCommandStack(string input)
        {

            var parts = input.Split(';');
            foreach (var part in parts)
            {
                if (string.IsNullOrWhiteSpace(part))
                    continue;

                var cmd = CommandInfo.Parse(part.Trim());
                if (cmd != null)
                {
                    this.commands.Enqueue(cmd);
                }
            }

        }

        public void Add(CommandTypeOption type, string data)
        {
            var cmd = new CommandInfo(type, data);
            this.Add(cmd);
        }

        public void Add(CommandInfo cmd)
        {
            this.commands.Enqueue(cmd);           
        }

        public CommandInfo GetNextCommand()
        {
            CommandInfo cmd = null;
            if (!this.commands.IsEmpty)
            {
                this.commands.TryDequeue(out cmd);
            }

            return cmd;
        }
    }
}