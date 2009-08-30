using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;
using StiLib.Core;

namespace DisplayCal
{
    class CalControl : SLGDControl
    {
        public Vector3 calColor;


        protected override void Initialize()
        {
            // Hook the idle event to constantly redraw.
            Application.Idle += delegate { Invalidate(); };
        }

        protected override void Update()
        {
        }

        protected override void Draw()
        {
            GraphicsDevice.Clear(new Color(calColor));
        }

    }
}
