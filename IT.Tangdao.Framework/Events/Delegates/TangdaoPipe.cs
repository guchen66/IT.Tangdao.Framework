using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Events
{
    public delegate void TangdaoPipe<in T>(T msg);
}