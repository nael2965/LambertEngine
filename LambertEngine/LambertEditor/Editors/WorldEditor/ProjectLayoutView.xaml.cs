    using System.Windows;
    using System.Windows.Controls;
    using LambertEditor.GameProjectBrowser;

    namespace LambertEditor.Editors
    {
        public partial class ProjectLayoutView : UserControl
        {
            public ProjectLayoutView()
            {
                InitializeComponent();
            }

            private void On_AddSceen_Button_Clicked(object sender, RoutedEventArgs e)
            {
                var vm = DataContext as Project;
                vm.AddScene("New Scene " + vm.Scenes.Count);
            }
        }
    }

