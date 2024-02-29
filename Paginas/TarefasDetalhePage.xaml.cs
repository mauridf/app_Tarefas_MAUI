using app_Tarefas.Models;
using app_Tarefas.Servicos;
using app_Tarefas.Constantes;

namespace app_Tarefas.Paginas;

public partial class TarefasDetalhePage : ContentPage
{
	public Tarefa Tarefa { get; set; }

    private DataBaseServico<Tarefa> _tarefaservico;

    private DataBaseServico<Comentario> _comentarioservico;

    private DataBaseServico<Anexo> _anexoservico;
    public TarefasDetalhePage(Tarefa tarefa)
	{
		InitializeComponent();
		Tarefa = tarefa;
		BindingContext = this;
        _tarefaservico = new DataBaseServico<Tarefa>(Db.DB_PATH);
        _comentarioservico = new DataBaseServico<Comentario>(Db.DB_PATH);
        _anexoservico = new DataBaseServico<Anexo>(Db.DB_PATH);

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
        CarregarImagens();
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

    private async void CarregarImagens()
    {
        var fotos = await _anexoservico.Query().Where(a => a.TarefaId == Tarefa.Id && !string.IsNullOrEmpty(a.Arquivo)).ToListAsync();
        if(fotos.Count > 0)
        {
            FotosCollection.IsVisible = true;
            FotosCollection.ItemsSource = fotos;
            return;
        }
        FotosCollection.IsVisible = false;
    }

    private async void TirarFotoClicked(object sender, EventArgs e)
    {
        try
        {
            //Verifica se a câmera está disponível no dispositivo
            if (MediaPicker.Default.IsCaptureSupported)
            {
                //Tira uma foto e obtem o arquivo resultante
                FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

                if (photo != null)
                {  
                    //Cria um stream a partir do arquivo da foto
                    using Stream stream = await photo.OpenReadAsync();

                    //Define o Diretório e o nome aonde a foto será salva
                    string directory = FileSystem.AppDataDirectory;
                    string filename = Path.Combine(directory, $"{DateTime.Now.ToString("ddMMyyyy_hhmmss")}.jpg");

                    //Salva a foto no diretório definido
                    using FileStream fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write);
                    await stream.CopyToAsync(fileStream);

                    await _anexoservico.IncluirAsync(new Anexo
                    {
                        Arquivo = filename,
                        TarefaId = Tarefa.Id,
                    });

                    CarregarImagens();
                }
            }
            else
            {
                await DisplayAlert("Erro", "A Captura de Fotos não é suportada nesse dispositivo", "Fechar");
            }
        }
        catch (FeatureNotSupportedException fnsex)
        {
            await DisplayAlert("Erro", "A Captura de Fotos não é suportada nesse dispositivo. - " + fnsex.Message, "Ok");
        }
        catch(PermissionException pex)
        {
            await DisplayAlert("Erro", "Permissão para Acessar a Câmera não concesida. - " + pex.Message, "Ok");
        }
        catch(Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "Ok");
        }
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