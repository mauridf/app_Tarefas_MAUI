using app_Tarefas.Enums;
using app_Tarefas.Models;
using app_Tarefas.Servicos;
using app_Tarefas.Constantes;

namespace app_Tarefas.Paginas;

public partial class TarefasSalvarPage : ContentPage
{
    public Tarefa Tarefa { get; set; }

    DataBaseServico<Tarefa> _tarefaServico;

    public TarefasSalvarPage(Tarefa tarefa = null)
	{
        InitializeComponent();

        _tarefaServico = new DataBaseServico<Tarefa>(Db.DB_PATH);

        Tarefa = tarefa ?? new Tarefa();
        BindingContext = tarefa;

        StatusPicker.ItemsSource = Enum.GetValues(typeof(Status)).Cast<Status>().ToList();
        UsuarioPicker.ItemsSource = UsuarioServico.Instancia().Todos();

        StatusPicker.SelectedItem = Tarefa.Status ?? Status.Backlog;
        UsuarioPicker.SelectedItem = Tarefa.Usuario;
    }

    private async void VoltarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(TituloEntry.Text))
        {
            await DisplayAlert("Erro", "O Título é Obrigatório", "OK");
            TituloEntry.Focus();
            return;
        }

        Tarefa.Titulo = TituloEntry.Text;
        Tarefa.Descricao = DescricaoEditor.Text;
        if (StatusPicker.SelectedItem != null)
            Tarefa.Status = (Status)StatusPicker.SelectedItem;
        else
            Tarefa.Status = Status.Backlog;

        if(UsuarioPicker.SelectedItem != null)
            Tarefa.UsuarioId = ((Usuario)UsuarioPicker.SelectedItem).Id;

        if(Tarefa.Id == 0)
            await _tarefaServico.IncluirAsync(Tarefa);
        else
            Tarefa.DataAtualizacao = DateTime.Now;
            await _tarefaServico.AlterarAsync(Tarefa);

        await Navigation.PopAsync();
    }
}