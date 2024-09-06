using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using LambertEditor.Components;
using LambertEditor.GameProjectBrowser;

namespace LambertEditor.Editors;
public partial class ProjectLayoutView : UserControl
{
    public ProjectLayoutView()
    {
        InitializeComponent();
    }

    private void OnAddGameEntity_Button_Click(object sender, RoutedEventArgs e)
    {
        var btn = sender as Button;
        var vm = btn.DataContext as Scene;
        vm.AddGameEntityCommand.Execute(new GameEntity(vm){Name = "Empty Game Entity"});
    }
}