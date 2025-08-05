﻿using IT.Tangdao.Framework.DaoIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoEvents
{
    public class RouteChangedEventArgs : EventArgs
    {
        public ITangdaoPage PreviousPage { get; }
        public ITangdaoPage NewPage { get; }
        public object Parameters { get; }

        public RouteChangedEventArgs(ITangdaoPage previous, ITangdaoPage newPage, object parameters = null)
        {
            PreviousPage = previous;
            NewPage = newPage;
            Parameters = parameters;
        }
    }
}