using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace ContentBuild
{
    /// <summary>
    /// Main user interface for the program.
    /// </summary>
    public partial class MainForm : Form
    {
        ContentBuilder contentBuilder;
        ContentManager contentManager;
        string contentName;
        string contentType;


        public MainForm()
        {
            InitializeComponent();

            contentBuilder = new ContentBuilder();
            contentManager = new ContentManager(contentViewerControl.Services, contentBuilder.OutputDirectory);

            // Automatically bring up the "Load Content" dialog when first shown.
            this.Shown += openToolStripMenuItem_Click;
        }

        /// <summary>
        /// Event handler for the Open menu option.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfileDialog = new OpenFileDialog();

            // Default directory which contains content files.
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string relativePath = Path.Combine(assemblyLocation, "/Content");
            string contentPath = Path.GetFullPath(relativePath);

            openfileDialog.InitialDirectory = contentPath;
            openfileDialog.Title = "Load Content";
            openfileDialog.Filter = "Model Files (*.fbx;*.x)|*.fbx;*.x|" +
                                "Image Files (*.bmp;*.dds;*.dib;*.hdr;*.jpg;*.pfm;*.png;*.ppm;*.tga)|*.bmp;*.dds;*.dib;*.hdr;*.jpg;*.pfm;*.png;*.ppm;*.tga|" +
                                "Font Files (*.spritefont)|*.spritefont|" +
                                "Effect Files (*.fx)|*.fx|" +
                                "Sound Files (*.mp3;*.wav;*.wma)|*.mp3;*.wav;*.wma)|" +
                                "Video Files (*.wmv)|*.wmv|" +
                                "All Files (*.*)|*.*";

            if (openfileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadContent(openfileDialog.FileName);
            }
        }

        /// <summary>
        /// Event handler for the Save menu option.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefileDialog = new SaveFileDialog();

            savefileDialog.Title = "Save Content";
            savefileDialog.AddExtension = true;
            savefileDialog.DefaultExt = ".xnb";
            savefileDialog.FileName = contentName;
            if (savefileDialog.ShowDialog() == DialogResult.OK)
            {
                File.Copy(contentBuilder.OutputDirectory + "\\" + contentName + ".xnb", savefileDialog.FileName);
            }
        }

        /// <summary>
        /// Event handler for the Exit menu option.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }


        /// <summary>
        /// Builds and Loads content into the ContentViewerControl.
        /// </summary>
        /// <param name="fileName"></param>
        void LoadContent(string fileName)
        {
            Cursor = Cursors.WaitCursor;
            contentName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
            contentType = fileName.Substring(fileName.LastIndexOf(".") + 1);
            contentName = contentName.Remove(contentName.LastIndexOf("."));

            // Tell the ContentBuilder what to build.
            contentBuilder.Clear();
            switch (contentType)
            {
                case "x":
                case "fbx":
                    contentBuilder.Add(fileName, contentName, null, "ModelProcessor");
                    break;
                case "bmp":
                case "dds":
                case "dib":
                case "hdr":
                case "jpg":
                case "pfm":
                case "png":
                case "ppm":
                case "tga":
                    contentBuilder.Add(fileName, contentName, null, "TextureProcessor");
                    break;
                case "spritefont":
                    contentBuilder.Add(fileName, contentName, null, "FontDescriptionProcessor");
                    break;
                case "fx":
                    contentBuilder.Add(fileName, contentName, null, "EffectProcessor");
                    break;
                case "mp3":
                case "wav":
                case "wma":
                    contentBuilder.Add(fileName, contentName, null, "SoundEffectProcessor");
                    break;
                case "wmv":
                    contentBuilder.Add(fileName, contentName, null, "VideoProcessor");
                    break;
                default:
                    MessageBox.Show("Content Type Not Supported !", "Error");
                    Cursor = Cursors.Arrow;
                    return;
            }

            // Build this new content data.
            string buildError = contentBuilder.Build();
            if (string.IsNullOrEmpty(buildError))
            {
                // Unload any existing content.
                contentViewerControl.Clear();
                contentManager.Unload();
                // If the build succeeded, use the ContentManager to
                // load the temporary .xnb file that we just created.
                switch (contentType)
                {
                    case "x":
                    case "fbx":
                        contentViewerControl.Model = contentManager.Load<Model>(contentName);
                        break;
                    case "bmp":
                    case "dds":
                    case "dib":
                    case "hdr":
                    case "jpg":
                    case "pfm":
                    case "png":
                    case "ppm":
                    case "tga":
                        contentViewerControl.Image = contentManager.Load<Texture2D>(contentName);
                        break;
                    case "spritefont":
                        contentViewerControl.SpriteFont = contentManager.Load<SpriteFont>(contentName);
                        break;
                    case "fx":
                        saveToolStripMenuItem_Click(new object(), new EventArgs());
                        break;
                    case "mp3":
                    case "wav":
                    case "wma":
                        contentViewerControl.SoundEffect = contentManager.Load<SoundEffect>(contentName);
                        break;
                    case "wmv":
                        contentViewerControl.Video = contentManager.Load<Video>(contentName);
                        break;
                }
            }
            else
            {
                // If the build failed, display an error message.
                MessageBox.Show(buildError, "Error");
            }

            Cursor = Cursors.Arrow;
        }

    }
}
