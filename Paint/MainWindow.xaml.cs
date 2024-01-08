using Fluent;
using Contract;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Reflection;

namespace Paint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        private bool _isDrawing = false;
        List<IShape> _shapes = new List<IShape>();
        IShape _preview;
        Dictionary<string, IShape> _prototypes = new Dictionary<string, IShape>();
        string _selectedShapeName = "";


        private int currentThickness = 1;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = true;
            Point pos = e.GetPosition(canvas);
            _preview.HandleStart(pos.X, pos.Y);
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawing)
            {
                Point pos = e.GetPosition(canvas);

                _preview.HandleEnd(pos.X, pos.Y);

                // Xoa het cac hinh ve cu
                canvas.Children.Clear();

                // Ve lai cac hinh truoc do
                foreach(var shape in _shapes)
                {
                    UIElement element = shape.Draw(currentThickness);
                    canvas.Children.Add(element);
                }

                // Ve lai hinh preview
                canvas.Children.Add(_preview.Draw(currentThickness));
            }
        }

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = false;

            // Them doi tuong cuoi cung vao mang quan ly
            Point pos = e.GetPosition(canvas);
            _preview.HandleEnd(pos.X, pos.Y);
            _shapes.Add(_preview); // Đưa hình preview thành đối tượng chính thống

            // Sinh ra đối tượng mẫu kế
            _preview = _prototypes[_selectedShapeName].Clone();

            // Xóa toàn bộ
            canvas.Children.Clear();

            // Vẽ lại tất cả
            foreach(var shape in _shapes)
            {
                UIElement element = shape.Draw(currentThickness);
                canvas.Children.Add(element);
            }
        }

        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Nap cac DLL
            var exeFolder = AppDomain.CurrentDomain.BaseDirectory;
            var dlls = new DirectoryInfo(exeFolder).GetFiles("*.dll");

            foreach(var dll in dlls) {
                var assembly = Assembly.LoadFile(dll.FullName);
                var types = assembly.GetTypes();

                foreach(var type in types)
                {
                    if (type.IsClass)
                    {
                        if (typeof(IShape).IsAssignableFrom(type))
                        {
                            var shape = Activator.CreateInstance(type) as IShape;
                            _prototypes.Add(shape.Name, shape);
                        }
                    }
                }
            }

            // Tao ra cac button prototypes
            foreach(var item in _prototypes)
            {
                var shape = item.Value as IShape;

                var button = new System.Windows.Controls.Button()
                {
                    Width = 80,
                    Height = 35,
                    Margin = new Thickness(5, 0, 5, 0),
                    Tag = shape.Name,
                    
                    
                    
                };
                button.Click += prototypeButton_Click;
                prototypesStackPanel.Children.Add(button);
            }

            _selectedShapeName = _prototypes.First().Value.Name;
            _preview = _prototypes[_selectedShapeName].Clone();
        }

        private void prototypeButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedShapeName = (sender as System.Windows.Controls.Button).Tag as string;
            _preview = _prototypes[_selectedShapeName];
        }


        private void newButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void openButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void saveAsBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void importButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void sizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = sizeComboBox.SelectedIndex;

            switch(index)
            {
                case 0:
                    currentThickness = 1;
                    break;
                case 1:
                    currentThickness = 3;
                    break;
                case 2:
                    currentThickness = 5;
                    break;
                case 3:
                    currentThickness = 8;
                    break;
                default:
                    break;

            }
        }

        private void editColorBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void blackBasicBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void blackBasicBtn_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void grayBasicBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void darkRedBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void greenBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void whiteBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void blueBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void darkBlueBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void purpleBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lavenderBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void redBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void orangeBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void brushesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


    }
}