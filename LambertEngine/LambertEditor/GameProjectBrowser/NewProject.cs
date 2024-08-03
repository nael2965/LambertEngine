using LambertEditor.Common;
using LambertEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LambertEditor.GameProjectBrowser
{
    [DataContract]
    public class ProjectTemplate
    {
        ~ProjectTemplate() { Console.WriteLine("Delete tamplate"); }
        [DataMember]
        public string ProjectType { get; set; }
        [DataMember]
        public string ProjectFile { get; set; }
        [DataMember]
        public List<string> Folders { get; set; }

        public byte[] Icon { get; set; }
        public byte[] Screenshot { get; set; }
        public string IconFilePath { get; set; }
        public string ScreenshotFilePath { get; set; }
        public string ProjectFilePath { get; set; }
    }

    class NewProject : ViewModelBase
    {
        //TODO: Get Engine installation path
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
                }
            }
        }

        private string _projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\LambertProject\";
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
                }
            }
        }
        
        private ObservableCollection<ProjectTemplate> _projectTemplate = new ObservableCollection<ProjectTemplate>();
        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates
        { get; }

        private bool ValidateProjectPath()
        {
            IsValid = false;
            
            // 프로젝트 이름 검증
            if (string.IsNullOrWhiteSpace(_projectName?.Trim()))
            {
                ErrorMsg = "Type in a Project Name.";
                return false;
            }
            
            if (_projectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                ErrorMsg = "Invalid character(s) used in Project Name.";
                return false;
            }

            // 프로젝트 경로 검증
            if (string.IsNullOrWhiteSpace(_projectPath?.Trim()))
            {
                ErrorMsg = "Select a valid project folder.";
                return false;
            }

            try
            {
                var fullPath = Path.GetFullPath(Path.Combine(_projectPath, _projectName));

                // 경로 길이 검사
                if (fullPath.Length > 260) {
                    ErrorMsg = "Path is too long.";
                    return false;
                }

                // 루트 드라이브 존재 여부 확인
                var root = Path.GetPathRoot(fullPath);
                if (string.IsNullOrEmpty(root) || !Directory.Exists(root)) {
                    ErrorMsg = "Invalid drive or network path.";
                    return false;
                }

                // 부적절한 문자 검사
                if (fullPath.IndexOfAny(Path.GetInvalidPathChars()) != -1) {
                    ErrorMsg = "Invalid character(s) used in Project Path.";
                    return false;
                }

                // 경로 존재 여부 및 비어있는지 확인
                if (Directory.Exists(fullPath)) {
                    if (Directory.EnumerateFileSystemEntries(fullPath).Any()) {
                        ErrorMsg = "Selected project folder already exists and is not empty.";
                        return false;
                    }
                }
                else {
                    // 상위 디렉토리 접근 권한 확인
                    var parentDir = Directory.GetParent(fullPath);
                    if (parentDir != null && !HasWriteAccessToFolder(parentDir.FullName)) {
                        ErrorMsg = "No write permission to the selected folder.";
                        return false;
                    }
                }

                // 모든 검증을 통과
                ErrorMsg = string.Empty;
                IsValid = true;
                return true;
            }
            catch (Exception ex) {
                ErrorMsg = $"Invalid project path: {ex.Message}";
                return false;
            }
        }

        private bool HasWriteAccessToFolder(string folderPath) {
            try {
                // 임시 파일 생성 시도
                var tempFilePath = Path.Combine(folderPath, Path.GetRandomFileName());
                using (File.Create(tempFilePath)) { }
                File.Delete(tempFilePath);
                return true;
            }
            catch {
                return false;
            }
        }
        public NewProject()
        {
            ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplate);
            try
            {
                var templatesFiles = Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories);
                Debug.Assert(templatesFiles.Any());
                foreach (var file in templatesFiles)
                {
                    var template = Serializer.FromFile<ProjectTemplate>(file);
                    template.IconFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Icon.png "));
                    template.Icon = File.ReadAllBytes(template.IconFilePath);
                    template.ScreenshotFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Screenshot.png "));
                    template.Screenshot = File.ReadAllBytes(template.ScreenshotFilePath);
                    template.ProjectFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), template.ProjectFile));

                    _projectTemplate.Add(template);
                }   
                ValidateProjectPath();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                //TODO: Log Error
            }
        }
    }
}