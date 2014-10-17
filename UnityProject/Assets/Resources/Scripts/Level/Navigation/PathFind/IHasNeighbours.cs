using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


    public interface IHasNeighbours<T>
    {
        IEnumerable<T> Neighbours { get; set; }
    }

