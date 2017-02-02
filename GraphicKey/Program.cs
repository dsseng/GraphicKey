using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFML;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Threading;

namespace GraphicKey
{
    class Program
    {     
        // Graphic window
        static RenderWindow rw;
        
        // Overlay for gestures displaying
        static RenderTexture rt;

        //If it's true, some gesture is capturing now
        static bool isCapturing = false;

        // Current capturing gesture
        static List<Vector2i> gesture = new List<Vector2i>();

        // Gestures, that contains in password
        static List<List<Vector2i>> passwordGestures = new List<List<Vector2i>>();

        // Captured gestures
        static List<List<Vector2i>> capturedGestures = new List<List<Vector2i>>();

        // Settings
        #region Settings
        // Display gestures?
        static bool displayGestures = true;
        
        // Minimal distance between vertex and another vertex
        static int ed = 3;

        // Maximal gesture lenght difference
        static int mgld = 3;

        // Maximal distance between vertex of password and input vertex
        static int dd = 20;
        #endregion

        static void Main(string[] args)
        {
            Texture t;
            try
            {
                t = new Texture(System.IO.Directory.GetFiles("./", "*.jpg")[0]);
            } catch
            {
                MessageBox.Show("Please place any *.jpg picture in current directory!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            RectangleShape s = new RectangleShape(new Vector2f(1138, 640));
            s.Texture = t;

            rt = new RenderTexture(1138, 640);
            Sprite ov = new Sprite(rt.Texture);

            rw = new RenderWindow(new VideoMode(1138, 640), "Identification");
            rw.MouseButtonPressed += Rw_MouseButtonPressed;
            rw.MouseButtonReleased += Rw_MouseButtonReleased;
            rw.MouseMoved += Rw_MouseMoved;
            rw.Closed += Rw_Closed;
            
            while (rw.IsOpen)
            {
                rw.DispatchEvents();
                rw.Clear();
                rw.Draw(s);
                rw.Draw(ov);
                rw.Display();
            }
        }
        
        private static void GestureCaptured(List<Vector2i> lGesture) {
            capturedGestures.Add(lGesture);
            if (displayGestures)
            {
                DrawGesture(lGesture);
            }
        }

        private static void Rw_Closed(object sender, EventArgs e)
        {
            rw.Close();
        }

        private static void Rw_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            if(isCapturing)
            {
                Vector2i mc = Mouse.GetPosition(rw);
                if (gesture.Count <= 0) {

                }
                else
                {
                    if (InRange(mc.X, gesture.Last().X - ed, gesture.Last().X + ed) && InRange(mc.Y, gesture.Last().Y - ed, gesture.Last().Y + ed)) return;
                }
                gesture.Add(mc);
            }
        }

        private static void Rw_MouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
            {
                isCapturing = false;
                GestureCaptured(gesture);
            }
        }

        private static void Rw_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            switch (e.Button) {
                case Mouse.Button.Middle: MessageBox.Show(TestGestures().ToString()); capturedGestures = new List<List<Vector2i>>(); rt.Clear(Color.Transparent); break;
                case Mouse.Button.Left: gesture = new List<Vector2i>(); isCapturing = true; gesture.Add(Mouse.GetPosition(rw)); break;
                case Mouse.Button.Right: passwordGestures = capturedGestures; rt.Clear(Color.Transparent); break;
                default: gesture = new List<Vector2i>(); isCapturing = true; break;
            }
        }

        private static bool InRange(int x, int min, int max)
        {
            if (x <= max && x >= min)
            {
                return true;
            }
            return false;
        }
        
        private static bool CompareGestures(List<Vector2i> a, List<Vector2i> b)
        {
            if(InRange(a.Count, b.Count - mgld, b.Count))
            {
                for(int i = 0; i < a.Count; i++)
                {
                    if (InRange(a[i].X, b[i].X - dd, b[i].X + dd) && InRange(a[i].Y, b[i].Y - dd, b[i].Y + dd))
                        continue;
                    else
                        return false;
                }
                return true;
            }
            else if (InRange(b.Count, a.Count - mgld, a.Count))
            {
                for (int i = 0; i < b.Count; i++)
                {
                    if (InRange(b[i].X, a[i].X - dd, a[i].X + dd) && InRange(b[i].Y, a[i].Y - dd, a[i].Y + dd))
                        continue;
                    else
                        return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool TestGestures()
        {
            if (capturedGestures.Count == passwordGestures.Count)
            {
                for (int i = 0; i < capturedGestures.Count; i++)
                {
                    if (CompareGestures(capturedGestures[i], passwordGestures[i]))
                        continue;
                    else
                        return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void DrawGesture(List<Vector2i> gesture)
        {
            foreach (Vector2i x in gesture)
            {
                CircleShape cs = new CircleShape(10);
                cs.Origin = new Vector2f(5, 5);
                cs.Position = new Vector2f(x.X, 640 - x.Y);
                cs.FillColor = new Color(255, 255, 255, 127);

                rt.Draw(cs);
            }
        }
    }
}
