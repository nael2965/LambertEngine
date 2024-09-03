using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using LambertEditor.GameProjectBrowser;

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
        ((INotifyCollectionChanged)Project.UndoRedo.UndoList).CollectionChanged += (s, e) => Focus();
    }

    
}