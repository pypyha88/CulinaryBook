// Pages/Reg.xaml.cs 
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.ApplicationData;

namespace WpfApp1.Pages
{
    /// <summary> 
    /// 🧙 Страница регистрации новых волшебников для Кулинарной книги Гарри Поттера 
    /// </summary> 
    public partial class Reg : Page
    {
        public Reg()
        {
            InitializeComponent();

            // ✨ Инициализация страницы 
            InitializePage();
        }

        /// <summary> 
        /// 🎭 Инициализация страницы при загрузке 
        /// </summary> 
        private void InitializePage()
        {
            // Устанавливаем текущую дату в DatePicker 
            dB.SelectedDate = System.DateTime.Today;

            // Фокус на поле имени при загрузке 
            Loaded += (s, e) => txtName.Focus();

            // Отключаем кнопку регистрации до проверки паролей 
            btnReg.IsEnabled = false;
        }

        /// <summary> 
        /// ✨ Обработчик кнопки "Стать волшебником" 
        /// </summary> 
        private void btnReg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 🧙 Шаг 1: Проверка уникальности магического имени (логина) 
                if (IsLoginAlreadyExists())
                {
                    ShowMagicMessage("Это магическое имя уже занято!\nПридумайте другое.", "⚠️");
                    txtLogin.Focus();
                    txtLogin.SelectAll();
                    return;
                }

                // 🧙 Шаг 2: Проверка заполнения обязательных полей 
                if (!AreRequiredFieldsFilled())
                {
                    ShowMagicMessage("Заполните все обязательные поля!", "📝");
                    return;
                }

                // 🧙 Шаг 3: Проверка совпадения защитных заклинаний (паролей) 
                if (!DoPasswordsMatch())
                {
                    ShowMagicMessage("Защитные заклинания не совпадают!\nПроверьте правильность ввода.", "🔒");
                    pswPass.Focus();
                    pswPass.SelectAll();
                    return;
                }

                // 🧙 Шаг 4: Валидация введенных данных 
                if (!ValidateInputData())
                {
                    return;
                }

                // 🎉 Шаг 5: Создание нового волшебника 
                CreateNewWizard();

