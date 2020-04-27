#region copyright
// Copyright (C) Daniel Fink 2019 - All Rights Reserved.
// Unauthorized copying of this file (via any medium) is strictly prohibited.
// Proprietary and confidential.
// Written by Daniel Fink <daniel@placeful.io>, 2018.
#endregion copyright
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper;
using Grasshopper.Kernel.Types;

using Rhino.Geometry;

namespace Antelope
{
    /// <summary>
    /// Base interface for all Geometry that have geospatial information via recording a Projection System
    /// </summary>
    public interface IGeospatialGeom
    {
        ProjectionSystem ProjectionSystem { get; }
        //GeometryBase Geometry { get; }
    }


}
