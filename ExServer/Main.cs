using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StiLib.Core;
using StiLib.Vision;
using System.ServiceModel;

namespace ExServer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class Server : ExService
    {
        private Text text;
        private string hr = "";


        public Server()
            : base(1, 400, 200, 0, true, true, true, false, Vector3.One)
        {
            text = new Text(GraphicsDevice, Services, "Content", "Arial");
            this.Text = "ExServer";
        }


        public override string Invoke(string ex)
        {
            var h = base.Invoke(ex);
            if (string.IsNullOrEmpty(h))
            {
                hr = ex + "    Invoked !";
            }
            else
            {
                hr = h;
            }
            return h;
        }

        public override string Terminate(string ex)
        {
            var h = base.Terminate(ex);
            if (string.IsNullOrEmpty(h))
            {
                hr = ex + "    Terminated !";
            }
            else
            {
                hr = h;
            }
            return h;
        }

        protected override void Draw()
        {
            GraphicsDevice.Clear(Color.SlateGray);
            text.Draw(new Vector2(10, 10), "StiLib Experiment Server Started !" + "\n\n\n" + hr, Color.Gold);
        }

    }

}
