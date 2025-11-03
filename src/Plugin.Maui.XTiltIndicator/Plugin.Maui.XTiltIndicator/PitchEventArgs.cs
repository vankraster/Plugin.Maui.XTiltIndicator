using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Maui.XTiltIndicator
{
    public class PitchEventArgs: EventArgs
    {
        public double PitchLeft {  get; set; }  
        public double PitchRight {  get; set; }  
        public double PitchHorizontal {  get; set; }  
        public Color PitchHorizontalColor {  get; set; }  
    }
}
