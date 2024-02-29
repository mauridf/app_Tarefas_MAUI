using app_Tarefas.Models;
using app_Tarefas.Servicos;
using app_Tarefas.Constantes;

namespace app_Tarefas.Paginas;

public partial class TarefasDetalhePage : ContentPage
{
	public Tarefa Tarefa { get; set; }

    private DataBaseServico<Tarefa> _tarefaservico;

    private DataBaseServico<Comentario> _comentarioservico;
    public TarefasDetalhePage(Tarefa tarefa)
	{
		InitializeComponent();
		Tarefa = tarefa;
		BindingContext = this;
        _tarefaservico = new DataBaseServico<Tarefa>(Db.DB_PATH);
        _comentarioservico = new DataBaseServico<Comentario>(Db.DB_PATH);

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LabelTitulo.Text = Tarefa.Titulo;
        LabelNomeUsuario.Text = Tarefa.NomeUsuario;
        LabelStatus.Text = Tarefa.Status.ToString();
        LabelDescricao.Text = Tarefa.Descricao;
        LabelDataCriacao.Text = Tarefa.DataCriacao.ToString();
        LabelDataAtualizacao.Text = Tarefa.DataAtualizacao.ToString();
        UsuarioPicker.ItemsSource = UsuarioServico.Instancia().Todos();

        CarregarComentarios();
    }

    private async void AdicionarComentarioClicked(object sender, EventArgs e)
    {
        if(string.IsNullOrEmpty(NovoComentarioEditor.Text) || UsuarioPicker.SelectedItem == null)
        {
            await DisplayAlert("Erro", "Digite o comentário e Selecione o Usuário","Fechar");
            return;
        }

        var usuario = (Usuario)UsuarioPicker.SelectedItem;

        await _comentarioservico.IncluirAsync(new Comentario
        {
            UsuarioId = usuario.Id,
            TarefaId = Tarefa.Id,
            Texto = NovoComentarioEditor.Text
        });

        NovoComentarioEditor.Text = string.Empty;
        UsuarioPicker.SelectedItem = -1;

        CarregarComentarios();
    }

    private async void CarregarComentarios()
    {
        ComentariosCollection.ItemsSource = await _comentarioservico.Query().Where(c => c.TarefaId == Tarefa.Id).ToListAsync();
    }

    private async void VoltarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void IrParaAlteracaoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TarefasSalvarPage(Tarefa));
    }

    private async void IrParaExclusaoClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Confirmação", "Deseja Excluir essa Tarefa?", "Sim", "Não");
        if (confirm)
        {
            await _tarefaservico.DeleteAsync(Tarefa);
            await Navigation.PopAsync();
        }
    }
}