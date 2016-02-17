using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Subname
{
	public partial class frmMain : Form
	{
		public frmMain()
		{
			InitializeComponent();
		}

		private void frmMain_DragOver(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Move;
			else
				e.Effect = DragDropEffects.None;
		}

		private void frmMain_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);

				this.SyncName(paths);
			}
		}

        private static readonly string[] videos = { ".avi", ".wmv", ".wmp", ".wm", ".asf", ".mpg", ".mpeg", ".mpe", ".m1v", ".m2v", ".mpv2", ".mp2v", ".ts", ".tp", ".tpr", ".trp", ".vob", ".ifo", ".ogm", ".ogv", ".mp4", ".m4v", ".m4p", ".m4b", ".3gp", ".3gpp", ".3g2", ".3gp2", ".mkv", ".rm", ".ram", ".rmvb", ".rpm", ".flv", ".swf", ".mov", ".qt", ".amr", ".nsv", ".dpg", ".m2ts", ".m2t", ".mts", ".dvr-ms", ".k3g", ".skm", ".evo", ".nsr", ".amv", ".divx", ".webm", ".wtv", ".f4v" };
        private static readonly string[] subtitles = { ".ass", ".idx", ".smi", ".srt", ".ssa", ".sub" };
        public void SyncName(string[] paths)
        {
            var lstVideo	= new List<string>();
            var lstSub		= new List<string>();

            int i = 0;
            int j;
            string ext;

            for (i = 0; i < paths.Length; i++)
            {
                ext = Path.GetExtension(paths[i]).ToLower();

                if (subtitles.Contains(ext))
                    lstSub.Add(paths[i]);
                else if (videos.Contains(ext))
                    lstVideo.Add(paths[i]);
            }

            Comparison<string> sort = (x, y) => StringCompare(Path.GetFileName(x), Path.GetFileName(y));

            lstVideo.Sort(sort);
            lstSub.Sort(sort);

            j = Math.Min(lstVideo.Count, lstSub.Count);

            for (i = 0; i < j; i++)
            {
                if (Path.GetFileNameWithoutExtension(lstVideo[i]) == Path.GetFileNameWithoutExtension(lstSub[i])) continue;

                try
                {
                    File.Move(
                        lstSub[i],
                        Path.Combine(
                            Path.GetDirectoryName(lstSub[i]),
                            Path.GetFileNameWithoutExtension(lstVideo[i]) + Path.GetExtension(lstSub[i])
                        )
                    );
                }
                catch
                { }
            }
        }
        
        private static string GetPart(string str, ref int index, out bool isDigit)
        {
            // 문자열이면 문자열까지
            // 숫자면 숫자까지 가져옴

            isDigit = char.IsDigit(str[index]);

            var b = new StringBuilder();
            var c = '\0';
            
            do
            {
                c = str[index];
                if (isDigit != char.IsDigit(c)) break;

                index++;

                b.Append(c);
            } while (index < str.Length);

            return b.ToString();
        }
        private static int StringCompare(string x, string y)
        {
            if (string.IsNullOrEmpty(x) || string.IsNullOrEmpty(y)) return 0;

            int ix = 0, iy = 0;
            string sx, sy;
            int nx, ny;
            int r;
            bool dx, dy;


            while (ix < x.Length && iy < y.Length)
            {
                sx = GetPart(x, ref ix, out dx);
                sy = GetPart(y, ref iy, out dy);

                if (dx && dy)
                {
                    if (int.TryParse(sx, out nx) && int.TryParse(sy, out ny))
                        r = nx.CompareTo(ny);
                    else
                        r = sx.CompareTo(sy);
                }
                else
                {
                    r = sx.CompareTo(sy);
                }

                if (r != 0) return r;
            }

            return x.Length - y.Length;
        }
	}
}
