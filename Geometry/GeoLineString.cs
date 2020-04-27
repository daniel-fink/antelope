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
using Rhino.Geometry;
using ProjNet.CoordinateSystems;

using Grasshopper;
using Grasshopper.Kernel.Types;

using NetTopologySuite.Geometries;

namespace Antelope
{
    /// <summary>
    /// A PolyChain with geospatial positioning information.
    /// </summary>
    [Serializable]
    public class GeoLineString : PolylineCurve, IGeospatialGeom
    {
        public ProjectionSystem ProjectionSystem { get; private set; }
        
        public GeoLineString(Polyline polyline, ProjectionSystem sourceProjection) : base(polyline)
        {
            if (polyline.IsValid)
            {
                this.ProjectionSystem = sourceProjection;
                //this.Value = new PolylineCurve(polyline);
                //this.Geometry = this.GetGeometry();
            }

            else
            {
                throw new ArgumentException("Polyline is not valid", nameof(polyline));
            }
        }

        public GeoLineString(LineString lineString, Plane sourceBasis) : base(lineString.ToPolyline())
        {
            this.ProjectionSystem = new ProjectionSystem(lineString.SRID, sourceBasis);
            //this.Value = lineString.ToPolyline().ToPolylineCurve();
            //this.Geometry = this.GetGeometry();
        }

        // public GeoPoint[] GetCorners()
        // {
        //     return this.As<PolylineCurve>().ToPolyline()
        //         .Select(point => new GeoPoint(point, this.ProjectionSystem))
        //         .ToArray();
        // }

        private Transform TargetTransform()
        {
            return Rhino.Geometry.Transform.ChangeBasis(Plane.WorldXY, this.ProjectionSystem.SourceBasis);
        }

        /// <summary>
        /// Transforms a GeoLineString from its source basis to Rhino's origin basis (target).
        /// </summary>
        public void ToTargetBasis()
        {
            this.Transform(this.TargetTransform());
        }

        /// <summary>
        /// Transforms an GeoLineString from its target basis to its source basis.
        /// </summary>
        public void ToSourceBasis()
        {
            Transform inverseTransform;
            this.TargetTransform().TryGetInverse(out inverseTransform);
            this.Transform(inverseTransform);
        }
    }

    /// <summary>
    /// Multiple PolyChains with geospatial positioning information.
    /// </summary>
    // [Serializable]
    // public class MultiGeoLineString : GH_GeometryGroup, IGeospatialGeom
    // {
    //     public ProjectionSystem ProjectionSystem { get; private set; }
    //     public GeometryBase[] Geometries
    //     {
    //         get
    //         {
    //             return this.GetGeometries();
    //         }
    //         private set
    //         {
    //
    //         }
    //     }
    //
    //     private GeometryBase[] GetGeometries()
    //     {
    //         var geoms = this.Objects.Select(geom => (GeoLineString)geom);
    //         return geoms.Select(geom => (GeometryBase)geom).ToArray();
    //     }
    //
    //     public void Add(GeoLineString geoLineString)
    //     {
    //         this.Objects.Add(geoLineString);
    //     }
    //
    //     public MultiGeoLineString(IEnumerable<GeoLineString> geoLineStrings)
    //     {
    //         foreach (var geoLineString in geoLineStrings)
    //         {
    //             this.Objects.Add(geoLineString);
    //         }
    //         var projSystems = geoLineStrings.Select(ls => ls.ProjectionSystem);
    //         this.ProjectionSystem = projSystems.Condense<ProjectionSystem>();
    //         this.Geometries = this.GetGeometries();
    //     }
    //
    //     public MultiGeoLineString(IEnumerable<Polyline> polylines, ProjectionSystem sourceProjection)
    //     {
    //         foreach (var polyline in polylines)
    //         {
    //             this.Objects.Add(new GeoLineString(polyline, sourceProjection));
    //         }
    //         this.ProjectionSystem = sourceProjection;
    //         this.Geometries = this.GetGeometries();
    //     }
    //
    //     public MultiGeoLineString(IEnumerable<LineString> lineStrings, Plane sourceBasis)
    //     {
    //         foreach (var lineString in lineStrings)
    //         {
    //             this.Objects.Add(new GeoLineString(lineString, sourceBasis));
    //         }
    //         var srids = lineStrings.Select(ls => ls.SRID);
    //         this.ProjectionSystem = new ProjectionSystem(srids.Condense<int>(), sourceBasis);
    //         this.Geometries = this.GetGeometries();
    //     }
    //
    //     private Transform TargetTransform()
    //     {
    //         return Rhino.Geometry.Transform.ChangeBasis(Plane.WorldXY, this.ProjectionSystem.SourceBasis);
    //     }
    //
    //     public void ToTargetBasis()
    //     {
    //         this.Objects.Select(ls => ls.Transform(this.TargetTransform()));
    //     }
    //
    //     public void ToSourceBasis()
    //     {
    //         Transform inverseTransform;
    //         this.TargetTransform().TryGetInverse(out inverseTransform);
    //         this.Objects.Select(ls => ls.Transform(inverseTransform));
    //     }
    // }
}
