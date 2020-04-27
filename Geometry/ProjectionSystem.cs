#region copyright
// Copyright (C) Daniel Fink 2019 - All Rights Reserved.
// Unauthorized copying of this file (via any medium) is strictly prohibited.
// Proprietary and confidential.
// Written by Daniel Fink <daniel@placeful.io>, 2018.
#endregion copyright
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using Rhino;
using Rhino.Geometry;
using Rhino.DocObjects;
using Rhino.Collections;

using ProjNet.CoordinateSystems;

using NetTopologySuite.Geometries;

namespace Antelope
{
    /// <summary>
    /// Records and classifies GIS Projection Systems and the geometry's source basis (local origin) 
    /// </summary>
    public class ProjectionSystem : IEquatable<ProjectionSystem>
    {
        public int SRID { get; private set; }
        public Plane SourceBasis { get; private set; }

        public enum EPSG
        {                    
            EPSG4326 = 4326,
            EPSG2263 = 2263,
            EPSG2249 = 2249
        }

        public ProjectionSystem(int srid, Plane sourceBasis)
        {
            this.SRID = srid;
            this.SourceBasis = sourceBasis;
        }

        public bool Equals(ProjectionSystem otherSystem)
        {
            var sridBool = this.SRID.Equals(otherSystem.SRID);
            var sourceBasisBool = this.SourceBasis.Equals(otherSystem.SourceBasis);

            if (sridBool == true && sourceBasisBool == true)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        public static ProjNet.CoordinateSystems.CoordinateSystem GetReferenceSystem(int srid)
        {
            var result = _refSystems.Find(system => system.EPSG == srid.ToString());
            var coordSystemFactory = new CoordinateSystemFactory();
            var coordSystem = coordSystemFactory.CreateFromWkt(result.WKT);

            return coordSystem as ProjNet.CoordinateSystems.CoordinateSystem;
        }

        private class ReferenceSystem
        {
            public string WKT { get; private set; }
            public string EPSG { get; private set; }

            public ReferenceSystem(string epsg, string wkt)
            {
                EPSG = epsg;
                WKT = wkt;
            }
        }

        private static List<ReferenceSystem> _refSystems = new List<ReferenceSystem> // These come from that website with geo references stuff... (TODO: make this comment better)
        {
            new ReferenceSystem("2263", "PROJCS[\"NAD83 / Massachusetts Mainland (ftUS)\",GEOGCS[\"NAD83\",DATUM[\"North_American_Datum_1983\",SPHEROID[\"GRS 1980\",6378137,298.257222101,AUTHORITY[\"EPSG\",\"7019\"]],TOWGS84[0,0,0,0,0,0,0],AUTHORITY[\"EPSG\",\"6269\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4269\"]],PROJECTION[\"Lambert_Conformal_Conic_2SP\"],PARAMETER[\"standard_parallel_1\",42.68333333333333],PARAMETER[\"standard_parallel_2\",41.71666666666667],PARAMETER[\"latitude_of_origin\",41],PARAMETER[\"central_meridian\",-71.5],PARAMETER[\"false_easting\",656166.667],PARAMETER[\"false_northing\",2460625],UNIT[\"US survey foot\",0.3048006096012192,AUTHORITY[\"EPSG\",\"9003\"]],AXIS[\"X\",EAST],AXIS[\"Y\",NORTH],AUTHORITY[\"EPSG\",\"2249\"]]"),
            new ReferenceSystem("2249", "PROJCS[\"NAD83 / New York Long Island (ftUS)\",GEOGCS[\"NAD83\",DATUM[\"North_American_Datum_1983\",SPHEROID[\"GRS 1980\",6378137,298.257222101,AUTHORITY[\"EPSG\",\"7019\"]],TOWGS84[0,0,0,0,0,0,0],AUTHORITY[\"EPSG\",\"6269\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4269\"]],PROJECTION[\"Lambert_Conformal_Conic_2SP\"],PARAMETER[\"standard_parallel_1\",41.03333333333333],PARAMETER[\"standard_parallel_2\",40.66666666666666],PARAMETER[\"latitude_of_origin\",40.16666666666666],PARAMETER[\"central_meridian\",-74],PARAMETER[\"false_easting\",984250.0000000002],PARAMETER[\"false_northing\",0],UNIT[\"US survey foot\",0.3048006096012192,AUTHORITY[\"EPSG\",\"9003\"]],AXIS[\"X\",EAST],AXIS[\"Y\",NORTH],AUTHORITY[\"EPSG\",\"2263\"]]"),
            new ReferenceSystem("4326", "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4326\"]]")
        };   

    }
}
