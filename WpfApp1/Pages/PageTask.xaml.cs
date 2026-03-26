using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp1.ApplicationData;

namespace WpfApp1
{
    /// <summary> 
    /// 🍲 Главная страница приложения - Библиотека магических рецептов 
    /// </summary> 
    public partial class PageTask : Page
    {
        public PageTask()
        {
            InitializeComponent();

            // ⏰ Инициализация страницы 
            InitializePage();
        }

        /// <summary> 
        /// 🎭 Инициализация страницы при загрузке 
        /// </summary> 
        private void InitializePage()
        {
            try
            {
                // 📚 Загрузка рецептов из базы данных 
                LoadRecipes();

                // 🏷️ Заполнение фильтров и сортировки 
                InitializeFilters();

                // 📊 Установка начальных значений 
                SetInitialValues();

                // 🔄 Обновление счетчика 
                UpdateRecipeCounter();
            }
            catch (System.Exception ex)
            {
                ShowErrorMessage($"Магический сбой при загрузке страницы:\n{ex.Message}");
            }
        }

        /// <summary> 
        /// 📚 Загрузка рецептов из базы данных 
        /// </summary> 
        private void LoadRecipes()
        {
            // Получаем все рецепты из базы данных 
            var recipes = AppConnect.model01.Recipes.ToList();

            // Устанавливаем источник данных для ListView 
            listProducts.ItemsSource = recipes;

            // Для отображения в виде плитки 
            // myGridView.ItemsSource = recipes; 
        }

        /// <summary> 
        /// 🏷️ Инициализация фильтров и сортировки 
        /// </summary> 
        private void InitializeFilters()
        {
            // Очистка существующих элементов 
            ComboSort.Items.Clear();
            ComboFilter.Items.Clear();

            // ⏰ Добавление вариантов сортировки 
            ComboSort.Items.Add("🔀 Без сортировки");
            ComboSort.Items.Add("🧙 По возрастанию времени");
            ComboSort.Items.Add("🧙 По убыванию времени");
            ComboSort.SelectedIndex = 0;

            // 🏷 ️ Добавление фильтра по категориям 
            ComboFilter.Items.Add("📁 Все категории");

            // Получаем категории из базы данных 
            var categories = AppConnect.model01.Categories.ToList();
            foreach (var category in categories)
            {
                ComboFilter.Items.Add($"📁 {category.CategoryName}");
            }

            ComboFilter.SelectedIndex = 0;
        }

        /// <summary> 
        /// 📊 Установка начальных значений 
        /// </summary> 
        private void SetInitialValues()
        {
            // Фокус на поле поиска 
            TextSearch.Focus();

            // Установка подсказки в поле поиска 
            TextSearch.Text = "";
        }

        /// <summary> 
        /// 🔄 Обновление счетчика найденных рецептов 
        /// </summary> 
        private void UpdateRecipeCounter()
        {
            var recipes = GetFilteredRecipes();

            if (recipes != null && recipes.Length > 0)
            {
                tbCounter.Text = $"✨ Найдено {recipes.Length} магических рецептов";
                tbCounter.Foreground = System.Windows.Media.Brushes.LightGreen;
            }
            else
            {
                tbCounter.Text = "😔 Рецепты не найдены";
                tbCounter.Foreground = System.Windows.Media.Brushes.LightCoral;
            }
        }