                // 🏰 Шаг 6: Возврат на страницу авторизации 
                ReturnToAuthorization();
            }
            catch (System.Exception ex)
            {
                // 💥 Обработка магических сбоев 
                ShowMagicMessage($"Произошел магический сбой:\n{ex.Message}", "💥", true);
            }
        }

        /// <summary> 
        /// 🔍 Проверка существования логина в базе данных 
        /// </summary> 
        private bool IsLoginAlreadyExists()
        {
            return AppConnect.model01.Authors
                .Any(x => x.Login == txtLogin.Text);
        }

        /// <summary> 
        /// 📝 Проверка заполнения обязательных полей 
        /// </summary> 
        private bool AreRequiredFieldsFilled()
        {
            // Проверяем основные поля 
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtLogin.Text) ||
                string.IsNullOrWhiteSpace(pswPass.Password))
            {
                return false;
            }

            // Проверяем дату рождения 
            if (!dB.SelectedDate.HasValue)
            {
                ShowMagicMessage("Выберите дату рождения!", "📅");
                return false;
            }

            // Проверяем стаж 
            if (string.IsNullOrWhiteSpace(stag.Text))
            {
                ShowMagicMessage("Укажите ваш стаж в магической кулинарии!", "🍳");
                return false;
            }

            return true;
        }

        /// <summary> 
        /// 🔒 Проверка совпадения паролей 
        /// </summary> 
        private bool DoPasswordsMatch()
        {
            return pswPass.Password == pswPass2.Password;
        }

        /// <summary> 
        /// 🧙 Валидация введенных данных 
        /// </summary> 
        private bool ValidateInputData()
        {
            // Проверка стажа (должно быть число) 
            if (!float.TryParse(stag.Text, out float stage))
            {
                ShowMagicMessage("Стаж должен быть числом!", "🔢");
                stag.Focus();
                stag.SelectAll();
                return false;
            }

            // Проверка, что стаж не отрицательный 
            if (stage < 0)
            {
                ShowMagicMessage("Стаж не может быть отрицательным!", "⚠️");
                stag.Focus();
                stag.SelectAll();
                return false;
            }

            // Проверка даты рождения (не может быть в будущем) 
            if (dB.SelectedDate > System.DateTime.Today)
            {
                ShowMagicMessage("Дата рождения не может быть в будущем!", "📅");
                dB.Focus();
                return false;
            }

            // Проверка возраста (должно быть больше 11 лет для Хогвартса) 
            var age = System.DateTime.Today.Year - dB.SelectedDate.Value.Year;
            if (age < 11)
            {
                ShowMagicMessage("В Хогвартс принимают с 11 лет!\nПодрасти немного 😊", "🏰");
                dB.Focus();
                return false;
            }

            return true;
        }

        /// <summary> 
        /// 🧙 Создание нового волшебника в базе данных 
        /// </summary> 
        private void CreateNewWizard()
        {
            // Создаем нового автора (волшебника) 
            Authors newWizard = new Authors()
            {
                AuthorName = txtName.Text.Trim(),
                Login = txtLogin.Text.Trim(),
                Password = pswPass.Password,
                BirthDay = dB.SelectedDate.Value,
                Stage = float.Parse(stag.Text),
                Mail = txtMail.Text?.Trim(),
                Number = txtNumber.Text?.Trim(),
                // 🏰 Автоматически определяем факультет по имени 
             
            };

            // Добавляем волшебника в базу данных 
            AppConnect.model01.Authors.Add(newWizard);
            AppConnect.model01.SaveChanges();

            // 🎉 Показываем сообщение об успехе 
            string successMessage = $"Поздравляем, {newWizard.AuthorName}! 🎉\n" +
                                  $"Вы успешно зарегистрированы в Хогвартсе!\n" +
                                  $"Ваш факультет: {"будет определен позже"}"; 

            ShowMagicMessage(successMessage, "✨");
        }

        /// <summary> 
        /// 🏰 Определение факультета по имени волшебника 
        /// </summary> 
        private string DetermineHouseByName(string wizardName)
        {
            // Простая логика определения факультета 
            // В реальном приложении можно сделать более сложную логику 

            if (wizardName.ToLower().Contains("гарри") ||
                wizardName.ToLower().Contains("рон") ||
                wizardName.ToLower().Contains("гермиона"))
                return "Gryffindor";

            if (wizardName.ToLower().Contains("драко") ||
                wizardName.ToLower().Contains("снейп"))
                return "Slytherin";

            if (wizardName.ToLower().Contains("луна") ||
                wizardName.ToLower().Contains("чжоу"))
                return "Ravenclaw";

            if (wizardName.ToLower().Contains("седарик") ||
                wizardName.ToLower().Contains("няфф"))
                return "Hufflepuff";

            return null; // Факультет определит Распределяющая шляпа позже 
        }

        /// <summary> 
        /// ↩️ Возврат на страницу авторизации 
        /// </summary> 
        private void ReturnToAuthorization()
        {
            // Ждем 2 секунды, чтобы пользователь увидел сообщение об успехе 
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = System.TimeSpan.FromSeconds(2);
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                ApplicationData.AppFrame.frmMain.GoBack();
            };
            timer.Start();
        }

        /// <summary> 
        /// 🔄 Обработчик изменения пароля (валидация в реальном времени) 
        /// </summary> 
        private void pswPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Проверяем совпадение паролей 
            bool passwordsMatch = DoPasswordsMatch();

            // Визуальная обратная связь 
            if (!passwordsMatch)
            {
                // 🔴 Пароли не совпадают 
                btnReg.IsEnabled = false;
                pswPass2.Background = System.Windows.Media.Brushes.LightCoral;
                pswPass2.BorderBrush = System.Windows.Media.Brushes.Red;

                // Показываем подсказку 
                if (!string.IsNullOrEmpty(pswPass2.Password))
                {
                    pswPass2.ToolTip = "Защитные заклинания не совпадают!";
                }
            }
            else
            {
                // ✅ Пароли совпадают 
                btnReg.IsEnabled = true;
                pswPass2.Background = System.Windows.Media.Brushes.LightGreen;
                pswPass2.BorderBrush = System.Windows.Media.Brushes.Green;
                pswPass2.ToolTip = "Защитные заклинания совпадают!";
            }
        }

        /// <summary> 
        /// ↩️ Обработчик кнопки "Вернуться" 
        /// </summary> 
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            // Возвращаемся на предыдущую страницу 
            ApplicationData.AppFrame.frmMain.GoBack();
        }

        /// <summary> 
        /// 🎪 Показать волшебное сообщение 
        /// </summary> 
        private void ShowMagicMessage(string message, string emoji, bool isError = false)
        {
            MessageBox.Show($"{emoji} {message}",
            isError ? "Магическая ошибка" : "Волшебное уведомление",
            MessageBoxButton.OK,
            isError ? MessageBoxImage.Error : MessageBoxImage.Information);
        }
    }
}

