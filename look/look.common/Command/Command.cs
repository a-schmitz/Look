namespace look.common.Command
{
    using System.Windows.Forms;

    public static class Command
    {
        public static void Execute(CommandInfo cmd)
        {
            if (cmd.CommandType == CommandTypeOption.MouseMove)
            {
                MouseMove(cmd.Data);
            }
        }

        private static void MouseMove(string data)
        {
            var parts = data.Split(',');
            if (parts.Length != 2)
            {
                return;
            }
            var cursorX = int.Parse(parts[0]);
            var cursorY = int.Parse(parts[1]);

            Cursor.Position = new System.Drawing.Point(cursorX, cursorY);
        }
    }
}