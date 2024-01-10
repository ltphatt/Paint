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
using System.Text.Json.Serialization;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;

namespace Paint
{
    public partial class MainWindow : RibbonWindow
    {
        private bool _isDrawing = false;
        private bool isSaved = false;


        List<IShape> _shapes = new List<IShape>();
        IShape _preview;
        Dictionary<string, IShape> _prototypes = new Dictionary<string, IShape>();
        string _selectedShapeName = "";


        private static int currentThickness = 1;
        private static SolidColorBrush currentColor = new SolidColorBrush(Colors.Black);
        private static DoubleCollection currentDash = null;

        private List<IShape> allShape = new List<IShape>();

        private string imagePath = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = true;
            Point pos = e.GetPosition(canvas);
            _preview.HandleStart(pos.X, pos.Y);
            _preview.ColorBrush = currentColor;
            _preview.Thickness = currentThickness;
            _preview.Dash = currentDash;

        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawing)
            {
                Point pos = e.GetPosition(canvas);

                _preview.HandleEnd(pos.X, pos.Y);

                ReDraw();
                // Ve lai hinh preview
                canvas.Children.Add(_preview.Draw());
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
            ReDraw();
        }
        private void ReDraw()
        {
            canvas.Children.Clear();
            foreach (var shape in _shapes)
            {
                UIElement element = shape.Draw();
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

            foreach(var item in _prototypes)
            {
                var shape = item.Value as IShape;
                allShape.Add(shape);
            }

            iconListView.ItemsSource = allShape;

            if(allShape.Count == 0)
            {
                return;
            }

            _selectedShapeName = allShape[0].Name;
            _preview = _prototypes[_selectedShapeName].Clone();
        }

        private void iconListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(allShape.Count == 0)
            {
                return;
            }

            var idx = iconListView.SelectedIndex;
            _selectedShapeName = allShape[idx].Name;
            _preview = _prototypes[_selectedShapeName].Clone();

        }

        private void newButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void openButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects
            };

            var serializedShapeList = JsonConvert.SerializeObject(_shapes, settings);

            // experience 
            StringBuilder builder = new StringBuilder();
            builder.Append(serializedShapeList).Append("\n").Append($"{imagePath}");
            string content = builder.ToString();


            var dialog = new System.Windows.Forms.SaveFileDialog();

            dialog.Filter = "JSON (*.json)|*.json";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;
                File.WriteAllText(path, content);
                isSaved = true;
            }
        }

        private void SaveCanvas(Canvas canvas, string fileName, string extension = "png")
        {
            var render = new RenderTargetBitmap((int)canvas.Width, (int)canvas.Height, 96d, 96d, PixelFormats.Pbgra32);

            canvas.Measure(new Size((int)canvas.Width, (int)canvas.Height));
            canvas.Arrange(new Rect(new Size((int)canvas.Width, (int)canvas.Height)));

            render.Render(canvas);

            switch (extension)
            {
                case "png":
                    var pngEncoder = new PngBitmapEncoder();
                    pngEncoder.Frames.Add(BitmapFrame.Create(render));

                    using (FileStream file = File.Create(fileName))
                    {
                        pngEncoder.Save(file);
                    }
                    break;
                case "jpeg":
                    var jpegEncoder = new JpegBitmapEncoder();
                    jpegEncoder.Frames.Add(BitmapFrame.Create(render));

                    using (FileStream file = File.Create(fileName))
                    {
                        jpegEncoder.Save(file);
                    }
                    break;
                case "tiff":
                    var tiffEncoder = new TiffBitmapEncoder();
                    tiffEncoder.Frames.Add(BitmapFrame.Create(render));

                    using (FileStream file = File.Create(fileName))
                    {
                        tiffEncoder.Save(file);
                    }
                    break;
                case "bmp":
                    var bitmapEncoder = new BmpBitmapEncoder();
                    bitmapEncoder.Frames.Add(BitmapFrame.Create(render));

                    using (FileStream file = File.Create(fileName))
                    {
                        bitmapEncoder.Save(file);
                    }
                    break;
                default:
                    break;
            }
        }


        private void saveAsBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void importButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void exportButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.Filter = "PNG (*.png)|*.png| JPEG (*.jpeg)|*.jpeg| BMP (*.bmp)|*.bmp | TIFF (*.tiff)|*.tiff";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;
                string extension = path.Substring(path.LastIndexOf('\\') + 1).Split('.')[1];

                SaveCanvas(canvas, path, extension);
            }
            isSaved = true;
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
            System.Windows.Forms.ColorDialog colorPicker = new System.Windows.Forms.ColorDialog();

            if (colorPicker.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                currentColor = new SolidColorBrush(Color.FromRgb(colorPicker.Color.R, colorPicker.Color.G, colorPicker.Color.B));
                iconBorder.Background = currentColor;
            }
        }

        private void blackBasicBtn_Click(object sender, RoutedEventArgs e)
        {
            currentColor = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            iconBorder.Background = currentColor;
        }

        private void grayBasicBtn_Click(object sender, RoutedEventArgs e)
        {
            currentColor = new SolidColorBrush(Color.FromRgb(127, 127, 127));
            iconBorder.Background = currentColor;
        }

        private void darkRedBtn_Click(object sender, RoutedEventArgs e)
        {
            currentColor = new SolidColorBrush(Color.FromRgb(136, 0, 21));
            iconBorder.Background = currentColor;
        }

        private void greenBtn_Click(object sender, RoutedEventArgs e)
        {
            currentColor = new SolidColorBrush(Color.FromRgb(34, 177, 76));
            iconBorder.Background = currentColor;
        }

        private void whiteBtn_Click(object sender, RoutedEventArgs e)
        {
            currentColor = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            iconBorder.Background = currentColor;
        }

        private void blueBtn_Click(object sender, RoutedEventArgs e)
        {
            currentColor = new SolidColorBrush(Color.FromRgb(0, 162, 232));
            iconBorder.Background = currentColor;
        }

        private void darkBlueBtn_Click(object sender, RoutedEventArgs e)
        {
            currentColor = new SolidColorBrush(Color.FromRgb(63, 72, 204));
            iconBorder.Background = currentColor;
        }

        private void purpleBtn_Click(object sender, RoutedEventArgs e)
        {
            currentColor = new SolidColorBrush(Color.FromRgb(163, 73, 164));
            iconBorder.Background = currentColor;
        }

        private void lavenderBtn_Click(object sender, RoutedEventArgs e)
        {
            currentColor = new SolidColorBrush(Color.FromRgb(200, 191, 231));
            iconBorder.Background = currentColor;
        }

        private void redBtn_Click(object sender, RoutedEventArgs e)
        {
            currentColor = new SolidColorBrush(Color.FromRgb(237, 28, 36));
            iconBorder.Background = currentColor;
        }

        private void orangeBtn_Click(object sender, RoutedEventArgs e)
        {
            currentColor = new SolidColorBrush(Color.FromRgb(255, 127, 39));
            iconBorder.Background = currentColor;
        }

        private void yellowBtn_Click(object sender, RoutedEventArgs e)
        {
            currentColor = new SolidColorBrush(Color.FromRgb(255, 242, 0));
            iconBorder.Background = currentColor;   
        }

        private void currentColorBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void brushesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void copyBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void pasteBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void currentColorBtn_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}