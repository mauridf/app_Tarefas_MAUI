namespace app_Tarefas
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell(); // Muda toda a página

            //MainPage = new NavigationPage(new AppShell()); //Muda somente o frame e mantem o menu
        }
    }
}