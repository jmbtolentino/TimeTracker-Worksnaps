namespace TimeTracker_Worksnaps
{
    internal static class Program
    {
        private static GlobalKeyboardHook _globalKeyboardHook = new GlobalKeyboardHook();
        private static Form1 _form;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            _globalKeyboardHook.KeyUp += GlobalKeyboardHook_KeyUp;
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(_form = new Form1());
            _globalKeyboardHook.Dispose();
        }

        static void GlobalKeyboardHook_KeyUp(object? sender, KeyEventArgs e)
        {
            if (_form == null) return;

            if (e.KeyCode == Keys.T && _globalKeyboardHook.IsCtrlPressed && _globalKeyboardHook.IsAltPressed)
            {
                if (_globalKeyboardHook.IsShiftPressed)
                {
                    _form.Close();
                }
                else
                {
                    if (_form.Opacity > 0)
                    {
                        _form.Opacity = 0;
                    }
                    else
                    {
                        _form.Opacity = 100;
                    }
                }
            }
        }
    }
}