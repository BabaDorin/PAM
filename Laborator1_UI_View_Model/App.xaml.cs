namespace Laborator1_UI_View_Model
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "Laborator1_UI_View_Model" };
        }
    }
}
