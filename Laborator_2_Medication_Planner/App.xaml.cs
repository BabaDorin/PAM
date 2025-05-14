namespace Laborator_2_Medication_Planner
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "Laborator_2_Medication_Planner" };
        }
    }
}
