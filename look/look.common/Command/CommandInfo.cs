namespace look.common.Command
{

    using System;

    public class CommandInfo
    {
        public CommandTypeOption CommandType { get; set; }

        public string Data { get; set; }

        public CommandInfo(CommandTypeOption type, string data)
        {
            this.CommandType = type;
            this.Data = data;
        }

        public override string ToString()
        {
            return this.CommandType + "|" + this.Data;
        }

        public static CommandInfo Parse(string input)
        {
            var parts = input.Split('|');
            if (parts.Length != 2)
            {
                return null;
            }

            var type = (CommandTypeOption)Enum.Parse(typeof(CommandTypeOption), parts[0]);
            var data = parts[1];
            return new CommandInfo(type, data);
        }
    }
}
