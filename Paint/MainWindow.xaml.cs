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
        private bool _isSaved = false;
        private bool _isEditing = false;    


        List<IShape> _shapes = new List<IShape>();
        Stack<IShape> _shapesStack = new Stack<IShape>();

        IShape _preview;
        Dictionary<string, IShape> _prototypes = new Dictionary<string, IShape>();
        string _selectedShapeName = "";

        private static int _currentThickness = 1;
        private static SolidColorBrush _currentColor = new SolidColorBrush(Colors.Black);
        private static DoubleCollection _currentDash = null;

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
            _preview.ColorBrush = _currentColor;
            _preview.Thickness = _currentThickness;
            _preview.Dash = _currentDash;
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
            if(imagePath.Length >0 && _shapes.Count == 0)
            {
                imagePath = "";
                canvas.Background = new SolidColorBrush(Colors.White);
            }

            if(_shapes.Count == 0)
            {
                MessageBox.Show("This canvas was empty!");
                return;
            }

            if (_isSaved)
            {
                ResetCanvas();
                return;
            }

            var choice = MessageBox.Show("There are unsaved changes in your canvas.", "Do you want to save your work?", MessageBoxButton.YesNoCancel);

            if(MessageBoxResult.Yes == choice)
            {
                var dialog = new System.Windows.Forms.SaveFileDialog();
                dialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                dialog.FilterIndex = 1;

                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    string path = dialog.FileName;
                    WriteFile(path);
                }

                ResetCanvas();
                _isSaved = true;
            }
            else if(MessageBoxResult.No == choice)
            {
                ResetCanvas();
                return;
            }
            else
            {
                return;
            }

        }

        private void ResetCanvas()
        {
            if(allShape.Count == 0)
            {
                return;
            }

            _isSaved = false;
            _isEditing =false;
            _isDrawing = false;

            _shapes.Clear();
            _selectedShapeName = allShape[0].Name;
            _preview = _prototypes[_selectedShapeName];

            _currentThickness = 1;
            _currentColor = new SolidColorBrush(Colors.Black);
            _currentDash = null;

            imagePath = "";

            brushesComboBox.SelectedIndex = 0;
            sizeComboBox.SelectedIndex = 0;

            canvas.Children.Clear();
            canvas.Background = new SolidColorBrush(Colors.White);
        }

        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "JSON (*.json)|*.json";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = dialog.FileName;
                ReadFile(filePath);
                ReDraw();
            }
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            dialog.FilterIndex = 1;

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;
                WriteFile(path);
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
            _isSaved = true;
        }

        private void sizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = sizeComboBox.SelectedIndex;

            switch(index)
            {
                case 0:
                    _currentThickness = 1;
                    break;
                case 1:
                    _currentThickness = 3;
                    break;
                case 2:
                    _currentThickness = 5;
                    break;
                case 3:
                    _currentThickness = 8;
                    break;
                default:
                    break;
            }
        }
        private void brushesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = brushesComboBox.SelectedIndex;
            switch (index)
            {
                case 0:
                    _currentDash = null;
                    break;
                case 1:
                    _currentDash = new DoubleCollection() { 1 };
                    break;
                case 2:
                    _currentDash = new DoubleCollection() { 2 };
                    break;
                case 3:
                    _currentDash = new DoubleCollection() { 3 };
                    break;
            }
        }

        private void editColorBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorPicker = new System.Windows.Forms.ColorDialog();

            if (colorPicker.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _currentColor = new SolidColorBrush(Color.FromRgb(colorPicker.Color.R, colorPicker.Color.G, colorPicker.Color.B));
                iconBorder.Background = _currentColor;
            }
        }

        #region ColorButton
        private void blackBasicBtn_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            iconBorder.Background = _currentColor;
        }

        private void grayBasicBtn_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(127, 127, 127));
            iconBorder.Background = _currentColor;
        }

        private void darkRedBtn_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(136, 0, 21));
            iconBorder.Background = _currentColor;
        }

        private void greenBtn_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(34, 177, 76));
            iconBorder.Background = _currentColor;
        }

        private void whiteBtn_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            iconBorder.Background = _currentColor;
        }

        private void blueBtn_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(0, 162, 232));
            iconBorder.Background = _currentColor;
        }

        private void darkBlueBtn_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(63, 72, 204));
            iconBorder.Background = _currentColor;
        }

        private void purpleBtn_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(163, 73, 164));
            iconBorder.Background = _currentColor;
        }

        private void lavenderBtn_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(200, 191, 231));
            iconBorder.Background = _currentColor;
        }

        private void redBtn_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(237, 28, 36));
            iconBorder.Background = _currentColor;
        }

        private void orangeBtn_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(255, 127, 39));
            iconBorder.Background = _currentColor;
        }

        private void yellowBtn_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(255, 242, 0));
            iconBorder.Background = _currentColor;   
        }
      
        private void currentColorBtn_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Pick current color
        }
        #endregion

        #region Read and write file Json
        private void WriteFile(string filePath)
        {
            string json = JsonConvert.SerializeObject(_shapes, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
            });

            File.WriteAllText(filePath, json);
        }

        private void ReadFile(string filePath)
        {
            string json = File.ReadAllText(filePath);
            if (json == null) return;

            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects
            };
            _shapes = JsonConvert.DeserializeObject<List<IShape>>(json, settings)!;
        }
        #endregion

        #region Undo and Redo buttons
        private void undoBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_shapes.Count == 0)
            {
                return;
            }
            if (_shapes.Count == 0 && _shapesStack.Count == 0)
            {
                return;
            }

            int lastIndex = _shapes.Count - 1;
            _shapesStack.Push(_shapes[lastIndex]);
            _shapes.RemoveAt(lastIndex);

            ReDraw();
        }

        private void redoBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_shapesStack.Count == 0)
            {
                return;
            }
            if (_shapes.Count == 0 && _shapesStack.Count == 0)
            {
                return;
            }

            var lastShape = _shapesStack.Pop();
            _shapes.Add(lastShape);
            ReDraw();
        }
        #endregion



        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void copyBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void pasteBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cutBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void editModeBtn_Click(object sender, RoutedEventArgs e)
        {
            this._isEditing = !this._isEditing;
            if(_isEditing)
            {
                editModeBtn.Header = "Edit Mode";
            }
            else
            {
                editModeBtn.Header = "Draw Mode";
            }
        }

        private void addPictureBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "PNG (*.png)|*.png| JPEG (*.jpeg)|*.jpeg| BMP (*.bmp)|*.bmp";

            if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;

                imagePath = path;

                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri(path, UriKind.Absolute));
                canvas.Background = brush;
            }
        }

        private void addTextBtn_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}