        /// <summary> 
        /// 🎯 Получение отфильтрованного списка рецептов 
        /// </summary> 
        private Recipes[] GetFilteredRecipes()
        {
            try
            {
                // Начальный список всех рецептов 
                var recipes = AppConnect.model01.Recipes.ToList();

                // 🔍 Фильтрация по поисковому запросу 
                if (!string.IsNullOrWhiteSpace(TextSearch.Text))
                {
                    recipes = recipes.Where(x =>
                        x.RecipeName.ToLower().Contains(TextSearch.Text.ToLower()) ||
                        x.Description.ToLower().Contains(TextSearch.Text.ToLower()) ||
                        x.Categories.CategoryName.ToLower().Contains(TextSearch.Text.ToLower())
                    ).ToList();
                }

                // 🏷️ Фильтрация по категории 
                if (ComboFilter.SelectedIndex > 0)
                {
                    // Получаем выбранную категорию 
                    string selectedCategory = ComboFilter.SelectedItem.ToString()
                        .Replace("📁 ", "");

                    recipes = recipes.Where(x =>
                        x.Categories.CategoryName == selectedCategory).ToList();
                }

                // ⏰ Сортировка 
                if (ComboSort.SelectedIndex > 0)
                {
                    switch (ComboSort.SelectedIndex)
                    {
                        case 1: // По возрастанию времени 
                            recipes = recipes.OrderBy(x => x.CookingTime).ToList();
                            break;

                        case 2: // По убыванию времени 
                            recipes = recipes.OrderByDescending(x => x.CookingTime).ToList();
                            break;
                    }
                }

                return recipes.ToArray();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        /// <summary> 
        /// 🔍 Обработчик изменения текста поиска 
        /// </summary> 
        private void TextSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Обновляем список рецептов 
            var filteredRecipes = GetFilteredRecipes();
            listProducts.ItemsSource = filteredRecipes;

            // Обновляем счетчик 
            UpdateRecipeCounter();
        }

        /// <summary> 
        /// 🏷️ Обработчик изменения фильтра по категориям 
        /// </summary> 
        private void ComboFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboFilter.SelectedIndex >= 0)
            {
                // Обновляем список рецептов 
                var filteredRecipes = GetFilteredRecipes();
                listProducts.ItemsSource = filteredRecipes;

                // Обновляем счетчик 
                UpdateRecipeCounter();
            }
        }

