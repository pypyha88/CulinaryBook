// 🧪 Код главного окна MainWindow.xaml.cs
using System.Linq;
using System.Net;
using System.Windows;
namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // 1. 🧪 Инициализация магического подключения к базе заклинаний (БД)
            InitializeDatabaseConnection();

            // 2. 🔮 Настройка портала для перемещений между страницами
            SetupNavigationSystem();

            // 3. 🚪 Загрузка страницы авторизации (Вход в Хогвартс)
            LoadAuthorizationPage();
        }

        /// <summary>
        /// 🧪 Инициализация подключения к базе данных
        /// </summary>
        private void InitializeDatabaseConnection()
        {
            try
            {
                // Создаем экземпляр контекста базы данных
                ApplicationData.AppConnect.model01 = new ApplicationData.CulinaryBookDBEntities();

            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Не удалось подключиться к волшебной базе данных:\n{ex.Message}", "Магическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 🔮 Настройка системы навигации
        /// </summary>
        private void SetupNavigationSystem()
        {
            // Инициализируем статический класс для работы с фреймом
            ApplicationData.AppFrame.frmMain = FrmMain;
        }
        /// <summary>
        /// 🚪 Загрузка страницы авторизации
        /// </summary>
        private void LoadAuthorizationPage()
        {
            // Создаем страницу авторизации
            FrmMain.Navigate(new Autorizaiton());
        }
    }
}
