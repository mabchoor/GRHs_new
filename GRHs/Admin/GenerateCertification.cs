using System;
using System.IO;
using System.Windows.Forms;
using word = Microsoft.Office.Interop.Word;

namespace GRHs.Admin
{
    public class GenerateCertification
    {
        private string[] wordsToReplace;
        private string[] newValues;
        private string employeeCin;

        public GenerateCertification(string[] wordsToReplace, string[] newValues ,string employeeCin)
        {
            this.wordsToReplace = wordsToReplace;
            this.newValues = newValues;
            this.employeeCin = employeeCin;
        }

        public void ReplaceWordsInWordFile()
        {
            // Define the absolute path to the template file
            string templateFilePath = @"D:\new project\GRHs\GRHs\GRHs\Assets\temp.docx";

            // Debugging output
            //MessageBox.Show("Template file path: " + templateFilePath);

            // Check if the template file exists
            if (!File.Exists(templateFilePath))
            {
                MessageBox.Show("Template file does not exist at: " + templateFilePath);
                return;
            }

            // Define the path for the copied file
            string newFilePath = Path.GetTempFileName() + ".docx"; // Temporary file path

            // Copy the template file to the new location
            try
            {
                File.Copy(templateFilePath, newFilePath, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to copy the file: " + ex.Message);
                return;
            }

            // Show the Save File dialog to let the user choose where to save the file
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Word Documents (*.docx)|*.docx";
                saveFileDialog.Title = "Save the Word Document";
                saveFileDialog.FileName = $"Attestation_{employeeCin}.docx"; // Use the employee CIN in the filename


                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string savePath = saveFileDialog.FileName;

                    // Move the copied file to the user-defined location
                    try
                    {
                        File.Move(newFilePath, savePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to save the file: " + ex.Message);
                        return;
                    }

                    // Open the Word document
                    word.Application wordApp = new word.Application();
                    word.Document wordDoc = null;

                    try
                    {
                        wordApp.Visible = true;
                        wordDoc = wordApp.Documents.Open(savePath);

                        // Replace the words in the Word document
                        for (int i = 0; i < wordsToReplace.Length; i++)
                        {
                            word.Find findObject = wordApp.Selection.Find;
                            findObject.ClearFormatting();
                            findObject.Text = wordsToReplace[i];
                            findObject.Replacement.ClearFormatting();
                            findObject.Replacement.Text = newValues[i];

                            object replaceAll = word.WdReplace.wdReplaceAll;
                            findObject.Execute(Replace: ref replaceAll);
                        }

                        // Save the changes
                        wordDoc.Save();
                        MessageBox.Show("Words replaced and document saved successfully.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while processing the document: " + ex.Message);
                    }
                    finally
                    {
                        if (wordDoc != null)
                        {
                            wordDoc.Close();
                        }
                        wordApp.Quit();
                    }
                }
            }
        }
    }
}
