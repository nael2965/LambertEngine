using System.ComponentModel;
using LambertEditor.GameProjectBrowser;
using System.Diagnostics;
using System.Windows;

namespace LambertEditor
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnMainWindowLoaded;
            Closing += OnMainWindowClosing;
            Debug.WriteLine("MainWindow 생성자 호출됨 (MainWindow constructor called)");
        }

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("MainWindow 로드됨 (MainWindow loaded)");
            Loaded -= OnMainWindowLoaded;
            OpenProjectDialog();
        }

        private void OnMainWindowClosing(object sender, CancelEventArgs e)
        {
            Debug.WriteLine("MainWindow 닫히는 중 (MainWindow closing)");
            Closing -= OnMainWindowClosing;
            Project.Current?.Unload();
            Debug.WriteLine("현재 프로젝트 언로드됨 (Current project unloaded)");
        }

        private void OpenProjectDialog()
        {
            Debug.WriteLine("프로젝트 브라우저 다이얼로그 열기 (Opening project browser dialog)");
            var projectBrowser = new ProjectBrowserDialog();
            if (projectBrowser.ShowDialog() == true)
            {
                Debug.WriteLine("프로젝트가 선택됨 (Project selected)");
                Project.Current?.Unload();
                DataContext = projectBrowser.DataContext;
                InitializeMainWindow();
            }
            else
            {
                Debug.WriteLine("프로젝트가 선택되지 않음, 애플리케이션 종료 (No project selected, shutting down application)");
                Application.Current.Shutdown();
            }
        }

        // 새로 추가된 메서드
        private void InitializeMainWindow()
        {
            Debug.WriteLine("MainWindow 초기화 시작 (Starting MainWindow initialization)");
            if (DataContext is Project project)
            {
                Title = $"Lambert Editor - {project.Name}";
                Debug.WriteLine($"프로젝트 로드됨: {project.Name} (Project loaded: {project.Name})");
                
                // TODO: 프로젝트 씬 목록 로드
                // TODO: Load project scene list
                Debug.WriteLine("씬 목록 로드 필요 (Need to load scene list)");

                // TODO: UI 업데이트
                // TODO: Update UI
                Debug.WriteLine("UI 업데이트 필요 (Need to update UI)");

                // TODO: 기타 필요한 초기화 작업
                // TODO: Other necessary initialization tasks
                Debug.WriteLine("추가 초기화 작업 필요 (Additional initialization tasks needed)");
            }
            else
            {
                Debug.WriteLine("오류: DataContext가 Project 타입이 아님 (Error: DataContext is not of type Project)");
            }
            Debug.WriteLine("MainWindow 초기화 완료 (MainWindow initialization completed)");
        }
    }
}