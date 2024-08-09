using System.Windows;
using System.Windows.Controls;

namespace LambertEditor.Editors.WorldEditor;

public partial class WorldEditorView : UserControl
{
    public WorldEditorView()
    {
        InitializeComponent();
        Loaded += OnWorldEditorLoaded;
    }

    private void OnWorldEditorLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnWorldEditorLoaded;
        Focus();
    }

    
}