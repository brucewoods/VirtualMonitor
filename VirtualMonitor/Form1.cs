using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VirtualMonitor
{
    public partial class Form1 : Form
    {
        #region Member Variables
        string dragItemTempFileName = string.Empty;
        private bool itemDragStart = false;
        string executingFolder = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
        string path;
        string thumbnail;
        #endregion
        public Form1()
        {
            InitializeComponent();

            
            listView1.View = View.Details;
            listView1.View = View.LargeIcon;
            Icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);

              path = System.IO.Path.Combine(executingFolder, "av\\");
            thumbnail = Path.Combine(path, ".thumbnails\\");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            if (!Directory.Exists(thumbnail)) Directory.CreateDirectory(thumbnail);

        }

        
        private void showFiles()
		{
            ListViewItem li;
            
          
            if(listView1.Items!=null)
           listView1.Items.Clear();
            if(listView1.LargeImageList!=null)
           listView1.LargeImageList.Images.Clear(); 
            
                try
                {
                   fileSystemWatcher1.Path = path;
                
                    String[] ss = Directory.GetFiles(path);
              if(listView1.LargeImageList==null)  listView1.LargeImageList = new ImageList();
                // var s = Image.FromFile("E:\\dws\\TV\\img\\2.jpg");
                Image e = null;
                //listView1.LargeImageList.Images.Add(e);
                listView1.LargeImageList.ImageSize = new Size(130, 90);
                for (int i=0;i<ss.Length;i++)
                {

                    lock (this)
                    {
                        ShellFile shellFile = ShellFile.FromFilePath(ss[i]);

                        Bitmap bm = shellFile.Thumbnail.Bitmap;
                        Image mge = (Image)bm;
                        // var t = thumbnail + str.Split('\\').Last().Split('.').First() + ".jpg";
                        //if(!File.Exists(t)) bm.Save(t,System.Drawing.Imaging.ImageFormat.Jpeg);
                     

                        listView1.LargeImageList.Images.Add(mge);

                   
                        //listView1.SmallImageList.Images.Add(new Icon("E:\\dws\\TV\\img\\1.jpg"));

                    }
                    li = new ListViewItem(Path.GetFileName(ss[i]),i);
                    li.Tag = ss[i];
                    FileInfo fi = new FileInfo(ss[i]);
    
                   li.SubItems.Add((fi.Length / 1024).ToString()); 
                        li.SubItems.Add(ss[i]);
                        li.SubItems.Add("2dd");
                    li.SubItems.Add(File.GetCreationTime(ss[i]).ToLongDateString());
                         
                        this.listView1.Items.Add(li);
                    }
                
                }
                catch
                { }
            }
     

        private void Form1_Load(object sender, EventArgs e)
		{
            showFiles();
            
        }

		private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
		{

		}

		private void listView1_DragDrop(object sender, DragEventArgs e)
		{

            MessageBox.Show("a");
		}

		private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
		{
           // MessageBox.Show("!");
		}

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {

            //Cears the Drag Data
           ClearDragData();
            if (e.Button == MouseButtons.Left && listView1.SelectedItems.Count > 0)
            {
                if (listView1.SelectedItems[0].SubItems==null ) return;

                Program.objDragItem = listView1.SelectedItems[0].SubItems[2].Text;
                itemDragStart = true;
            }
            //MessageBox.Show("md");
           // DataObject d = new DataObject();
            //d.SetData(DataFormats.FileDrop, "E:\\dws\\TV\\img\\1.jpg");
           // listView1.DoDragDrop(d, DragDropEffects.Copy|DragDropEffects.Move);

        
        }

        DateTime lastTimeClick
           ;
        int count = 0;
        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            lastTimeClick = DateTime.Now;

            var span = DateTime.Now - lastTimeClick;
            if (span.Milliseconds > 1000) return;
            if (e.Button != MouseButtons.Left || listView1.SelectedItems.Count == 0) return;
            count++;
            if (count >= 2)
            {
                if (listView1.SelectedItems[0].SubItems==null) return;
                // openWithDefaultProgram(listView1.SelectedItems[0].SubItems[2].Text);
                playVideo(listView1.SelectedItems[0].SubItems[2].Text);
                count = 0;
            }

        }
        private void playVideo(string path) {
            Player p = new Player();
            p.Visible = true;
            p.Show();
            p.Text = Path.GetFileName(path);
            var mp = (AxWMPLib.AxWindowsMediaPlayer)p.Controls[0];
            mp.URL = path;
            mp.settings.autoStart = true;
          
        }
        private  void openWithDefaultProgram(string path)
        {
              Process fileopener = new Process();

            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + path + "\"";
            fileopener.Start();
        }
        #region DragMethods
        private void ClearDragData()
        {
            try
            {
                if (File.Exists(dragItemTempFileName))
                    File.Delete(dragItemTempFileName);
                Program.objDragItem = null;
                dragItemTempFileName = string.Empty;
                itemDragStart = false;
                Program.ClearFileWatchers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DragNDrop Error1", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
		#endregion

		private void listView1_MouseMove(object sender, MouseEventArgs e)
		{
            //         MessageBox.Show("up");
            //         var s = new System.Collections.Specialized.StringCollection();
            //         foreach (string  d in s)
            //{
            //             MessageBox.Show(d);
            //}


            if (e.Button == MouseButtons.None)
                return;
            if (itemDragStart && Program.objDragItem != null)
            {
                dragItemTempFileName =  string.Format("{0}{1}{2}.tmp", Path.GetTempPath(), Program.DRAG_SOURCE_PREFIX, listView1.SelectedItems[0].Text);
                try
                {
                    Util.CreateDragItemTempFile(dragItemTempFileName);

                    string[] fileList = new string[] { dragItemTempFileName };
                    DataObject fileDragData = new DataObject(DataFormats.FileDrop, fileList);
                    DoDragDrop(fileDragData, DragDropEffects.Move);

                    ClearDragData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "DragNDrop Error2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

		private void listView1_DoubleClick(object sender, EventArgs e)
		{
            
            openWithDefaultProgram(listView1.SelectedItems[0].SubItems[2].Text);
        }

		private void toolStripStatusLabel1_Click(object sender, EventArgs e)
		{

		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
            
		}
	}


}
