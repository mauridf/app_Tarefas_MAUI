using app_Tarefas.Constantes;
using app_Tarefas.Models;
using app_Tarefas.Servicos;
using app_Tarefas.Enums;
using System.Windows.Input;

namespace app_Tarefas.Paginas
{
    public partial class MainPage : ContentPage
    {
        DataBaseServico<Tarefa> _tarefaServico;

        public ICommand NavigateToDetailCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand ChangeCommand { get; private set; }
        public MainPage()
        {
            InitializeComponent();

            _tarefaServico = new DataBaseServico<Tarefa>(Db.DB_PATH);

            NavigateToDetailCommand = new Command<Tarefa>(async (tarefa) => await NavigateToDetail(tarefa));
            DeleteCommand = new Command<Tarefa>(ExecuteDeleteCommand);
            ChangeCommand = new Command<Tarefa>(async (tarefa) => await NavigateToChange(tarefa));
            //CardBacklog.BindingContext = this;

            CarregarTarefas();
        }

        private async Task NavigateToChange(Tarefa tarefa)
        {
            await Navigation.PushAsync(new TarefasSalvarPage(tarefa));
        }

        private async void ExecuteDeleteCommand(Tarefa tarefa)
        {
            bool confirm = await DisplayAlert("Confirmação", "Deseja Excluir essa Tarefa?", "Sim", "Não");
            if (confirm)
            {
                await _tarefaServico.DeleteAsync(tarefa);
                CarregarTarefas();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CarregarTarefas();
        }

        private async Task NavigateToDetail(Tarefa tarefa)
        {
            await Navigation.PushAsync(new TarefasDetalhePage(tarefa));
        }

        private async void CarregarTarefas()
        {
            CardBacklog.ItemsSource = await _tarefaServico.Query().Where(t => t.Status == Status.Backlog).ToArrayAsync();
            CardAnalise.ItemsSource = await _tarefaServico.Query().Where(t => t.Status == Status.Analise).ToArrayAsync();
            CardParaFazer.ItemsSource = await _tarefaServico.Query().Where(t => t.Status == Status.ParaFazer).ToArrayAsync();
            CardDesenvolvimento.ItemsSource = await _tarefaServico.Query().Where(t => t.Status == Status.Desenvolvimento).ToArrayAsync();
            CardFeito.ItemsSource = await _tarefaServico.Query().Where(t => t.Status == Status.Feito).ToArrayAsync();
        }

        private async void NovoClicked(object sender, EventArgs e)
        {
            var botao = sender as Button;
            if(sender != null)
            {
                var status = (Enums.Status)botao.CommandParameter;
                await Navigation.PushAsync(new TarefasSalvarPage(new Tarefa { Status = status }));
            }
        }
    }
}