using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfApp1.ApplicationData;

namespace WpfApp1.Pages
{
    /// <summary>
    /// 🍲 Страница добавления / редактирования рецепта
    /// </summary>
    public partial class AddRecip : Page
    {
        // 📦 Текущий рецепт (null — новый, не null — редактирование)
        private Recipes _recipe;

        public AddRecip(Recipes recipe = null)
        {
            InitializeComponent();

            _recipe = recipe ?? new Recipes();

            // 🏷️ Заполняем выпадающие списки
            FillCategories();
            FillAuthors();

            // 📝 Если редактируем — подгружаем данные
            if (recipe != null)
            {
                LoadRecipeData(recipe);
            }
        }

        /// <summary>
        /// 📁 Заполнение списка категорий
        /// </summary>
        private void FillCategories()
        {
            CategoryCombo.Items.Clear();
            CategoryCombo.Items.Add("📁 Выбор категории");
            CategoryCombo.SelectedIndex = 0;

            var categories = AppConnect.model01.Categories;
            foreach (var c in categories)
            {
                CategoryCombo.Items.Add(c.CategoryName);
            }
        }

        /// <summary>
        /// 🧙 Заполнение списка авторов
        /// </summary>
        private void FillAuthors()
        {
            AuthorCombo.Items.Clear();
            AuthorCombo.Items.Add("🧙 Выбор автора");
            AuthorCombo.SelectedIndex = 0;

            var authors = AppConnect.model01.Authors;
            foreach (var a in authors)
            {
                AuthorCombo.Items.Add(a.AuthorName);
            }
        }

        /// <summary>
        /// 📖 Подгрузка данных рецепта при редактировании
        /// </summary>
        private void LoadRecipeData(Recipes recipe)
        {
            NameRecepis.Text = recipe.RecipeName;
            DescRecipes.Text = recipe.Description;
            TextTime.Text = recipe.CookingTime?.ToString();
            TextPage.Text = recipe.Image;

            // 🏷️ Выбор категории
            if (recipe.CategoryID.HasValue && recipe.CategoryID.Value < CategoryCombo.Items.Count)
                CategoryCombo.SelectedIndex = recipe.CategoryID.Value;

            // 🧙 Выбор автора
            if (recipe.AuthorID.HasValue && recipe.AuthorID.Value < AuthorCombo.Items.Count)
                AuthorCombo.SelectedIndex = recipe.AuthorID.Value;

            // 🖼️ Загрузка изображения
            if (!string.IsNullOrEmpty(recipe.Image))
            {
                string imagePath = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Images\\", recipe.Image);
                LoadImageToPictureBox(imagePath);
            }
        }

        /// <summary>
        /// ✅ Обработчик кнопки "Добавить"
        /// </summary>
        private void AddRecep_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 🔍 Валидация полей
                if (string.IsNullOrWhiteSpace(NameRecepis.Text) ||
                    string.IsNullOrWhiteSpace(DescRecipes.Text) ||
                    string.IsNullOrWhiteSpace(TextTime.Text) ||
                    CategoryCombo.SelectedIndex == 0 ||
                    AuthorCombo.SelectedIndex == 0)
                {
                    MessageBox.Show("Не заполнены все поля!", "📖 Уведомление",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // ⏰ Проверка числового формата времени
                if (!int.TryParse(TextTime.Text, out int cookingTime) || cookingTime <= 0)
                {
                    MessageBox.Show("Введите корректное время приготовления (целое число)!", "📖 Уведомление",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 📝 Заполняем объект рецепта
                _recipe.RecipeName = NameRecepis.Text.Trim();
                _recipe.Description = DescRecipes.Text.Trim();
                _recipe.CategoryID = CategoryCombo.SelectedIndex;
                _recipe.AuthorID = AuthorCombo.SelectedIndex;
                _recipe.CookingTime = cookingTime;
                _recipe.Image = TextPage.Text;

                // 💾 Сохраняем в базу
                if (_recipe.RecipeID == 0)
                {
                    // ➕ Новый рецепт
                    AppConnect.model01.Recipes.Add(_recipe);
                }
                // (если RecipeID != 0 — EF уже отслеживает изменения)

                AppConnect.model01.SaveChanges();

                MessageBox.Show("✨ Данные успешно добавлены!", "✅ Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // 🔙 Возврат к списку рецептов
                ApplicationData.AppFrame.frmMain.Navigate(new PageTask());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"💥 Ошибка при добавлении данных:\n{ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// ⬅️ Обработчик кнопки "К рецептам"
        /// </summary>
        private void HomeWorld_Click(object sender, RoutedEventArgs e)
        {
            ApplicationData.AppFrame.frmMain.Navigate(new PageTask());
        }

        /// <summary>
        /// 📂 Обработчик кнопки загрузки изображения
        /// </summary>
        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*",
                Title = "Выберите изображение"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string photoName = System.IO.Path.GetFileName(dialog.FileName);
                    string imagesDirectory = System.IO.Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Images\\");

                    if (!Directory.Exists(imagesDirectory))
                        Directory.CreateDirectory(imagesDirectory);

                    string destinationPath = System.IO.Path.Combine(imagesDirectory, photoName);
                    File.Copy(dialog.FileName, destinationPath, true);

                    _recipe.Image = photoName;
                    TextPage.Text = photoName;

                    LoadImageToPictureBox(destinationPath);

                    MessageBox.Show($"📸 Изображение загружено: {photoName}", "✅ Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"💥 Ошибка при загрузке изображения:\n{ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Изображение не выбрано.", "⚠️ Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// 🖼️ Загрузка изображения в превью
        /// </summary>
        private void LoadImageToPictureBox(string imagePath)
        {
            if (!File.Exists(imagePath)) return;

            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(imagePath);
                bitmap.EndInit();

                pictureBox.Source = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"💥 Ошибка при отображении изображения:\n{ex.Message}");
            }
        }
    }
}
