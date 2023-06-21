using System.IO;
using System.IO.Compression;
using System.Text.Json;
using Microsoft.VisualBasic.Logging;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Drawing.Text;

namespace ModifyNow {
    public partial class ModifyNow : Form {

        public ModifyNow()
        {
            InitializeComponent();

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filePath = Path.Combine(appDataPath + "//ModifyNow//", "config.json");

            // Check if the JSON file exists
            if (File.Exists(filePath))
            {
                currentLanguageText();
            }
            else
            {
                // Create a new JSON object with default values
                var jsonObject = new
                {
                    language = "EN"
                };

                // Serialize the JSON object
                string jsonData = JsonSerializer.Serialize(jsonObject);

                // Save the JSON data to the file
                File.WriteAllText(filePath, jsonData);

                Console.WriteLine("Config file created with default values.");
            }

            changeGUI();
        }

        string appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        //BUTTONS-------------------------------------------------------------------------------
        private void installRarZipBttn_Click(object sender, EventArgs e)
        {
            modsProgressBar.Maximum = 3;
            modsProgressBar.Value = 0;

            string compressedFilePath;
            string compressedFileName;
            string compressedFileExtension;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                try
                {
                    openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.Filter = currentLanguage[0];
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        compressedFilePath = openFileDialog.FileName;
                        compressedFileName = openFileDialog.SafeFileName;
                        compressedFileExtension = Path.GetExtension(compressedFileName);

                        modsProgressBar.Value = 1;
                        clearModsFolder(compressedFilePath, compressedFileExtension);

                    }
                }
                catch
                {
                    string message = "Error when selecting file. Please make sure it is a RAR/ZIP file and that it is not damaged.";
                    string caption = "Error Detected when selecting file";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result;

                    //Displays the MessageBox
                    result = MessageBox.Show(message, caption, buttons);
                }

            }
        }


        private void installFromFolderBttn_Click(object sender, EventArgs e)
        {
            modsProgressBar.Value = 0;
            modsProgressBar.Maximum = 3;

            string modsFilePath;

            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                try
                {
                    folderBrowserDialog.InitialDirectory = "c:\\";

                    if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                    {
                        modsFilePath = folderBrowserDialog.SelectedPath;

                        modsProgressBar.Value = 1;


                        clearModsFolder(modsFilePath, "");
                    }
                }
                catch
                {
                    string message = "Error when selecting folder. Try running this app with admin privileges.";
                    string caption = "Error Detected when selecting folder";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result;

                    //Displays the MessageBox
                    result = MessageBox.Show(message, caption, buttons);
                }

            }

        }

        private void installFromJarButton_Click(object sender, EventArgs e)
        {
            modsProgressBar.Maximum = 3;
            modsProgressBar.Value = 0;

            string[] jarFiles;


            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Multiselect = true;
                try
                {
                    openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.Filter = currentLanguage[1];
                    openFileDialog.ValidateNames = true;
                    openFileDialog.CheckFileExists = true;
                    openFileDialog.CheckPathExists = true;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        jarFiles = openFileDialog.FileNames;


                        modsProgressBar.Value = 1;

                        clearModsFolderForJars(jarFiles);
                    }
                }
                catch
                {
                    string message = "Error when selecting JAR files. Please try running again with Admin privileges.";
                    string caption = "Error Detected when selecting JAR files";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result;

                    //Displays the MessageBox
                    result = MessageBox.Show(message, caption, buttons);
                }

            }
        }

        private void updateFromCompressedBttn_Click(object sender, EventArgs e)
        {
            modsProgressBar.Maximum = 3;
            modsProgressBar.Value = 0;

            string compressedFilePath;
            string compressedFileName;
            string compressedFileExtension;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                string minecraftModsPath = appDataFolderPath + "\\.minecraft\\mods\\";

                try
                {
                    openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.Filter = currentLanguage[0];
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        compressedFilePath = openFileDialog.FileName;
                        compressedFileName = openFileDialog.SafeFileName;
                        compressedFileExtension = Path.GetExtension(compressedFileName);

                        modsProgressBar.Value = 1;
                        extractMods(compressedFileExtension, compressedFilePath, minecraftModsPath);
                    }
                }
                catch
                {
                    string message = "Error when selecting file. Please make sure it is a RAR/ZIP file and that it is not damaged.";
                    string caption = "Error Detected when selecting file";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result;

                    //Displays the MessageBox
                    result = MessageBox.Show(message, caption, buttons);
                }

            }
        }

        private void updateFromJar_Click(object sender, EventArgs e)
        {
            modsProgressBar.Maximum = 3;
            modsProgressBar.Value = 0;

            string[] jarFiles;


            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Multiselect = true;
                string minecraftModsPath = appDataFolderPath + "\\.minecraft\\mods\\";

                try
                {
                    openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.Filter = currentLanguage[1];
                    openFileDialog.ValidateNames = true;
                    openFileDialog.CheckFileExists = true;
                    openFileDialog.CheckPathExists = true;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        jarFiles = openFileDialog.FileNames;


                        modsProgressBar.Value = 1;

                        installJarFiles(jarFiles, minecraftModsPath);
                    }
                }
                catch
                {
                    string message = "Error when selecting JAR files. Please try running again with Admin privileges.";
                    string caption = "Error Detected when selecting JAR files";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result;

                    //Displays the MessageBox
                    result = MessageBox.Show(message, caption, buttons);
                }

            }
        }

       
        //MODS FOLDER MANAGMENT-------------------------------------------------------------------------------
        private void clearModsFolder(string path, string extension)
        {
            try
            {
                string minecraftModsPath = appDataFolderPath + "\\.minecraft\\mods\\";

                if (Directory.Exists(minecraftModsPath))
                {
                    DirectoryInfo dir = new DirectoryInfo(minecraftModsPath);

                    foreach (FileInfo file in dir.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo directory in dir.GetDirectories())
                    {
                        directory.Delete(true);
                    }
                }
                else
                {
                    Directory.CreateDirectory(minecraftModsPath);
                }


                modsProgressBar.Value = 2;

                if (extension != "")
                {
                    extractMods(extension, path, minecraftModsPath);

                }
                else
                {
                    copyMods(path, minecraftModsPath);
                }
            }
            catch
            {
                string message = "Error when deleting the contents of the mods folder. Try running this app with Admin privileges.";
                string caption = "Error Detected when clearing the mods folder";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                //Displays the MessageBox
                result = MessageBox.Show(message, caption, buttons);
            }
        }

        private void clearModsFolderForJars(string[] jarFiles)
        {
            try
            {
                string minecraftModsPath = appDataFolderPath + "\\.minecraft\\mods\\";

                if (Directory.Exists(minecraftModsPath))
                {
                    DirectoryInfo dir = new DirectoryInfo(minecraftModsPath);

                    foreach (FileInfo file in dir.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo directory in dir.GetDirectories())
                    {
                        directory.Delete(true);
                    }
                }
                else
                {
                    Directory.CreateDirectory(minecraftModsPath);
                }


                modsProgressBar.Value = 2;

                installJarFiles(jarFiles, minecraftModsPath);


            }
            catch
            {
                string message = "Error when deleting the contents of the mods folder. Try running this app with Admin privileges.";
                string caption = "Error Detected when clearing the mods folder";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                //Displays the MessageBox
                result = MessageBox.Show(message, caption, buttons);
            }
        }

        //MODS MANAGMENT-------------------------------------------------------------------------------
        private void extractMods(string extension, string path, string minecraftModsPath)
        {
            try
            {
                if (extension == ".zip")
                {
                    ZipFile.ExtractToDirectory(path, minecraftModsPath);
                }
                else
                {
                    using (var archive = RarArchive.Open(path))
                    {
                        foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                        {
                            entry.WriteToDirectory(minecraftModsPath, new ExtractionOptions()
                            {
                                ExtractFullPath = true,
                                Overwrite = true
                            });
                        }
                    }
                }

                modsProgressBar.Value = 3;
            }
            catch
            {
                string message = "Error when extracting the mods to the mods folder. Try running this app with Admin privileges.";
                string caption = "Error Detected when extracting the mods.";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                //Displays the MessageBox
                result = MessageBox.Show(message, caption, buttons);
            }
        }

        private void copyMods(string path, string minecraftModsPath)
        {
            try
            {
                string[] fileEntries = Directory.GetFiles(path, "*.jar"); // Filter files by "*.jar"

                foreach (string fileName in fileEntries)
                {
                    string destinationPath = Path.Combine(minecraftModsPath, Path.GetFileName(fileName));
                    File.Copy(fileName, destinationPath, true);


                }

                modsProgressBar.Value = 3;
            }

            catch
            {
                string message = "Error when copying the mods to the mods folder. Try running this app with Admin privileges.";
                string caption = "Error Detected when copying the mods.";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                result = MessageBox.Show(message, caption, buttons);
            }

        }

        private void installJarFiles(string[] jarFiles, string path)
        {
            try
            {
                foreach (string file in jarFiles)
                {
                    FileInfo jarFile = new FileInfo(file);
                    string destinationPath = Path.Combine(path, jarFile.Name);
                    jarFile.CopyTo(destinationPath, true);

                    modsProgressBar.Value = 3;
                }
            }
            catch
            {
                string message = "Error when copying the mods to the mods folder. Try running this app with Admin privileges.";
                string caption = "Error Detected when copying the mods.";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                result = MessageBox.Show(message, caption, buttons);
            }
        }


       
        
        //LANGUAGE-------------------------------------------------------------------------------------------------------


        /* FROM 0 TO 8, IT IS ALL MODS RELATED TEXT.
        /* [0 , 1] = FILTERS OF FILE BROWSERS, 0 -> COMPRESSED, 1 -> JAR
        /* [2 TO 6] = Text of Buttons. In order: Install COMPRESSED, INSTALL FOLDER, INSTALL JAR, UPDATE COMPRESSED, UPDATE JAR.
        /* [7 , 8] = Text of Warnings. In order: 7 -> Warning text, 8 -> Update "warning" text.          
         */
        string[] spanish = {
            //MODS TEXT--
            "Archivos comprimidos (*.rar;*.zip;)|*.rar;*.zip;",
            "Archivos JAR  (*.jar)|*.jar",

            "Instalar desde un RAR/ZIP",
            "Instalar desde una carpeta",
            "Instalar desde un JAR",
            "Actualizar desde un RAR/ZIP",
            "Actualizar desde un JAR",

            "Aviso: Instalar borra todos los mods que hayan",
            "Actualizar simplemente pone los nuevos mods en la \r\n carpeta, sin borrar los otros",

        };
        string[] english = {
            //MODS TEXT--
            "Archive files (*.rar;*.zip;)|*.rar;*.zip;",
            "JAR files (*.jar)|*.jar",

            "Install from RAR/ZIP",
            "Install from folder",
            "Install from JAR",
            "Update from RAR/ZIP",
            "Update from JAR",

            "Warning: Install deletes all mods from the folder",
            "Update just puts the new mods in the folder, without \r\n deleting the old ones",


        };

        string[] currentLanguage;


        //LANGUAGE CHANGE-------------------------------------------------------------------
        private void spanishBttn_Click(object sender, EventArgs e)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string directoryPath = Path.Combine(appDataPath, "ModifyNow");
            Directory.CreateDirectory(directoryPath); // Create the directory if it doesn't exist

            string filePath = Path.Combine(directoryPath, "config.json");

            var jsonObject = new
            {
                language = "ES"
            };
            string jsonData = JsonSerializer.Serialize(jsonObject);

            File.WriteAllText(filePath, jsonData);
            currentLanguageText();

        }


        private void englishBttn_Click(object sender, EventArgs e)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string directoryPath = Path.Combine(appDataPath, "ModifyNow");
            Directory.CreateDirectory(directoryPath); // Create the directory if it doesn't exist

            string filePath = Path.Combine(directoryPath, "config.json");

            var jsonObject = new
            {
                language = "EN"
            };
            string jsonData = JsonSerializer.Serialize(jsonObject);

            File.WriteAllText(filePath, jsonData);
            currentLanguageText();
        }

        private void currentLanguageText()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filePath = Path.Combine(appDataPath + "//ModifyNow//", "config.json");

            string jsonData = File.ReadAllText(filePath);
            var jsonObject = JsonDocument.Parse(jsonData).RootElement;
            string languageValue = jsonObject.GetProperty("language").GetString();
            if (languageValue == "ES")
            {
                currentLanguage = spanish;
            }
            else
            {
                currentLanguage = english;
            }
            changeGUI();
        }

        private void changeGUI()
        {
            installRarZipBttn.Text = currentLanguage[2];
            installFromFolderBttn.Text = currentLanguage[3];
            installFromJarButton.Text = currentLanguage[4];
            updateFromCompressedBttn.Text = currentLanguage[5];
            updateFromJar.Text = currentLanguage[6];

            warningModsTxt.Text = currentLanguage[7];
            updateLabelTxt.Text = currentLanguage[8];

        }

        
    }
}



