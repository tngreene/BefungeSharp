using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungePF
{
    interface BoardComponent
    {
        void Draw(BoardMode mode);
        void ClearArea(BoardMode mode);
        void Update(BoardMode mode);
    }
}
