using System.Windows;
using System.Windows.Controls;

namespace demo2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new HomePage());
            // Default page
        }

        private void MenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MenuListBox.SelectedItem is ListBoxItem selectedItem)
            {
                switch (selectedItem.Content.ToString())
                {
                    case "Home":
                         MainFrame.Navigate(new HomePage()); 
                        break;
                    case "Chat Bot":
                        MainFrame.Navigate(new bot());
                        break;
                    case "Quiz":
                        MainFrame.Navigate(new QuizPage()); // ✅ ENABLE THIS LINE
                        break;
                    case "Tasks":
                        MainFrame.Navigate(new tasking());
                        break;
                }
            }
        }
    }
}
