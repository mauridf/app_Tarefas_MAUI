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
        CarregarLocalizacoes();
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
            FotosFrame.IsVisible = true;
            FotosCollection.ItemsSource = fotos;
            return;
        }
        FotosFrame.IsVisible = false;
    }

    private async void CarregarLocalizacoes()
    {
        var localizacoes = await _anexoservico.Query().Where(a => a.TarefaId == Tarefa.Id && string.IsNullOrEmpty(a.Arquivo)).ToListAsync();
        if (localizacoes.Count > 0)
        {
            LocalizacaoFrame.IsVisible = true;
            LocalizacaoCollection.ItemsSource = localizacoes;
            return;
        }
        LocalizacaoFrame.IsVisible = false;
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

    private async void LabelLinkGoogleMaps_Tapped(object sender, EventArgs e)
    {
        var label = sender as Label;
        if (label != null)
        {
            var url = label.Text.Split('-')[1].Trim();
            if(!string.IsNullOrEmpty(url))
            {
                await Launcher.OpenAsync(new Uri(url));
            }
        }
    }

    private async void GPSClicked(object sender, EventArgs e)
    {
        var confirmado = await DisplayAlert("Localização", $"Confirma a captura de sua Localização?", "Localizar", "Cancelar");
        if (confirmado)
        {
            LocalizacaoBotao.Text = "Carregando ...";
            LocalizacaoBotao.IsEnabled = false;

            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if(status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    if(status != PermissionStatus.Granted)
                    {
                        await DisplayAlert("Permissão de Localização", "Permissão de acesso a Localização não é permitida.","Ok");
                        return;
                    }
                }

                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    await _anexoservico.IncluirAsync(new Anexo
                    {
                        Latitude = location.Latitude,
                        Longitude = location.Longitude,
                        TarefaId = Tarefa.Id,
                    });

                    CarregarLocalizacoes();
                    await DisplayAlert("Localização", $"Latitude: {location.Latitude} e Longitude: {location.Longitude}","Ok");
                }
            }
            catch (FeatureNotSupportedException fnsex)
            {
                await DisplayAlert("Erro", "GPS não suportado nesse dispositivo. - " + fnsex.Message, "Ok");
            }
            catch (FeatureNotEnabledException fnex)
            {
                await DisplayAlert("Erro", "GPS não está habilitado. - " + fnex.Message, "Ok");
            }
            catch (PermissionException pex)
            {
                await DisplayAlert("Erro", "Permissão de GPS negada. - " + pex.Message, "Ok");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", "Não foi possível obter a localização devido a um erro. - " + ex.Message, "OK");
            }
        }
    }
}