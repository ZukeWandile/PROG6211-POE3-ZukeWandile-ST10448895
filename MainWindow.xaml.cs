using System.Windows;
using System.Windows.Controls;

namespace demo2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Set the default page to HomePage when the application starts
            MainFrame.Navigate(new HomePage());
        }

        // Handles selection change in the menu (ListBox)
        private void MenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if the selected item is a ListBoxItem
            if (MenuListBox.SelectedItem is ListBoxItem selectedItem)
            {
                // Navigate to the appropriate page based on the selected menu option
                switch (selectedItem.Content.ToString())
                {
                    case "Home":
                        // Navigate to the Home page
                        MainFrame.Navigate(new HomePage());
                        break;

                    case "Chat Bot":
                        // Navigate to the Chat Bot page
                        MainFrame.Navigate(new bot());
                        break;

                    case "Quiz":
                        // Navigate to the Quiz page
                        MainFrame.Navigate(new QuizPage());
                        break;

                    case "Tasks":
                        // Navigate to the Task Manager page
                        MainFrame.Navigate(new tasking());
                        break;
                }
            }
        }
    }
}
