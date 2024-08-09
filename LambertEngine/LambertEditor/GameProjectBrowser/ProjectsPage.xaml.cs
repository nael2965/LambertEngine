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
    public partial class ProjectsPage : UserControl
    {
        public ProjectsPage()
        {
            InitializeComponent();
            Loaded += ProjectsPage_Loaded;
        }

        private void ProjectsPage_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("ProjectsPage 로드됨 (ProjectsPage loaded)");
            if (ProjectsListBox != null && ProjectsListBox.Items.Count > 0)
            {
                ProjectsListBox.SelectedIndex = 0;
            }
        }
        
        private void OnOpenProject(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("프로젝트 오픈 버튼 클릭 (Project open button clicked)");
            var selectedItem = ProjectsListBox.SelectedItem as ProjectData;
            if (selectedItem == null)
            {
                Debug.WriteLine("선택된 프로젝트가 null임.");
                return;
            }

            var project = OpenProject.Open(selectedItem);
            if (project != null)
            {
                Debug.WriteLine("프로젝트 열기 성공.");
                var window = Window.GetWindow(this) as ProjectBrowserDialog;
                if (window != null)
                {
                    window.DialogResult = true;
                    window.DataContext = project;
                    window.Close();
                }
            }
            else
            {
                Debug.WriteLine("프로젝트 열기 실패.");
                MessageBox.Show("프로젝트를 열 수 없습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
