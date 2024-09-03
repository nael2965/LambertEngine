using LambertEditor.Common;
using LambertEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace LambertEditor.GameProjectBrowser;

[DataContract]
public class ProjectTemplate
{
    // 소멸자: 디버그 목적으로 객체 소멸 추적
    // Destructor: Tracking object destruction for debugging purposes
    ~ProjectTemplate()
    {
        Debug.WriteLine("프로젝트 템플릿 삭제됨 (Project template deleted)");
    }

    [DataMember] public string ProjectType { get; set; }

    [DataMember] public string ProjectFile { get; set; }

    [DataMember] public List<string> Folders { get; set; } = new();

    public byte[] Icon { get; set; }
    public byte[] Screenshot { get; set; }
    public string IconFilePath { get; set; }
    public string ScreenshotFilePath { get; set; }
    public string ProjectFilePath { get; set; }
}

internal class NewProject : ViewModelBase
{
    // TODO: 엔진 설치 경로를 설정 파일에서 읽어오도록 변경
    // TODO: Change to read engine installation path from a config file
    private readonly string _templatePath = @"..\..\LambertEditor\ProjectTemplates\";

    private string _projectName = "NewProject";

    public string ProjectName
    {
        get => _projectName;
        set
        {
            if (_projectName != value)
            {
                _projectName = value;
                OnPropertyChanged(nameof(ProjectName));
                ValidateProjectPath();
                Debug.WriteLine($"프로젝트 이름 변경: {value} (Project name changed: {value})");
            }
        }
    }

    private string _projectPath =
        $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\LambertProject\";

    public string ProjectPath
    {
        get => _projectPath;
        set
        {
            if (_projectPath != value)
            {
                _projectPath = value;
                OnPropertyChanged(nameof(ProjectPath));
                ValidateProjectPath();
                Debug.WriteLine($"프로젝트 경로 변경: {value} (Project path changed: {value})");
            }
        }
    }

    private bool _isValid;

    public bool IsValid
    {
        get => _isValid;
        set
        {
            if (_isValid != value)
            {
                _isValid = value;
                OnPropertyChanged(nameof(IsValid));
                Debug.WriteLine($"프로젝트 경로 유효성: {value} (Project path validity: {value})");
            }
        }
    }

    private string _errorMsg;

    public string ErrorMsg
    {
        get => _errorMsg;
        set
        {
            if (_errorMsg != value)
            {
                _errorMsg = value;
                OnPropertyChanged(nameof(ErrorMsg));
                Debug.WriteLine($"오류 메시지: {value} (Error message: {value})");
            }
        }
    }

    private ProjectTemplate _selectedTemplate;

    public ProjectTemplate SelectedTemplate
    {
        get => _selectedTemplate;
        set
        {
            if (_selectedTemplate != value)
            {
                _selectedTemplate = value;
                OnPropertyChanged(nameof(SelectedTemplate));
                Debug.WriteLine(
                    $"선택된 템플릿 변경: {value?.ProjectType} (Selected template changed: {value?.ProjectType})");
            }
        }
    }

    private ObservableCollection<ProjectTemplate> _projectTemplate = new();
    public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates { get; }

