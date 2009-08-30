using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;
using MediaFoundation.Misc;
using StiLib.Core;

namespace SLControl
{
    /// <summary>
    /// A StiLib Media Player Control
    /// </summary>
    public partial class MediaControl : UserControl
    {
        const int WM_PAINT = 0x000F;
        const int WM_SIZE = 0x0005;
        const int WM_ERASEBKGND = 0x0014;
        const int WM_CHAR = 0x0102;
        const int WM_SETCURSOR = 0x0020;
        const int WM_APP = 0x8000;
        const int WM_APP_NOTIFY = WM_APP + 1;   // wparam = state
        const int WM_APP_ERROR = WM_APP + 2;    // wparam = HRESULT
        Media MFPlayer;
        bool g_bRepaintClient = true;


        public MediaControl()
        {
            InitializeComponent();
            MFPlayer = new Media(this.Handle, this.Handle);
        }
        ~MediaControl()
        {
            Cursor.Current = Cursors.Default;

            if (MFPlayer != null)
            {
                MFPlayer.Shutdown();
                MFPlayer = null;
            }
        }


        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_PAINT:
                    OnPaint(m.HWnd);
                    base.WndProc(ref m);
                    break;

                case WM_SIZE:
                    if (MFPlayer != null)
                    {
                        MFPlayer.ResizeVideo((short)(m.LParam.ToInt32() & 65535), (short)(m.LParam.ToInt32() >> 16));
                    }
                    break;

                case WM_CHAR:
                    OnKeyPress(m.WParam.ToInt32());
                    break;

                case WM_SETCURSOR:
                    m.Result = new IntPtr(1);
                    break;

                case WM_APP_NOTIFY:
                    UpdateUI(m.HWnd, (MediaState)m.WParam);
                    break;

                case WM_APP_ERROR:
                    NotifyError(m.HWnd, "An Error Occurred !", (int)m.WParam);
                    UpdateUI(m.HWnd, MediaState.Ready);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        void OnPaint(IntPtr hwnd)
        {
            if (!g_bRepaintClient)
            {
                // Video is playing. Ask the player to repaint.
                MFPlayer.Repaint();
            }
        }

        void OnKeyPress(int key)
        {
            switch (key)
            {
                // Space key toggles between running and paused
                case 0x20:
                    if (MFPlayer.GetState() == MediaState.Started)
                    {
                        MFPlayer.Pause();
                    }
                    else if (MFPlayer.GetState() == MediaState.Paused)
                    {
                        MFPlayer.Play();
                    }
                    break;
            }
        }

        void UpdateUI(IntPtr hwnd, MediaState state)
        {
            bool bWaiting = false;
            bool bPlayback = false;

            Debug.Assert(MFPlayer != null);

            switch (state)
            {
                case MediaState.OpenPending:
                    bWaiting = true;
                    break;

                case MediaState.Started:
                    bPlayback = true;
                    break;

                case MediaState.Paused:
                    bPlayback = true;
                    break;

                case MediaState.PausePending:
                    bWaiting = true;
                    bPlayback = true;
                    break;

                case MediaState.StartPending:
                    bWaiting = true;
                    bPlayback = true;
                    break;
            }

            bool uEnable = !bWaiting;

            openFileToolStripMenuItem.Enabled = uEnable;
            openUrlToolStripMenuItem.Enabled = uEnable;

            if (bWaiting)
            {
                Cursor.Current = Cursors.WaitCursor;
            }
            else
            {
                Cursor.Current = Cursors.Default;
            }

            if (bPlayback && MFPlayer.HasVideo())
            {
                g_bRepaintClient = false;
            }
            else
            {
                g_bRepaintClient = true;
            }
        }

        void NotifyError(IntPtr hwnd, string sErrorMessage, int hrErr)
        {
            string s = string.Format("{0} (HRESULT = 0x{1:x} {2})", sErrorMessage, hrErr, MFError.GetErrorText(hrErr));

            MessageBox.Show(this, s, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int hr = 0;

            openFileDialog.Filter = "All files|*.*|Windows Media|*.wmv;*.wma;*.asf|Wave|*.wav|MP3|*.mp3";

            // File dialog windows must be on STA threads.  ByteStream handlers are happier if
            // they are opened on MTA.  So, the application stays MTA and we call OpenFileDialog
            // on its own thread.
            Invoker I = new Invoker(openFileDialog);

            // Show the File Open dialog.
            if (I.Invoke() == DialogResult.OK)
            {
                // Open the file with the playback object.
                hr = MFPlayer.OpenURL(openFileDialog.FileName);

                if (hr >= 0)
                {
                    UpdateUI(this.Handle, MediaState.OpenPending);
                }
                else
                {
                    NotifyError(this.Handle, "Could Not Open The File.", hr);
                    UpdateUI(this.Handle, MediaState.Ready);
                }
            }
        }

        private void openUrlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int hr;
            URLForm f = new URLForm();

            if (f.ShowDialog(this) == DialogResult.OK)
            {
                // Open the file with the playback object.
                hr = MFPlayer.OpenURL(f.tbURL.Text);

                if (hr >= 0)
                {
                    UpdateUI(this.Handle, MediaState.OpenPending);
                }
                else
                {
                    NotifyError(this.Handle, "Could Not Open This URL.", hr);
                    UpdateUI(this.Handle, MediaState.Ready);
                }
            }
        }

    }

    /// <summary>
    /// Opens a specified FileOpenDialog box on an STA thread
    /// </summary>
    public class Invoker
    {
        private OpenFileDialog m_Dialog;
        private DialogResult m_InvokeResult;
        private Thread m_InvokeThread;


        // Constructor is passed the dialog to use
        public Invoker(OpenFileDialog Dialog)
        {
            m_InvokeResult = DialogResult.None;
            m_Dialog = Dialog;

            // No reason to waste a thread if we aren't MTA
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.MTA)
            {
                m_InvokeThread = new Thread(new ThreadStart(InvokeMethod));
                m_InvokeThread.SetApartmentState(ApartmentState.STA);
            }
            else
            {
                m_InvokeThread = null;
            }
        }


        // Start the thread and get the result
        public DialogResult Invoke()
        {
            if (m_InvokeThread != null)
            {
                m_InvokeThread.Start();
                m_InvokeThread.Join();
            }
            else
            {
                m_InvokeResult = m_Dialog.ShowDialog();
            }

            return m_InvokeResult;
        }

        // The thread entry point
        private void InvokeMethod()
        {
            m_InvokeResult = m_Dialog.ShowDialog();
        }

    }

}
