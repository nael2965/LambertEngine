using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LambertEditor.GameProject
{
    /// <summary>
    /// ProjectBrowserDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProjectBrowserDialog : Window
    {

        // 초기화
        public ProjectBrowserDialog()
        {
            InitializeComponent();
            NewProjectControl.CloseRequested += NewProjectControl_CloseRequested;
        }

        // 상태 저장용 변수
        private ToggleButton _lastCheckedButton = null;


        // 좌측 상단 토글 버튼 컨트롤러, 상호베타적
        private void OnToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_lastCheckedButton == null)
            {
                _lastCheckedButton = ProjectsButton;

            }

            ToggleButton clickedButton = sender as ToggleButton;
            if (clickedButton == null) return;

            // 클릭된 버튼이 이미 선택된 상태였다면 아무 동작도 하지 않음
            if (clickedButton == _lastCheckedButton) return;

            // 이전에 선택된 버튼 해제
            if (_lastCheckedButton != null)
            {
                _lastCheckedButton.IsChecked = false;
            }

                // 현재 클릭된 버튼 선택
                clickedButton.IsChecked = true;
            _lastCheckedButton = clickedButton;

            // MainPage 마진 조정
            if (clickedButton == ProjectsButton)
            {
                MainPage.Margin = new Thickness(0);
            }
            else if (clickedButton == AssetButton)
            {
                MainPage.Margin = new Thickness(-1400, 0, 0, 0);
            }
            else if (clickedButton == PluginsButton)
            {
                MainPage.Margin = new Thickness(-2800, 0, 0, 0);
            }

            // 이벤트 처리를 여기서 중단하여 기본 토글 동작을 막음
            e.Handled = true;
        }

        // 팝업을 표시하는 메서드
        private void ShowPopup()
        {
            PopupOverlay.Visibility = Visibility.Visible;
        }

        // 팝업을 닫는 메서드
        private void ClosePopup()
        {
            PopupOverlay.Visibility = Visibility.Collapsed;
        }

        // 팝업 창에서의 닫는 요청
        private void NewProjectControl_CloseRequested(object sender, EventArgs e)
        {
            ClosePopup();
        }

        // 새 프로젝트 이벤트
        private void NewProject(object sender, RoutedEventArgs e)
        {
            ShowPopup();
        }
    }
}
