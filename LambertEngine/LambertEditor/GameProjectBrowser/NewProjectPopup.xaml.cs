using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LambertEditor.GameProjectBrowser
{
    /// <summary>
    /// NewProjectPopup.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NewProjectPopup : UserControl{
        public event EventHandler CloseRequested;
        public NewProjectPopup(){
            InitializeComponent();
        }
        private void Cancle(object sender, RoutedEventArgs e){
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        private void OnCreate_Button_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("새 프로젝트 생성 버튼 클릭 (New project creation button clicked)");
            var vm = DataContext as NewProject;
            if (vm != null)
            {
                var selectedTemplate = tamplateListBox.SelectedItem as ProjectTemplate;
                if (selectedTemplate != null)
                {
                    Debug.WriteLine($"선택된 템플릿: {selectedTemplate.ProjectType} (Selected template: {selectedTemplate.ProjectType})");
                    var projectPath = vm.CreateProject(selectedTemplate);
                    if (!string.IsNullOrEmpty(projectPath))
                    {
                        Debug.WriteLine($"프로젝트 생성됨: {projectPath} (Project created: {projectPath})");
                        var projectData = new ProjectData
                        {
                            ProjectName = vm.ProjectName,
                            ProjectPath = projectPath,
                            Date = DateTime.Now
                        };
                        Debug.WriteLine($"새 ProjectData 생성: 이름={projectData.ProjectName}, 경로={projectData.ProjectPath} (New ProjectData created: Name={projectData.ProjectName}, Path={projectData.ProjectPath})");
                        
                        OpenProject.Open(projectData);
                        OpenProject.WriteProjectData();
                        Debug.WriteLine("프로젝트 데이터 저장됨 (Project data saved)");
                
                        var win = Window.GetWindow(this);
                        win.DataContext = projectData;
                        win.DialogResult = true;
                        win.Close();
                    }
                    else
                    {
                        Debug.WriteLine("프로젝트 생성 실패 (Project creation failed)");
                    }
                }
                else
                {
                    Debug.WriteLine("템플릿이 선택되지 않음 (No template selected)");
                    MessageBox.Show("프로젝트 템플릿을 선택해주세요.", "템플릿 선택 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}
