// Pages/Autorizaiton.xaml.cs 
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.ApplicationData;

namespace WpfApp1.Pages
{
    public partial class Autorizaiton : Page
    {
        // 🔔 Событие для уведомления об успешном входе волшебника 
        public event System.Action Logined;

        public Autorizaiton()
        {
            InitializeComponent();

        }

        /// <summary> 
        /// 🚪 Обработчик нажатия кнопки "Войти в Хогвартс" 
        /// </summary> 
        private void btnVhod_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 🧪 Проверка заполнения полей 
                if (string.IsNullOrWhiteSpace(txtLogin.Text))
                {
                    MessageBox.Show("Введите ваше магическое имя!", "Внимание",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtLogin.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPassword.Password))
                {
                    MessageBox.Show("Произнесите защитное заклинание!", "Внимание",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtPassword.Focus();
                    return;
                }

                // 🔮 Поиск волшебника в Книге заклинаний (БД) 
                var wizard = AppConnect.model01.Authors
                    .FirstOrDefault(x =>
                        x.Login == txtLogin.Text &&
                        x.Password == txtPassword.Password);

                if (wizard == null)
                {
                    MessageBox.Show("Неверное магическое имя или заклинание!\nПопробуйте снова.", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error); 

                    // 🔄 Сброс полей для повторной попытки 
                    txtPassword.Clear();
                    txtLogin.Focus();
                }
                else
                {
                    // 🎉 Успешный вход в Хогвартс! 
                    string welcomeMessage = $"Добро пожаловать, {wizard.AuthorName}!";
                    // 🍲 Переходим на главную страницу с рецептами 
                    ApplicationData.AppFrame.frmMain.Navigate(new PageTask());
                    AppConnect.AuthorID=-wizard.AuthorID;
                }
            }
            catch (System.Exception ex)
            {
                // 💥 Обработка магических сбоев 
                MessageBox.Show($"Произошла ошибка при входе:\n{ex.Message}", "Магический сбой", MessageBoxButton.OK, MessageBoxImage.Error); 
            }
        }

        private void btnReg_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}