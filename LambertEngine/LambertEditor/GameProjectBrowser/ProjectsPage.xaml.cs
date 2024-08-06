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
        }
        
        private void OnOpenProject(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("프로젝트 오픈 버튼 클릭 (Project open button clicked)");
            var project = OpenProject.Open(ProjectsListBox.SelectedItems as ProjectData);
            bool dialogResult = false;
            var win = Window.GetWindow(this);
            if (project != null)
            {
                dialogResult = true;
                Debug.WriteLine("프로젝트 선택 됨.");
            }
            else
            {
                Debug.WriteLine("선택된 프로젝트가 null임.");
            }
            win.DialogResult = dialogResult;
            win.Close();
        }
    }
}
