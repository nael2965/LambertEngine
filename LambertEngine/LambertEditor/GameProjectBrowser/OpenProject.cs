using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using LambertEditor.Common;
using LambertEditor.Utilities;

namespace LambertEditor.GameProjectBrowser;

[DataContract]
public class ProjectData
{
    [DataMember]
    public string ProjectName { get; set; }
    [DataMember]
    public string ProjectPath { get; set; }
    [DataMember]
    public DateTime Date { get; set; }
    public string FullPath => Path.Combine(ProjectPath, $"{ProjectName}{Project.Extension}");
    public byte[] Icon { get; set; }
    public byte[] Screenshot { get; set; }
}

[DataContract]
public class ProjectDataList
{
    [DataMember]
    public List<ProjectData> Projects { get; set; }
}

public class OpenProject : ViewModelBase
{
    private static readonly string _applicationDataPath =
        $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\LambertEditor\";
    private static readonly string _projectDataPath;
    private static readonly ObservableCollection<ProjectData> _projects = new ObservableCollection<ProjectData>();
    public static ReadOnlyObservableCollection<ProjectData> Projects { get; }

    public static void ReadProjectData()
    {
        Debug.WriteLine($"프로젝트 데이터 파일 경로: {_projectDataPath}");
        if (File.Exists(_projectDataPath))
        {
            Debug.WriteLine("프로젝트 데이터 파일 존재함");
            try
            {
                var content = File.ReadAllText(_projectDataPath);
                Debug.WriteLine($"파일 내용:\n{content}");

                var projectDataList = Serializer.FromFile<ProjectDataList>(_projectDataPath);
                if (projectDataList != null && projectDataList.Projects != null)
                {
                    Debug.WriteLine($"읽어온 프로젝트 수: {projectDataList.Projects.Count}");
                    _projects.Clear();
                    foreach (var project in projectDataList.Projects)
                    {
                        if (File.Exists(project.FullPath))
                        {
                            _projects.Add(project);
                            Debug.WriteLine($"프로젝트 추가됨: {project.ProjectName}, 경로: {project.FullPath}");
                        }
                        else
                        {
                            Debug.WriteLine($"프로젝트 파일 없음: {project.FullPath}");
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("프로젝트 데이터 리스트가 null임");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"프로젝트 데이터 읽기 오류: {ex.Message}");
                Debug.WriteLine($"스택 트레이스: {ex.StackTrace}");
            }
        }
        else
        {
            Debug.WriteLine("프로젝트 데이터 파일이 존재하지 않음");
        }
        Debug.WriteLine($"최종 로드된 프로젝트 수: {_projects.Count}");
    }

    public static void WriteProjectData()
    {
        Debug.WriteLine("프로젝트 데이터 쓰기 시작 (Starting to write project data)");
        try
        {
            var projects = _projects.OrderBy(x => x.Date).ToList();
            Serializer.ToFile(new ProjectDataList() { Projects = projects }, _projectDataPath);
            Debug.WriteLine($"{projects.Count}개의 프로젝트 저장됨 ({projects.Count} projects saved)");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"프로젝트 데이터 쓰기 오류: {ex.Message} (Error writing project data: {ex.Message})");
        }
    }

    public static Project Open(ProjectData data)
    {
        Debug.WriteLine($"프로젝트 열기 시도: {data.ProjectName} (Attempting to open project: {data.ProjectName})");
        ReadProjectData();
        var project = _projects.FirstOrDefault(x => x.FullPath == data.FullPath);
        if (project == null)
        {
            project = data;
            try
            {
                project.Icon = File.ReadAllBytes($@"{project.ProjectPath}\.Lambert\Icon.png");
                project.Screenshot = File.ReadAllBytes($@"{project.ProjectPath}\.Lambert\Screenshot.png");
                Debug.WriteLine("프로젝트 아이콘과 스크린샷 로드됨 (Project icon and screenshot loaded)");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"프로젝트 아이콘 또는 스크린샷 로드 오류: {ex.Message} (Error loading project icon or screenshot: {ex.Message})");
            }
            _projects.Add(project);
            Debug.WriteLine("새 프로젝트가 목록에 추가됨 (New project added to the list)");
        }
        project.Date = DateTime.Now;
        WriteProjectData();
        Debug.WriteLine($"프로젝트 열기 완료: {project.ProjectName} (Project opened: {project.ProjectName})");
        return new Project(project.ProjectName, project.ProjectPath);
    }
    
    static OpenProject()
    {
        try
        {
            if (!Directory.Exists(_applicationDataPath)) Directory.CreateDirectory(_applicationDataPath);
            _projectDataPath = $@"{_applicationDataPath}ProjectData.xml";
            Projects = new ReadOnlyObservableCollection<ProjectData>(_projects);
            ReadProjectData();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            // throw;
        }
        
    }
}