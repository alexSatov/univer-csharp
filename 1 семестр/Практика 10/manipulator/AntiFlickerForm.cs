using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Manipulator
{
    public class AntiFlickerForm : Form
    {
        public AntiFlickerForm()
        {
			//Включает механизм двойной буферизации вывода, который предотвращает мерцание формы при перерисовке
            DoubleBuffered = true;
        }
    }
}
