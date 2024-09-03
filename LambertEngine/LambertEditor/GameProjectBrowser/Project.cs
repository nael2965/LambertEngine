using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Input;
using LambertEditor.Common;
using LambertEditor.Utilities;

namespace LambertEditor.GameProjectBrowser;

[DataContract(Name = "Game")]
public class Project : ViewModelBase
{
    public static string Extension { get; } = ".lambert";
    [DataMember] public string Name { get; private set; } = "New Project";
    [DataMember] public string Path { get; private set; }
    public string FullPath => $@"{Path}\{Name}{Extension}";
    [DataMember(Name = "Scenes")] private ObservableCollection<Scene> _scenes = new();
    public ReadOnlyObservableCollection<Scene> Scenes { get; private set; }
    private Scene _activeScene;

    public Scene ActiveScene
    {
        get => _activeScene;
        set
        {
            if (_activeScene != value)
            {
                _activeScene = value;
                OnPropertyChanged(nameof(ActiveScene));
            }
        }
    }

    public static Project Current => Application.Current.MainWindow.DataContext as Project;
    public static UndoRedo UndoRedo { get; } = new UndoRedo();
    public ICommand UndoCommand { get; private set; }
    public ICommand RedoCommand { get; private set; }
    public ICommand AddSceneCommand { get; private set; }
    public ICommand RemoveSceneCommand { get; private set; }
    public ICommand SaveCommand { get; private set; }

    private void AddSceneInternal(string sceneName)
    {
        Debug.Assert(!string.IsNullOrEmpty(sceneName.Trim()));
        _scenes.Add(new Scene(this, sceneName));
    }

    private void RemoveSceneInternal(Scene scene)
    {
        Debug.Assert(_scenes.Contains(scene));
        _scenes.Remove(scene);
    }

    public static Project Load(string file)
    {
        Debug.WriteLine($"프로젝트 파일 로딩 시도: {file}");

        if (string.IsNullOrEmpty(file))
        {
            Debug.WriteLine("프로젝트 파일 경로가 null 또는 빈 문자열입니다.");
            throw new ArgumentException("프로젝트 파일 경로가 유효하지 않습니다.", nameof(file));
        }

        if (!File.Exists(file))
        {
            Debug.WriteLine($"프로젝트 파일을 찾을 수 없음: {file}");
            throw new FileNotFoundException($"프로젝트 파일을 찾을 수 없습니다: {file}");
        }

        try
        {
            var project = Serializer.FromFile<Project>(file);
        
            if (project == null)
            {
                Debug.WriteLine("역직렬화된 프로젝트가 null입니다.");
                throw new InvalidDataException("프로젝트 데이터를 로드할 수 없습니다.");
            }

            // Path와 Name 설정
            string directoryPath = System.IO.Path.GetDirectoryName(file);
            string projectName = System.IO.Path.GetFileNameWithoutExtension(file);

            if (string.IsNullOrEmpty(directoryPath) || string.IsNullOrEmpty(projectName))
            {
                Debug.WriteLine("프로젝트 이름 또는 경로를 추출할 수 없습니다.");
                throw new InvalidDataException("프로젝트 이름 또는 경로를 추출할 수 없습니다.");
            }

            // 프로젝트 객체 속성 설정
            project.SetProjectDetails(projectName, directoryPath);

            Debug.WriteLine($"프로젝트 로딩 성공: {project.Name}");
            return project;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"프로젝트 로딩 중 예외 발생: {ex.Message}");
            Debug.WriteLine($"스택 트레이스: {ex.StackTrace}");
            throw;
        }
    }

    // 프로젝트 세부 정보를 설정하는 새로운 메서드
    public void SetProjectDetails(string name, string path)
    {
        Name = name;
        Path = path;
    }

    public void Unload() { }
    public static void Save(Project project)
    {
        Serializer.ToFile(project, project.FullPath);
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        if (_scenes != null)
        {
            Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
            OnPropertyChanged(nameof(Scenes));
        }

        ActiveScene = Scenes.FirstOrDefault(x => x.IsActive);
        AddSceneCommand = new RelayCommand<object>(x =>
        {
            AddSceneInternal($"New Scene {_scenes.Count}");
            var newScene = _scenes.Last();
            var sceneIndex = _scenes.Count - 1;
            UndoRedo.Add(new UndoRedoAction(
                () => RemoveSceneInternal(newScene),
                () => { _scenes.Insert(sceneIndex, newScene); },
                $"Add {newScene.Name}"
            ));
        });
        RemoveSceneCommand = new RelayCommand<Scene>(x =>
        {
            var sceneIndex = _scenes.IndexOf(x);
            RemoveSceneInternal(x);

            UndoRedo.Add(new UndoRedoAction(
                () => _scenes.Insert(sceneIndex, x),
                () => RemoveSceneInternal(x),
                $"Remove {x.Name}"));
        }, x => x != null && !x.IsActive);
        
        UndoCommand = new RelayCommand<object>(x => UndoRedo.Undo());
        RedoCommand = new RelayCommand<object>(x => UndoRedo.Redo());
        SaveCommand = new RelayCommand<object>(x => Save(this));
    }

    public Project(string name, string path)
    {
        Name = name;
        Path = path;

        OnDeserialized(new StreamingContext());
    }
}