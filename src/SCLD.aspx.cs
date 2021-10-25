using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;
using System.Web.UI.HtmlControls;

namespace SCBasics.Tools
{
    public partial class SCLD : System.Web.UI.Page
    {
        
        private MemoryStream memoryStream;

        string LOGFOLDERPATH = (Sitecore.Configuration.Settings.DataFolder.StartsWith("/App_Data")) ?
                    System.Web.HttpContext.Current.Server.MapPath(Sitecore.Configuration.Settings.DataFolder + "/logs") :
                            Sitecore.Configuration.Settings.DataFolder + "/logs";

        StringBuilder messageToPrint = null;

        private const string AUTH_KEY = "c9f1eab8b35944a8aa297277b9667758"; // Replace with a new unique key generated from https://www.uuidgenerator.net/

        protected void Page_Init(object sender, EventArgs e)
        {
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Request.QueryString["authkey"] == AUTH_KEY)
                {
                    if (!IsPostBack)                    
                        LoadInstanceLogFolders();
                    LoadLogFilesList();
                }
                else
                {                    
                    HttpContext.Current.Response.Write("Authentication failed, please provide valid authkey.");                    
                    HttpContext.Current.Response.End();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Visible = true;
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Error occured : " + ex.Message;
            }

        }
        
        protected void btnLoadCurrentInstance_Click(object sender, EventArgs e)
        {
            ddllogFileType.ClearSelection();
            ddllogFileType.Items.FindByText(System.Environment.MachineName).Selected = true;
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            System.Collections.Generic.List<string> fileNames = new System.Collections.Generic.List<string>();

            try
            {
                bool isFileSelected = false;
                foreach (TableRow tableRow in tblData.Rows)
                {
                    if (!(tableRow is TableHeaderRow))
                    {
                        CheckBox foundCheckBox = tableRow.Cells[0].Controls[0] as CheckBox;
                        if (foundCheckBox != null)
                        {
                            if (foundCheckBox.Checked)
                            {
                                isFileSelected = true;
                                string fullFileName = ddllogFileType.Items[ddllogFileType.SelectedIndex].Value + "\\" + tableRow.Cells[1].Text;
                                // If the CheckBox is checked, then add the Filename to the StringBuilder
                                fileNames.Add(fullFileName);
                            }
                        }
                    }

                }

                if (isFileSelected)                    
                    CreateandSendZipFile(fileNames);
                else
                    lblMessage.Text = "Please select at least one file to download.";

            }
            catch (Exception ex)
            {
                lblMessage.Visible = true;
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Something went wrong : " + ex.Message;
            }

        }
        