    // 프로젝트 경로의 유효성을 검사하는 메서드
    // Method to validate the project path
    private bool ValidateProjectPath()
    {
        IsValid = false;
        Debug.WriteLine("프로젝트 경로 유효성 검사 시작 (Starting project path validation)");

        // 프로젝트 이름 검증 (Validate project name)
        if (string.IsNullOrWhiteSpace(_projectName?.Trim()))
        {
            ErrorMsg = "프로젝트 이름을 입력하세요. (Please enter a project name.)";
            Debug.WriteLine("프로젝트 이름이 비어있음 (Project name is empty)");
            return false;
        }

        if (_projectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
        {
            ErrorMsg = "프로젝트 이름에 유효하지 않은 문자가 포함되어 있습니다. (Project name contains invalid characters.)";
            Debug.WriteLine("프로젝트 이름에 유효하지 않은 문자 포함 (Project name contains invalid characters)");
            return false;
        }

        // 프로젝트 경로 검증 (Validate project path)
        if (string.IsNullOrWhiteSpace(_projectPath?.Trim()))
        {
            ErrorMsg = "유효한 프로젝트 폴더를 선택하세요. (Please select a valid project folder.)";
            Debug.WriteLine("프로젝트 경로가 비어있음 (Project path is empty)");
            return false;
        }

        try
        {
            var fullPath = Path.GetFullPath(Path.Combine(_projectPath, _projectName));
            Debug.WriteLine($"전체 경로: {fullPath} (Full path: {fullPath})");

            // 경로 길이 검사 (Check path length)
            if (fullPath.Length > 260)
            {
                ErrorMsg = "경로가 너무 깁니다. (Path is too long.)";
                Debug.WriteLine("경로 길이 초과 (Path length exceeded)");
                return false;
            }

            // 루트 드라이브 존재 여부 확인 (Check if root drive exists)
            var root = Path.GetPathRoot(fullPath);
            if (string.IsNullOrEmpty(root) || !Directory.Exists(root))
            {
                ErrorMsg = "유효하지 않은 드라이브 또는 네트워크 경로입니다. (Invalid drive or network path.)";
                Debug.WriteLine("유효하지 않은 드라이브 또는 네트워크 경로 (Invalid drive or network path)");
                return false;
            }

            // 부적절한 문자 검사 (Check for invalid characters)
            if (fullPath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                ErrorMsg = "프로젝트 경로에 유효하지 않은 문자가 포함되어 있습니다. (Project path contains invalid characters.)";
                Debug.WriteLine("프로젝트 경로에 유효하지 않은 문자 포함 (Project path contains invalid characters)");
                return false;
            }

            // 상위 폴더 존재 여부 및 접근 권한 확인 (Check if parent directory exists and is accessible)
            var parentDir = Directory.GetParent(fullPath);
            if (parentDir == null || !Directory.Exists(parentDir.FullName))
            {
                ErrorMsg = "상위 디렉토리가 존재하지 않습니다. (Parent directory does not exist.)";
                Debug.WriteLine("상위 디렉토리 존재하지 않음 (Parent directory does not exist)");
                return false;
            }

            if (!HasWriteAccessToFolder(parentDir.FullName))
            {
                ErrorMsg = "선택한 폴더에 대한 쓰기 권한이 없습니다. (No write permission for the selected folder.)";
                Debug.WriteLine("폴더에 대한 쓰기 권한 없음 (No write permission for the folder)");
                return false;
            }

            // 프로젝트 폴더 존재 여부 확인 (Check if project folder already exists)
            if (Directory.Exists(fullPath))
                if (Directory.EnumerateFileSystemEntries(fullPath).Any())
                {
                    ErrorMsg =
                        "선택한 프로젝트 폴더가 이미 존재하며 비어있지 않습니다. (Selected project folder already exists and is not empty.)";
                    Debug.WriteLine("프로젝트 폴더가 이미 존재하고 비어있지 않음 (Project folder already exists and is not empty)");
                    return false;
                }

            // 모든 검증을 통과 (All validations passed)
            ErrorMsg = string.Empty;
            IsValid = true;
            Debug.WriteLine("프로젝트 경로 유효성 검사 통과 (Project path validation passed)");
            return true;
        }
        catch (Exception ex)
        {
            ErrorMsg = $"유효하지 않은 프로젝트 경로: {ex.Message} (Invalid project path: {ex.Message})";
            Debug.WriteLine($"예외 발생: {ex.Message} (Exception occurred: {ex.Message})");
            return false;
        }
    }

    // 폴더에 대한 쓰기 권한을 확인하는 메서드
    // Method to check write access to a folder
    private bool HasWriteAccessToFolder(string folderPath)
    {
        try
        {
            // 임시 파일 생성 시도 (Try to create a temporary file)
            var tempFilePath = Path.Combine(folderPath, Path.GetRandomFileName());
            using (File.Create(tempFilePath))
            {
            }

            File.Delete(tempFilePath);
            Debug.WriteLine($"폴더에 쓰기 권한 확인: {folderPath} (Write permission confirmed for folder: {folderPath})");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(
                $"폴더 쓰기 권한 확인 실패: {ex.Message} (Failed to confirm write permission for folder: {ex.Message})");
            return false;
        }
    }

    // 새 프로젝트를 생성하는 메서드
    // Method to create a new project
    public string CreateProject(ProjectTemplate template)
    {
        if (template == null)
        {
            ErrorMsg = "프로젝트 템플릿을 선택해주세요. (Please select a project template.)";
            Debug.WriteLine("선택된 템플릿이 없습니다. (No template selected.)");
            return string.Empty;
        }

        Debug.WriteLine("프로젝트 생성 시작 (Starting project creation)");
        ValidateProjectPath();
        if (!IsValid)
        {
            Debug.WriteLine("프로젝트 경로가 유효하지 않음 (Project path is not valid)");
            return string.Empty;
        }

        var path = Path.Combine(ProjectPath, ProjectName);
        Debug.WriteLine($"프로젝트 생성 경로: {path} (Project creation path: {path})");

        try
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template), "템플릿이 null입니다. (Template is null.)");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.WriteLine($"프로젝트 폴더 생성: {path} (Project folder created: {path})");
            }

            if (template.Folders != null)
            {
                Debug.WriteLine(
                    $"템플릿 폴더: {string.Join(", ", template.Folders)} (Template folders: {string.Join(", ", template.Folders)})");
                foreach (var folder in template.Folders)
                    if (!string.IsNullOrWhiteSpace(folder))
                    {
                        var folderPath = Path.Combine(path, folder);
                        Directory.CreateDirectory(folderPath);
                        Debug.WriteLine($"하위 폴더 생성: {folderPath} (Subfolder created: {folderPath})");
                    }
            }
            else
            {
                Debug.WriteLine("템플릿 폴더 목록이 null입니다. (Template folder list is null.)");
            }

            var lambertDir = Path.Combine(path, ".Lambert");
            Directory.CreateDirectory(lambertDir);
            File.SetAttributes(lambertDir, FileAttributes.Hidden);
            Debug.WriteLine($".Lambert 숨김 폴더 생성: {lambertDir} (.Lambert hidden folder created: {lambertDir})");

            if (File.Exists(template.IconFilePath))
            {
                File.Copy(template.IconFilePath, Path.Combine(lambertDir, "Icon.png"), true);
                Debug.WriteLine("아이콘 파일 복사 완료 (Icon file copied)");
            }
            else
            {
                Debug.WriteLine(
                    $"아이콘 파일이 존재하지 않습니다: {template.IconFilePath} (Icon file does not exist: {template.IconFilePath})");
            }

            if (File.Exists(template.ScreenshotFilePath))
            {
                File.Copy(template.ScreenshotFilePath, Path.Combine(lambertDir, "Screenshot.png"), true);
                Debug.WriteLine("스크린샷 파일 복사 완료 (Screenshot file copied)");
            }
            else
            {
                Debug.WriteLine(
                    $"스크린샷 파일이 존재하지 않습니다: {template.ScreenshotFilePath} (Screenshot file does not exist: {template.ScreenshotFilePath})");
            }
            // var project = new Project(ProjectName, path);
            // var projectFilePath = Path.Combine(path, ProjectName + Project.Extension);
            // Serializer.ToFile(project, projectFilePath);
            // Debug.WriteLine($"프로젝트 파일 생성: {projectFilePath} (Project file created: {projectFilePath})");
            
            var projectXml = File.ReadAllText(template.ProjectFilePath);
            projectXml = projectXml.Replace("(0)", ProjectName).Replace("(1)", path);
            var projectPath = Path.GetFullPath(Path.Combine(path, $"{ProjectName}{Project.Extension}"));
            File.WriteAllText(projectPath, projectXml);
            Debug.WriteLine($"프로젝트 파일 생성: {projectPath} (Project file created: {projectPath})");

            return projectPath;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"프로젝트 생성 중 오류 발생: {ex.Message} (Error occurred during project creation: {ex.Message})");
            Debug.WriteLine($"스택 트레이스: {ex.StackTrace} (Stack trace: {ex.StackTrace})");
            ErrorMsg = $"프로젝트 생성 실패: {ex.Message} (Project creation failed: {ex.Message})";
            return string.Empty;
        }
    }

    // 생성자
    // Constructor
    public NewProject()
    {
        ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplate);
        Debug.WriteLine("NewProject 생성자 시작 (NewProject constructor started)");
        try
        {
            // 템플릿 파일 검색
            // Search for template files
            var templatesFiles = Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories);
            Debug.Assert(templatesFiles.Any(), "템플릿 파일을 찾을 수 없음 (No template files found)");
            Debug.WriteLine(
                $"발견된 템플릿 파일 수: {templatesFiles.Length} (Number of template files found: {templatesFiles.Length})");

            foreach (var file in templatesFiles)
            {
                // 템플릿 파일 역직렬화
                // Deserialize template file
                var template = Serializer.FromFile<ProjectTemplate>(file);

                // 아이콘 파일 경로 설정 및 로드
                // Set icon file path and load icon
                template.IconFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Icon.png"));
                if (File.Exists(template.IconFilePath))
                {
                    template.Icon = File.ReadAllBytes(template.IconFilePath);
                    Debug.WriteLine($"아이콘 로드됨: {template.IconFilePath} (Icon loaded: {template.IconFilePath})");
                }
                else
                {
                    Debug.WriteLine(
                        $"아이콘 파일 없음: {template.IconFilePath} (Icon file not found: {template.IconFilePath})");
                }

                // 스크린샷 파일 경로 설정 및 로드
                // Set screenshot file path and load screenshot
                template.ScreenshotFilePath =
                    Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Screenshot.png"));
                if (File.Exists(template.ScreenshotFilePath))
                {
                    template.Screenshot = File.ReadAllBytes(template.ScreenshotFilePath);
                    Debug.WriteLine(
                        $"스크린샷 로드됨: {template.ScreenshotFilePath} (Screenshot loaded: {template.ScreenshotFilePath})");
                }
                else
                {
                    Debug.WriteLine(
                        $"스크린샷 파일 없음: {template.ScreenshotFilePath} (Screenshot file not found: {template.ScreenshotFilePath})");
                }

                template.ProjectFilePath =
                    Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), template.ProjectFile));

                Debug.WriteLine($"템플릿 로드: {template.ProjectType} (Template loaded: {template.ProjectType})");
                if (template.Folders != null && template.Folders.Any())
                    Debug.WriteLine(
                        $"템플릿 폴더: {string.Join(", ", template.Folders)} (Template folders: {string.Join(", ", template.Folders)})");
                else
                    Debug.WriteLine("템플릿에 폴더가 정의되지 않음 (No folders defined in template)");

                _projectTemplate.Add(template);
            }

            // 초기 프로젝트 경로 유효성 검사
            // Initial project path validation
            ValidateProjectPath();
            Debug.WriteLine("초기 프로젝트 경로 유효성 검사 완료 (Initial project path validation completed)");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"템플릿 로딩 중 오류 발생: {ex.Message} (Error occurred while loading templates: {ex.Message})");
            Debug.WriteLine($"스택 트레이스: {ex.StackTrace} (Stack trace: {ex.StackTrace})");
            // TODO: 사용자에게 오류 알림 표시
            // TODO: Show error notification to user
        }

        Debug.WriteLine("NewProject 생성자 종료 (NewProject constructor ended)");
    }
}