        /// <summary> 
        /// ⏰ Обработчик изменения сортировки 
        /// </summary> 
        private void ComboSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboSort.SelectedIndex >= 0)
            {
                // Обновляем список рецептов 
                var filteredRecipes = GetFilteredRecipes();
                listProducts.ItemsSource = filteredRecipes;

                // Обновляем счетчик 
                UpdateRecipeCounter();
            }
        }

        /// <summary> 
        /// 📋 Переключение на вид списка 
        /// </summary> 
        private void Button_Click_List(object sender, RoutedEventArgs e)
        {
            listProducts.Visibility = Visibility.Visible;
            myGridView.Visibility = Visibility.Collapsed;

            // Визуальная обратная связь для кнопок 
            btnListView.Style = (Style)Application.Current.FindResource("ActiveViewToggleButtonStyle"); 
            btnGridView.Style = (Style)Application.Current.FindResource("ViewToggleButtonStyle"); 
        }

        /// <summary> 
        /// ⏰ Переключение на вид плитки 
        /// </summary> 
        private void Button_Click_Grid(object sender, RoutedEventArgs e)
        {
            listProducts.Visibility = Visibility.Collapsed;
            myGridView.Visibility = Visibility.Visible;
            myGridView.ItemsSource = GetFilteredRecipes();

            // Визуальная обратная связь для кнопок 
            btnGridView.Style = (Style)Application.Current.FindResource("ActiveViewToggleButtonStyle"); 
            btnListView.Style = (Style)Application.Current.FindResource("ViewToggleButtonStyle"); 
        }

        /// <summary> 
        /// ✏️ Обработчик кнопки редактирования рецепта 
        /// </summary> 
        private void btnRed_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is Recipes selectedRecipe)
            {
                // 🎭 Переход на страницу редактирования рецепта 
              //  NavigationService.Navigate(new Pages.AddRecip(selectedRecipe));
            }
            else
            {
                ShowInfoMessage("Выберите рецепт для редактирования!");
            }
        }

        /// <summary> 
        /// ➕ Обработчик кнопки добавления нового рецепта 
        /// </summary> 
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // 🎭 Переход на страницу добавления нового рецепта 
         //   NavigationService.Navigate(new Pages.AddRecip(null));
        }

        /// <summary> 
        /// ❤️ Обработчик кнопки "В избранное" 
        /// </summary> 
        private void btnLike_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is Recipes selectedRecipe)
            {
                ToggleFavoriteRecipe(selectedRecipe);
            }
            else
            {
                ShowInfoMessage("Выберите рецепт для добавления в избранное!");
            }
        }

        /// <summary> 
        /// ❤️ Добавление/удаление рецепта из избранного 
        /// </summary> 
        private void ToggleFavoriteRecipe(Recipes recipe)
        {
            try
            {
                // Проверяем, есть ли уже этот рецепт в избранном у текущего пользователя
                                var existingFavorite = AppConnect.model01.LikeRecipes
                                    .FirstOrDefault(x =>
                                        x.idAuthor == AppConnect.AuthorID &&
                                        x.idRecipes == recipe.RecipeID);

                if (existingFavorite == null)
                {
                    // 📝 Добавляем в избранное 
                    LikeRecipes newFavorite = new LikeRecipes()
                    {
                        idRecipes = recipe.RecipeID,
                        idAuthor = AppConnect.AuthorID,
                       // AddedDate = System.DateTime.Now
                    };

                    AppConnect.model01.LikeRecipes.Add(newFavorite);
                    AppConnect.model01.SaveChanges();

                    ShowSuccessMessage($"Рецепт \"{recipe.RecipeName}\" добавлен в избранное! ❤️"); 
                }
                else
                {
                    // 🗑️ Удаляем из избранного 
                    AppConnect.model01.LikeRecipes.Remove(existingFavorite);
                    AppConnect.model01.SaveChanges();

                    ShowInfoMessage($"Рецепт \"{recipe.RecipeName}\" удален из избранного."); 
                }

                // 🔄 Обновляем кнопку (можно добавить визуальную обратную связь) 
                UpdateLikeButtonState(recipe);
            }
            catch (System.Exception ex)
            {
                ShowErrorMessage($"Ошибка при работе с избранным:\n{ex.Message}");
            }
        }

        /// <summary> 
        /// 🔄 Обновление состояния кнопки "В избранное" 
        /// </summary> 
        private void UpdateLikeButtonState(Recipes recipe)
        {
            // Можно добавить логику изменения внешнего вида кнопки 
            // в зависимости от того, есть ли рецепт в избранном 

            var isFavorite = AppConnect.model01.LikeRecipes
                .Any(x => x.idAuthor == AppConnect.AuthorID && x.idRecipes == recipe.RecipeID);

            // Пример изменения текста кнопки: 
            // if (isFavorite) 
            //     button.Content = "❤️ В избранном"; 
            // else 
            //     button.Content = "⏰ В избранное"; 
        }

        /// <summary> 
        /// 📖 Обработчик кнопки перехода в избранное 
        /// </summary> 
        private void LikeButton_Click(object sender, RoutedEventArgs e)
        {
            // 🎭 Переход на страницу избранных рецептов 
         //   ApplicationData.AppFrame.frmMain.Navigate(new PageLike());
        }

        /// <summary> 
        /// 🖱️ Обработчик двойного клика по рецепту 
        /// </summary> 
        private void listProducts_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (listProducts.SelectedItem is Recipes selectedRecipe)
            {
                // 🎭 Переход на страницу просмотра/редактирования рецепта 
               // NavigationService.Navigate(new Pages.AddRecip(selectedRecipe));
            }
        }

        /// <summary> 
        /// 💬 Показать информационное сообщение 
        /// </summary> 
        private void ShowInfoMessage(string message)
        {
            MessageBox.Show(message, "📖 Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary> 
        /// ✅ Показать сообщение об успехе 
        /// </summary> 
        private void ShowSuccessMessage(string message)
        {
            MessageBox.Show(message, "✨ Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary> 
        /// ❌ Показать сообщение об ошибке 
        /// </summary> 
        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "💥 Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
