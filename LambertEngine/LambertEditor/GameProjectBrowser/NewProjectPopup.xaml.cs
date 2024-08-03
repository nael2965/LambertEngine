using System;
using System.Collections.Generic;
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
            var vm = DataContext as NewProject;
            if (vm != null)
            {
                var selectedTemplate = tamplateListBox.SelectedItem as ProjectTemplate;
                if (selectedTemplate != null)
                {
                    var projectPath = vm.CreateProject(selectedTemplate);
                    bool dialogResult = false;
                    var win = Window.GetWindow(this);
                    if (!string.IsNullOrEmpty(projectPath))
                    {
                        dialogResult = true;
                    }
                    win.DialogResult = dialogResult;
                    win.Close();
                }
                else
                {
                    MessageBox.Show("프로젝트 템플릿을 선택해주세요.", "템플릿 선택 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}