        protected void logFileType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Page load handles
        }


        private string LoadFiles(string selectedFolderPath)
        {
            string result = String.Empty;
            if (Directory.Exists(selectedFolderPath))
            {
                TableRow tr;
                TableCell td;

                TableHeaderRow thr;
                TableHeaderCell thd;

                int totalFilesCount;

                thr = new TableHeaderRow();
                tblData.Rows.Add(thr);
                thd = new TableHeaderCell();
                thd.Text = "Select";
                thr.Cells.Add(thd);

                //thr = new TableHeaderRow();
                //tblData.Rows.Add(thr);
                thd = new TableHeaderCell();
                thd.Text = "Name";
                thr.Cells.Add(thd);

                //thr = new TableHeaderRow();
                //tblData.Rows.Add(thr);
                thd = new TableHeaderCell();
                thd.Text = "Size";
                thr.Cells.Add(thd);

                //thr = new TableHeaderRow();
                //tblData.Rows.Add(thr);
                thd = new TableHeaderCell();
                thd.Text = "Date Created";
                thr.Cells.Add(thd);

                totalFilesCount = 0;


                System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(selectedFolderPath);
                System.Collections.Generic.List<System.IO.FileSystemInfo> listOfFiles =
                    new System.Collections.Generic.List<System.IO.FileSystemInfo>(directoryInfo.GetFileSystemInfos());
                
                listOfFiles.Sort(delegate (System.IO.FileSystemInfo aFInfo, System.IO.FileSystemInfo bFInfo)
                { return bFInfo.CreationTime.CompareTo(aFInfo.CreationTime); });

                foreach (System.IO.FileSystemInfo logFile in listOfFiles)
                {
                    if (logFile.GetType().Name == "FileInfo")
                    {
                        totalFilesCount++;

                        System.IO.FileInfo fileInfo = new System.IO.FileInfo(logFile.FullName);

                        tr = new TableRow();
                        tblData.Rows.Add(tr);

                        // CHECKBOX
                        td = new TableCell();
                        td.HorizontalAlign = HorizontalAlign.Center;
                        CheckBox chkSelect = new CheckBox();
                        //chkSelect.ID = "chkSelect" + fileInfo.Name.Replace(fileInfo.Extension, string.Empty).Replace("-", string.Empty);
                        chkSelect.ID = "chkSelect" + fileInfo.Name;
                        td.Controls.Add(chkSelect);
                        tr.Cells.Add(td);

                        td = new TableCell();
                        td.Text = fileInfo.Name;
                        tr.Cells.Add(td);

                        td = new TableCell();
                        td.Text = Sitecore.MainUtil.FormatSize(fileInfo.Length);
                        tr.Cells.Add(td);

                        td = new TableCell();
                        //td.Text = fileInfo.LastWriteTime.ToString();
                        td.Text = fileInfo.CreationTime.ToString();
                        tr.Cells.Add(td);
                    }

                }


                if (totalFilesCount == 0)
                {
                    result = string.Format("Sorry, No files found.");
                }
                else
                {
                    result = string.Format("Log file(s) loaded from : <strong> {0} </strong>", selectedFolderPath);
                }
            }
            else
            {
                btnDownloadZip.Enabled = false;
                result = "Selected directory does not exist, Can you please check that selected directory exists physically? (If you don't have physical access. Please contact admin.). Path : " + selectedFolderPath;
            }

            return result;

        }

        private void LoadLogFilesList()
        {
            messageToPrint = new StringBuilder();
            if (ddllogFileType.SelectedIndex > 0)
            {
                var fileLoadResult = LoadFiles(ddllogFileType.Items[ddllogFileType.SelectedIndex].Value);
                messageToPrint.AppendFormat(fileLoadResult + "<br/>");
            }
            else
            {
                // Load files from Data folder
                var fileLoadResult = LoadFiles(LOGFOLDERPATH);
                messageToPrint.AppendFormat(fileLoadResult + "<br/>");
            }

            // Print which ARRAffinity your server is and Machine name and select that machine name folder and load it
            messageToPrint.AppendFormat("Current Instance Name : <strong>{0}</strong> <br/>", System.Environment.MachineName);
            var cookieValue = (System.Web.HttpContext.Current.Request.Cookies["ARRAffinity"] != null) ? System.Web.HttpContext.Current.Request.Cookies["ARRAffinity"].Value : "NA";
            messageToPrint.AppendFormat("ARRAffinity Value : <strong>{0}</strong> <br/>", cookieValue);
            lblMessage.Text = messageToPrint.ToString();
            lblMessage.Visible = true;
            btnDownloadZip.Enabled = true;
        }

        private void LoadInstanceLogFolders()
        {
            ddllogFileType.Items.Add(new ListItem("Select Instance Log Folder", LOGFOLDERPATH));
            string[] directories = System.IO.Directory.GetDirectories(LOGFOLDERPATH);
            foreach (var aDirectory in directories)
            {
                System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(aDirectory);
                ddllogFileType.Items.Add(new ListItem(directoryInfo.Name, aDirectory));
            }
        }

        private void CreateandSendZipFile(System.Collections.Generic.List<string> fileNames)
        {

            //Create memory stream.

            ////Creates Zip file.
            CreateZipFile(fileNames);

            Response.Clear();
            Response.ClearHeaders();
            HttpContext.Current.Response.CacheControl = "public";
            HttpContext.Current.Response.ContentType = "application/octet-stream";


            if (this.memoryStream.Length > 0 && fileNames != null)
            {
                //Setting length
                if (fileNames != null)
                    Response.AddHeader("Content-Length", this.memoryStream.Length.ToString());
                else
                    Response.AddHeader("Content-Length", string.Empty.Length.ToString());

                //the filename might be dynamic as well.
                Response.AddHeader("Content-disposition",
                     "attachment; filename=" + ddllogFileType.SelectedItem.Text.Replace(" ", "_") + "_"
                     + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".zip; size="
                     + this.memoryStream.Length.ToString());
                Response.Flush();

                //one line of code to send back the stream.  Sweet!
                Response.BinaryWrite(this.memoryStream.ToArray());

                try
                {
                    HttpContext.Current.Response.End();
                }
                catch (System.Threading.ThreadAbortException)
                {
                    //ignore thread abort exception
                    //Added to Fix Backlog item # 572
                }
                //now i close the stream and dispose of it.
                //lblDownloadCheck.Text = "";
                //lblDownloadCheck.Visible = false;
                this.memoryStream.Close();
                this.memoryStream.Dispose();
                //HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            else
            {
                lblMessage.Visible = true;
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Sorry, an error occured while downloading zip.";
            }
        }

        private MemoryStream CreateZipFile(System.Collections.Generic.List<string> filesToZip)
        {

            this.memoryStream = null;
            this.memoryStream = new MemoryStream();

            ICSharpCode.SharpZipLib.Zip.ZipOutputStream zipStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(this.memoryStream);
            zipStream.SetLevel(9); // 0 - store only to 9 - means best compression
            if (filesToZip != null)
            {
                for (int i = 0; i < filesToZip.Count; i++)
                {

                    if (filesToZip[i] == null) continue;

                    // FULL PATH
                    string fileName = filesToZip[i];

                    // each zipentry is one file in the zip archive
                    string singleFileName = Path.GetFileName(fileName);

                    // 
                    ICSharpCode.SharpZipLib.Zip.ZipEntry entry =
                        new ICSharpCode.SharpZipLib.Zip.ZipEntry(singleFileName);
                    entry.DateTime = DateTime.Now;

                    // Read in the files one by one.
                    try
                    {
                        using (FileStream fileStream = File.Open(filesToZip[i], FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {

                            byte[] buffer = new byte[fileStream.Length];
                            fileStream.Read(buffer, 0, buffer.Length);

                            // set Size and the crc, because the information
                            // about the size and crc should be stored in the header
                            // if it is not set it is automatically written in the footer.
                            // (in this case size == crc == -1 in the header)
                            // Some ZIP programs have problems with zip files that don't store
                            // the size and crc in the header.
                            entry.Size = fileStream.Length;
                            fileStream.Close();


                            //add the zip entry to the stream.
                            zipStream.PutNextEntry(entry);
                            zipStream.Write(buffer, 0, buffer.Length);
                        }
                    }
                    catch (Exception ex)
                    {
                        lblMessage.Visible = true;
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                        lblMessage.Text = "Error occured while zipping your file : " + ex.Message;
                    }
                }
            }

            zipStream.Finish();
            zipStream.Flush();
            this.memoryStream.Flush();

            //the original code closed the stream after saving it to disk.  Instead, I 
            //return the stream so i can send it to the browser
            //zipStream.Close();

            return this.memoryStream;
        }
    }
}